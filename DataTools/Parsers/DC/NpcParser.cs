using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using DCTools;
using DCTools.Structures;
using Data.Enums;
using Data.Structures.Npc;
using Data.Structures.Template;
using ProtoBuf;
using Utils;

namespace DataTools.Parsers.DC
{
    class NpcParser
    {
        public static Dictionary<int, Dictionary<int, KeyValuePair<string, string>>> NpcNames;

        public static Dictionary<int, NpcShape> Shapes;

        public static Dictionary<int, Dictionary<int, NpcTemplate>> NpcTemplates;

        public static Dictionary<int, List<SpawnTemplate>> Spawn;

        public static void Parse()
        {
            ParseNpcNames();

            ParseNpcShapes();

            ParseNpcTemplates();

            ParseSpawn();
            
            using (FileStream fs = File.Create(Utils.GetOutput("new/npc_templates.bin")))
            {
                Serializer.Serialize(fs, NpcTemplates);
            }

            using (FileStream fs = File.Create(Utils.GetOutput("new/dc_spawn.bin")))
            {
                Serializer.Serialize(fs, Spawn);
            }
        }

        private static void ParseNpcNames()
        {
            NpcNames = new Dictionary<int, Dictionary<int, KeyValuePair<string, string>>>();
            DataCenter dc = DCT.GetDataCenter();
            int count = 0;
            var objects = dc.GetObjectsByName("StrSheet_Creature");
            foreach (var dcObject in objects)
            {
                 var strings = (Dictionary<string, object>)dc.GetValues(dcObject);
                var huntingZones = (List<Dictionary<string, object>>) strings["HuntingZone"];
                foreach (var zoneData in huntingZones)
                {
                    int huntingZoneId = int.Parse(zoneData["id"].ToString());

                    if (huntingZoneId == 0)
                        continue;

                    NpcNames.Add(huntingZoneId, new Dictionary<int, KeyValuePair<string, string>>());

                    foreach (var data in (List<Dictionary<string, object>>) zoneData["String"])
                    {
                        int id = int.Parse(data["templateId"].ToString());

                        string name = null;

                        if (data.ContainsKey("name"))
                            name = data["name"].ToString();

                        string title = null;
                        if (data.ContainsKey("title") && data["title"].ToString().Length > 0)
                            title = data["title"].ToString();

                        NpcNames[huntingZoneId].Add(id, new KeyValuePair<string, string>(name, title));
                        count++;
                    }
                }
            }

            Console.WriteLine("Loaded {0} npc names...", count);
        }

        private static void ParseNpcShapes()
        {
            Shapes = new Dictionary<int, NpcShape>();
            DataCenter dc = DCT.GetDataCenter();

            foreach (var dcObject in dc.GetMainObjectsByName("NpcShape"))
            {
                var values = dc.GetValues(dcObject);

                if (!values.ContainsKey("Shape"))
                    continue;

                foreach (var data in (List<Dictionary<string, object>>)values["Shape"])
                {
                    NpcShape shape = new NpcShape();
                    int id = int.Parse(data["id"].ToString());

                    if (data.ContainsKey("baseRunSpeed"))
                        shape.RunSpeed = float.Parse(data["baseRunSpeed"].ToString());

                    if (data.ContainsKey("baseTurnSpeed"))
                        shape.TurnSpeed = float.Parse(data["baseTurnSpeed"].ToString());

                    if (data.ContainsKey("baseWalkSpeed"))
                        shape.WalkSpeed = float.Parse(data["baseWalkSpeed"].ToString());

                    if (!Shapes.ContainsKey(id))
                        Shapes.Add(id, shape);
                    else
                        Console.WriteLine("Dublicate npc shape {0}", id);
                }
            }

            Console.WriteLine("Loaded {0} npc shapes...", Shapes.Count);
        }

