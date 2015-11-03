using System;
using System.Collections.Generic;
using System.IO;
using DCTools;
using DCTools.Structures;
using Data.Enums.Gather;
using Data.Structures.Template;
using Data.Structures.Template.Gather;
using Data.Structures.World;
using ProtoBuf;

namespace DataTools.Parsers.DC
{
    class GatherParser
    {
        public static Dictionary<int, GatherTemplate> Templates;

        public static List<GSpawnTemplate> GatherSpawnTemplates;

        public static DataCenter DC = DCT.GetDataCenter();

        public static void Parse()
        {
            ParseTemplates();

            ParseSpawn();

            string binPath = Path.GetFullPath("../../../../datapack/gameserver/data/");

            using (FileStream fs = File.Create(binPath + "gather_templates.bin"))
            {
                Serializer.Serialize(fs, Templates);
            }

            using (FileStream fs = File.Create(binPath + "spawn/dc_gather_spawns.bin"))
            {
                Serializer.Serialize(fs, GatherSpawnTemplates);
            }
        }

        public static void ParseTemplates()
        {
            Templates = new Dictionary<int, GatherTemplate>();

            foreach (var cnt in DC.GetMainObjectsByName("Collections"))
            {
                foreach (var data in (List<Dictionary<string, object>>)DC.GetValues(cnt)["Collection"])
                {
                    GatherTemplate t = new GatherTemplate();
                    t.CollectionId = int.Parse(data["collectionId"].ToString());

                    if (data.ContainsKey("despawnDuration"))
                    {
                        t.DespawnDuration = int.Parse(data["despawnDuration"].ToString());
                        t.DespawnEffectId = int.Parse(data["despawnEffectId"].ToString());
                        t.DespawnStartTime = int.Parse(data["despawnStartTime"].ToString());
                    }

                    t.Grade = int.Parse(data["grade"].ToString());
                    t.Height = int.Parse(data["height"].ToString());
                    t.Name = int.Parse(data["name"].ToString());
                    t.NeededProficiency = int.Parse(data["neededProficiency"].ToString());
                    t.PickSkillType = int.Parse(data["pickSkillType"].ToString());
                    t.QuestCollection = bool.Parse(data["questCollection"].ToString());
                    t.ResourceSize = int.Parse(data["resourceSize"].ToString());
                    t.ResourceType = (ResourceType) Enum.Parse(typeof (ResourceType), data["resourceType"].ToString());
                    t.ShowNamePlate = bool.Parse(data["showNamePlate"].ToString());
                    t.TypeName = (TypeName) Enum.Parse(typeof (TypeName), data["typeName"].ToString());

                    Templates.Add(t.CollectionId, t);
                }
            }
        }

        private static void ParseSpawn()
        {
            GatherSpawnTemplates = new List<GSpawnTemplate>();

            foreach (var dcObject in DC.GetMainObjectsByName("StrSheet_CollectionLoc"))
            {
                foreach (var data in (List<Dictionary<string, object>>)DC.GetValues(dcObject)["String"])
                {
                    int templateId = int.Parse(data["templateId"].ToString());

                    string[] arr = data["string"].ToString().Split(new[] {'|', '#'});

                    for (int i = 0; i < arr.Length; i++)
                    {
                        int mapId = int.Parse(arr[i++]);
                        string[] coords = arr[i].Split(new[] {','});

                        float x = float.Parse(coords[0].Replace('.', ','));
                        float y = float.Parse(coords[1].Replace('.', ','));
                        float z = float.Parse(coords[2].Replace('.', ','));

                        GSpawnTemplate template = new GSpawnTemplate
                            {
                                CollectionId = templateId,
                                WorldPosition = new WorldPosition
                                    {
                                        MapId = mapId,
                                        X = x,
                                        Y = y,
                                        Z = z,
                                    }
                            };

                        GatherSpawnTemplates.Add(template);
                    }
                }
            }
        }
    }
}
