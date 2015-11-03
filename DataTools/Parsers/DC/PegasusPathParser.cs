using System;
using System.Collections.Generic;
using System.IO;
using DCTools;
using DCTools.Structures;
using Data.Enums.Pegasus;
using Data.Structures.Geometry;
using Data.Structures.World;
using Data.Structures.World.Pegasus;
using ProtoBuf;

namespace DataTools.Parsers.DC
{
    class PegasusPathParser
    {
        public static Dictionary<int, Dictionary<PType, PegasusPath>> PegasusPaths;

        public static Dictionary<int, List<FlyTeleport>> Teleports;

        public static DataCenter DC = DCT.GetDataCenter();

        public static void Parse()
        {
            string binPath = Path.GetFullPath("../../../../datapack/gameserver/data/");

            ParsePPoints();

            using (FileStream fs = File.Create(binPath + "pegasus_paths.bin"))
                Serializer.Serialize(fs, PegasusPaths);

            using (FileStream fs = File.Create(binPath + "fly_teleports.bin"))
                Serializer.Serialize(fs, Teleports);
        }

        public static void ParsePPoints()
        {
            PegasusPaths = new Dictionary<int, Dictionary<PType, PegasusPath>>();
            Teleports = new Dictionary<int, List<FlyTeleport>>();

            foreach (var cnt in DC.GetMainObjectsByName("PegasusPaths"))
            {
                foreach (var data in (List<Dictionary<string, object>>) DC.GetValues(cnt)["PegasusPath"])
                {
                    PegasusPath pegasusPath = new PegasusPath();
                    pegasusPath.Index = int.Parse(data["index"].ToString());
                    pegasusPath.Stage = new PPStage();

                    foreach (Dictionary<string, object> dictionary in (List<Dictionary<string, object>>) data["Stage"])
                    {
                        pegasusPath.Stage.ContinentId = int.Parse(dictionary["continentId"].ToString());
                        pegasusPath.Stage.Type =
                            (PType) Enum.Parse(typeof (PType), dictionary["type"].ToString());

                        pegasusPath.Stage.FlySteps = new List<FlyStep>();

                        foreach (var seq in (List<Dictionary<string, object>>) dictionary["Seq"])
                        {
                            FlyStep fs = new FlyStep();
                            fs.Time = int.Parse(seq["time"].ToString());
                            fs.Rot = TransformStringArrayToIntArray(seq["rot"].ToString().Split(','));
                            fs.Loc = TransformStringArrayToPoint3D(seq["loc"].ToString().Split(','));

                            if (seq.ContainsKey("state"))
                                fs.State = int.Parse(seq["state"].ToString());

                            if (seq.ContainsKey("stateCount"))
                                fs.StateCounter = int.Parse(seq["stateCount"].ToString());

                            pegasusPath.Stage.FlySteps.Add(fs);
                        }
                    }

                    if (!PegasusPaths.ContainsKey(pegasusPath.Index))
                        PegasusPaths.Add(pegasusPath.Index, new Dictionary<PType, PegasusPath>());

                    PegasusPaths[pegasusPath.Index].Add(pegasusPath.Stage.Type, pegasusPath);
                }
            }

            foreach (KeyValuePair<int, Dictionary<PType, PegasusPath>> pegasusPath in PegasusPaths)
            {
                foreach (KeyValuePair<PType, PegasusPath> path in pegasusPath.Value)
                {
                    if(path.Key == PType.normal)
                    {
                        if(!Teleports.ContainsKey(path.Value.Stage.ContinentId))
                            Teleports.Add(path.Value.Stage.ContinentId, new List<FlyTeleport>());
                        FlyTeleport f = new FlyTeleport();

                        f.Id = path.Value.Index;
                        f.FromNameId = int.Parse(path.Value.Stage.ContinentId + "001");
                        f.Cost = 1000;


                        if (PegasusPaths[path.Value.Index].ContainsKey(PType.high_after))
                            f.ToNameId = int.Parse(PegasusPaths[path.Value.Index][PType.high_after].Stage.ContinentId + "001");

                        Teleports[path.Value.Stage.ContinentId].Add(f);
                    }
                }
            }
        }

        private static Point3D TransformStringArrayToPoint3D(string[] str)
        {
            return new Point3D
            {
                X = Convert.ToSingle(str[0].Replace(".", ",")),
                Y = Convert.ToSingle(str[1].Replace(".", ",")),
                Z = Convert.ToSingle(str[2].Replace(".", ","))
            };
        }

        private static int[] TransformStringArrayToIntArray(string[] str)
        {
            return new []
                       {
                           Convert.ToInt32(str[0]),
                           Convert.ToInt32(str[1]),
                           Convert.ToInt32(str[2])
                       };
        }
    }
}
