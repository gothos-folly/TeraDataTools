using System.Collections.Generic;
using System.IO;
using DCTools;
using DCTools.Structures;
using ProtoBuf;

namespace DataTools.Parsers.DC
{
    class CooldownGroupsParser
    {
        public static DataCenter DC = DCT.GetDataCenter();

        public static  Dictionary<int, List<int>> Groups; //groupd => List<itemid>

        public static void Parse()
        {
            string binPath = Path.GetFullPath("../../../../datapack/gameserver/data/");

            Groups = new Dictionary<int, List<int>>();
            ParseGroups();

            using (FileStream fs = File.Create(binPath + "item_cooldown_groups.bin"))
                Serializer.Serialize(fs, Groups);
        }

        public static void ParseGroups()
        {
            Groups = new Dictionary<int, List<int>>();

            foreach (var cnt in DC.GetMainObjectsByName("ItemCoolTimeGroup"))
            {
                foreach (var data in (List<Dictionary<string, object>>)DC.GetValues(cnt)["Group"])
                {
                    int id = int.Parse(data["id"].ToString());

                    Groups.Add(id, new List<int>());

                    foreach (var itm in (List<Dictionary<string, object>>)data["Item"])
                    {
                        Groups[id].Add(int.Parse(itm["id"].ToString()));
                    }
                }
            }
        }
    }
}
