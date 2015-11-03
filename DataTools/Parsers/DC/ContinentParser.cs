using System;
using System.Collections.Generic;
using System.IO;
using DCTools;
using DCTools.Structures;
using Data.Structures.Geometry;
using Data.Structures.World.Continent;
using ProtoBuf;

namespace DataTools.Parsers.DC
{
    class ContinentParser
    {
        public static Dictionary<int, Continent> Continents;

        public static Dictionary<int, List<Area>> Areas;

        public static Dictionary<int, string> Names; 

        public static DataCenter DC = DCT.GetDataCenter();

        public static void Parse()
        {
            LoadNames();
            LoadAreas();
            ParseContinents();

            string binPath = Path.GetFullPath("../../../../datapack/gameserver/data/");

            using (FileStream fs = File.Create(binPath + "continents.bin"))
                Serializer.Serialize(fs, Continents);

            using (FileStream fs = File.Create(binPath + "areas.bin"))
                Serializer.Serialize(fs, Areas);
        }

        protected static void ParseContinents()
        {
            Continents = new Dictionary<int, Continent>();

            foreach (var cnt in DC.GetMainObjectsByName("AreaList"))
            {
                foreach (var data in (List<Dictionary<string, object>>)DC.GetValues(cnt)["Continent"])
                {
                    Continent continent = new Continent
                                              {
                                                  Id = int.Parse(data["id"].ToString()),
                                                  Description = data["desc"].ToString(),
                                                  OriginZoneX = int.Parse(data["originZoneX"].ToString()),
                                                  OriginZoneY = int.Parse(data["originZoneY"].ToString()),
                                                  Areas = new List<Area>()
                                              };

                    foreach (Dictionary<string, object> dictionary in (List<Dictionary<string, object>>)data["Area"])
                    {
                        Area area = new Area();

                        if(dictionary.ContainsKey("Zones"))
                        {
                            area.Zones = new List<KeyValuePair<int, int>>();

                            foreach (Dictionary<string, object> keyValuePair in (List<Dictionary<string, object>>)dictionary["Zones"])
                            {
                                KeyValuePair<int, int> zone = new KeyValuePair<int, int>();

                                foreach (Dictionary<string, object> valuePair in (List<Dictionary<string, object>>)keyValuePair["Zone"])
                                {
                                    zone = new KeyValuePair<int, int>(int.Parse(valuePair["x"].ToString()), int.Parse(valuePair["y"].ToString()));
                                }
                                area.Zones.Add(zone);
                            }
                        }
                    }

                    Continents.Add(continent.Id, continent);
                }
            }
        }