        private static void ParseNpcTemplates()
        {
            NpcTemplates = new Dictionary<int, Dictionary<int, NpcTemplate>>();
            DataCenter dc = DCT.GetDataCenter();
            int count = 0;

            foreach (var dcObject in dc.GetMainObjectsByName("NpcData"))
            {
                var zoneData = dc.GetValues(dcObject);
                int huntingZoneId = int.Parse(zoneData["huntingZoneId"].ToString());

                NpcTemplates.Add(huntingZoneId, new Dictionary<int, NpcTemplate>());

                foreach (var data in (List<Dictionary<string, object>>)zoneData["Template"])
                {
                    NpcTemplate template = new NpcTemplate();

                    template.HuntingZoneId = huntingZoneId;

                    template.Id = int.Parse(data["id"].ToString());

                    template.Scale = data.ContainsKey("scale")
                                         ? float.Parse(data["scale"].ToString())
                                         : 1f;

                    if (data.ContainsKey("size") && data["size"].ToString().Length > 0)
                        template.Size = ParseEnum<NpcSize>(data["size"]);
                    else
                        template.Size = NpcSize.Medium;

                    int shapeId = int.Parse(data["shapeId"].ToString());

                    template.Shape = Shapes[shapeId];

                    try
                    {
                        template.Level =
                            int.Parse(((List<Dictionary<string, object>>) data["Stat"])[0]["level"].ToString());
                    }
                    catch
                    {
                        template.Level = 1;
                    }

                    template.Name = null;
                    template.Title =null;

                    if (NpcNames.ContainsKey(huntingZoneId) && NpcNames[huntingZoneId].ContainsKey(template.Id))
                    {
                        template.Name = NpcNames[huntingZoneId][template.Id].Key;
                        template.Title = NpcNames[huntingZoneId][template.Id].Value;
                    }

                    //resourceSize

                    //resourceType

                    //class

                    if (data.ContainsKey("gender") && data["gender"].ToString().Length > 0)
                        try
                        {
                            template.Gender = ParseEnum<Gender>(data["gender"]);
                        }
                        catch
                        {
                            //Nothing
                        }

                    if (data.ContainsKey("race") && data["race"].ToString().Length > 0)
                        template.Race = data["race"].ToString();
                    else
                        template.Race = null;

                    if (data.ContainsKey("parentId"))
                        template.ParentId = int.Parse(data["parentId"].ToString());

                    //despawnScriptId

                    //basicActionId

                    if (data.ContainsKey("collideOnMove"))
                        template.CollideOnMove = bool.Parse(data["collideOnMove"].ToString());

                    if (data.ContainsKey("cameraPenetratable"))
                        template.CameraPenetratable = bool.Parse(data["cameraPenetratable"].ToString());

                    if (data.ContainsKey("isHomunculus"))
                        template.IsHomunculus = bool.Parse(data["isHomunculus"].ToString());

                    //deathShapeId

                    if (data.ContainsKey("dontTurn"))
                        template.DontTurn = bool.Parse(data["dontTurn"].ToString());

                    if (data.ContainsKey("villager"))
                        template.IsVillager = bool.Parse(data["villager"].ToString());

                    if (data.ContainsKey("isObjectNpc"))
                        template.IsObject = bool.Parse(data["isObjectNpc"].ToString());

                    if (data.ContainsKey("elite"))
                        template.IsElite = bool.Parse(data["elite"].ToString());

                    if (data.ContainsKey("unionElite"))
                        template.IsUnionElite = bool.Parse(data["unionElite"].ToString());

                    if (data.ContainsKey("isFreeNamed"))
                        template.IsFreeNamed = bool.Parse(data["isFreeNamed"].ToString());

                    if (data.ContainsKey("showAggroTarget"))
                        template.ShowAggroTarget = bool.Parse(data["showAggroTarget"].ToString());

                    //spawnScriptId

                    if (data.ContainsKey("isSpirit"))
                        template.IsSpirit = bool.Parse(data["isSpirit"].ToString());

                    if (data.ContainsKey("isWideBroadcaster"))
                        template.IsWideBroadcaster = bool.Parse(data["isWideBroadcaster"].ToString());

                    //villagerVolumeActiveRange

                    //villagerVolumeHalfHeight

                    //villagerVolumeInteractionDist

                    //villagerVolumeOffset

                    if (data.ContainsKey("showShorttermTarget"))
                        template.ShowShorttermTarget = bool.Parse(data["showShorttermTarget"].ToString());

                    //__value__

                    //cannotPassThrough

                    //vehicleEx

                    //NamePlate

                    //VehicleType

                    //SeatList

                    //Shader

                    //DeathEffect

                    //Stat (Level)

                    //AltAnimation

                    //Emoticon

                    if (!NpcTemplates[huntingZoneId].ContainsKey(template.Id))
                        NpcTemplates[huntingZoneId].Add(template.Id, template);
                    else
                        Console.WriteLine("Dublicate NPC template: {0},{1}", huntingZoneId, template.Id);

                    count++;
                }
            }

            Console.WriteLine("Loaded {0} npc templates...", count);
        }

        private static void ParseSpawn()
        {
            Spawn = new Dictionary<int, List<SpawnTemplate>>();
            DataCenter dc = DCT.GetDataCenter();
            int count = 0;

            foreach (var dcObject in dc.GetMainObjectsByName("StrSheet_NpcLoc"))
                count += ParseSpawnData(dc.GetValues(dcObject));

            foreach (var dcObject in dc.GetMainObjectsByName("StrSheet_NpcLocManual"))
                count += ParseSpawnData(dc.GetValues(dcObject));

            Console.WriteLine("Loaded {0} spawn templates...", count);
        }

        private static int ParseSpawnData(Dictionary<string, object> spawnData)
        {
            int count = 0;

            foreach (var data in (List<Dictionary<string, object>>) spawnData["String"])
            {
                int huntingZoneId = int.Parse(data["huntingZoneId"].ToString());
                int templateId = int.Parse(data["templateId"].ToString());

                string[] arr = data["string"].ToString().Split(new[] {'|', '#'});

                for (int i = 0; i < arr.Length; i++)
                {
                    int mapId = int.Parse(arr[i++]);
                    string[] coords = arr[i].Split(new[] {','});

                    float x = float.Parse(coords[0].Replace('.', ','));
                    float y = float.Parse(coords[1].Replace('.', ','));
                    float z = float.Parse(coords[2].Replace('.', ','));

                    SpawnTemplate template = new SpawnTemplate
                                                 {
                                                     MapId = mapId,
                                                     NpcId = templateId,
                                                     Type = (short) huntingZoneId,
                                                     X = x,
                                                     Y = y,
                                                     Z = z,
                                                     Heading = (short) Funcs.Random().Next(short.MinValue, short.MaxValue)
                                                 };

                    if (!Spawn.ContainsKey(mapId))
                        Spawn.Add(mapId, new List<SpawnTemplate>());

                    Spawn[mapId].Add(template);
                    count++;
                }
            }

            return count;
        }

        private static T ParseEnum<T>(object o)
        {
            return (T) Enum.Parse(typeof (T), Regex.Replace(o.ToString(), "[^a-z0-9]", "", RegexOptions.IgnoreCase), true);
        }
    }
}