        protected static void LoadAreas()
        {
            Areas = new Dictionary<int, List<Area>>();
            foreach (var area in DC.GetMainObjectsByName("Area"))
            {
                var data = DC.GetValues(area);
                Area a = new Area();
                a.ContinentId = int.Parse(data["continentId"].ToString());
                a.NameId = int.Parse(data["nameId"].ToString());
                a.AreaName = data["areaName"].ToString();
                a.WorldMapGuardId = int.Parse(data["worldMapGuardId"].ToString());
                a.WorldMapWorldId = int.Parse(data["worldMapWorldId"].ToString());

                a.Sections = new List<Section>();

                foreach (var sct in (List<Dictionary<string, object>>)DC.GetValues(area)["Section"])
                {
                    Section s = new Section();
                    s.Priority = int.Parse(sct["priority"].ToString());
                    s.HuntingZoneId = int.Parse(sct["huntingZoneId"].ToString());
                    s.AddMaxZ = int.Parse(sct["addMaxZ"].ToString());
                    s.CampId = int.Parse(sct["campId"].ToString());
                    s.Destext = bool.Parse(sct["desTex"].ToString());
                    s.Floor = int.Parse(sct["floor"].ToString());
                    s.NameId = int.Parse(sct["nameId"].ToString());
                    s.PcMoveCylinder = bool.Parse(sct["pcMoveCylinder"].ToString());
                    s.PcSafe = sct["pk"].ToString() == "safe";

                    if (sct.ContainsKey("restBonus"))
                        s.RestBonus = bool.Parse(sct["restBonus"].ToString());

                    s.SubtractMinZ = int.Parse(sct["subtractMinZ"].ToString());
                    s.Vender = bool.Parse(sct["vender"].ToString());
                    s.WorldMapSectionId = int.Parse(sct["worldMapSectionId"].ToString());

                    if (Names.ContainsKey(s.NameId))
                        s.Name = Names[s.NameId];

                    if (sct.ContainsKey("Fence"))
                        foreach (Dictionary<string, object> fncpoint in (List<Dictionary<string, object>>)sct["Fence"])
                        {
                            if (s.Polygon == null)
                                s.Polygon = new Polygon {PointList = new List<Point3D>()};

                            s.Polygon.PointList.Add(TransformStringArrayToIntArray(fncpoint["pos"].ToString().Split(',')));
                        }

                    if (sct.ContainsKey("Section"))
                    {
                        if (s.Sections== null)
                            s.Sections = new List<Section>();

                        foreach (Dictionary<string, object> dictionary in (List<Dictionary<string, object>>)sct["Section"])
                        {
                            Section sq = new Section();
                            sq.Priority = int.Parse(dictionary["priority"].ToString());
                            sq.HuntingZoneId = int.Parse(dictionary["huntingZoneId"].ToString());
                            sq.AddMaxZ = int.Parse(dictionary["addMaxZ"].ToString());
                            sq.CampId = int.Parse(dictionary["campId"].ToString());
                            sq.Destext = bool.Parse(dictionary["desTex"].ToString());
                            sq.Floor = int.Parse(dictionary["floor"].ToString());
                            sq.NameId = int.Parse(dictionary["nameId"].ToString());
                            sq.PcMoveCylinder = bool.Parse(dictionary["pcMoveCylinder"].ToString());
                            sq.PcSafe = dictionary["pk"].ToString() == "safe";

                            if (dictionary.ContainsKey("restBonus"))
                                sq.RestBonus = bool.Parse(dictionary["restBonus"].ToString());

                            sq.SubtractMinZ = int.Parse(dictionary["subtractMinZ"].ToString());
                            sq.Vender = bool.Parse(dictionary["vender"].ToString());
                            sq.WorldMapSectionId = int.Parse(dictionary["worldMapSectionId"].ToString());

                            if (Names.ContainsKey(sq.NameId))
                                sq.Name = Names[sq.NameId];

                            if (dictionary.ContainsKey("Fence"))
                                foreach (Dictionary<string, object> fncpoint in (List<Dictionary<string, object>>)dictionary["Fence"])
                                {
                                    if (sq.Polygon == null)
                                        sq.Polygon = new Polygon {PointList = new List<Point3D>()};

                                    sq.Polygon.PointList.Add(TransformStringArrayToIntArray(fncpoint["pos"].ToString().Split(',')));
                                }

                            s.Sections.Add(sq);
                        }
                    }

                    a.Sections.Add(s);
                }

                if(!Areas.ContainsKey(a.ContinentId))
                    Areas.Add(a.ContinentId, new List<Area>());

                Areas[a.ContinentId].Add(a);
            }
        }

        private static void LoadNames()
        {
            Names = new Dictionary<int, string>();

            foreach (var cnt in DC.GetMainObjectsByName("StrSheet_Region"))
            {
                foreach (var data in (List<Dictionary<string, object>>)DC.GetValues(cnt)["String"])
                {
                    Names.Add(int.Parse(data["id"].ToString()), data["string"].ToString());
                }
            }

        }
        private static Point3D TransformStringArrayToIntArray(string[] str)
        {
            return new Point3D
                       {
                           X = Convert.ToSingle(str[0].Replace(".", ",")),
                           Y = Convert.ToSingle(str[1].Replace(".", ",")),
                           Z = Convert.ToSingle(str[2].Replace(".", ","))
                       };
        }
    }
}
