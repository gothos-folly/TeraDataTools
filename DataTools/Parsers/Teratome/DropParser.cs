using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Data.Structures.Template;
using ProtoBuf;

namespace DataTools.Parsers.Teratome
{
    class DropParser
    {
        public static Dictionary<int, List<int>> Drop;

        protected static object DropLock = new object();

        public static void Parse()
        {
            Drop = new Dictionary<int, List<int>>();

            DC.NpcParser.Parse();

            var npcTemplates = DC.NpcParser.NpcTemplates;

            Parallel.ForEach(
                npcTemplates,
                pair => Parallel.ForEach(pair.Value, valuePair => ParseFor(valuePair.Value)));

            string binPath = Path.GetFullPath("../../../../datapack/gameserver/data/");

            using (FileStream fs = File.Create(binPath + "drop.bin"))
            {
                Serializer.Serialize(fs, Drop);
            }
        }

        private static void ParseFor(NpcTemplate npcTemplate)
        {
            if (npcTemplate.Level < 1
                //|| npcTemplate.Level > 12
                || npcTemplate.IsObject || npcTemplate.IsVillager)
                return;

            int fullId = npcTemplate.FullId;

            lock (DropLock)
            {
                if (Drop.ContainsKey(fullId))
                    return;

                Drop.Add(fullId, new List<int>());
            }

            int page = 1;
            string name = "";

            while (true)
            {
                string npcHtml = "";

                if (page == 1)
                    npcHtml = Utils.LoadPage("http://www.teratome.com/npc/" + fullId);
                else
                    npcHtml = Utils.LoadPage("http://www.teratome.com/npc/" + fullId + "/" + name + "/detail/page/" + page + "/tab/drops");

                if (npcHtml.Length == 0)
                    return;

                foreach (Match itemMatch in Regex.Matches(npcHtml, "<tr data-href=\"/item/([0-9]+)/"))
                    Drop[fullId].Add(int.Parse(itemMatch.Groups[1].Value));

                if (name.Length == 0)
                {
                    Match nameMatch = Regex.Match(
                        npcHtml, "<link href=\"http://www.teratome.com/npc/" + fullId + "/([^\"]+)\"");

                    name = nameMatch.Groups[1].Value;
                }

                Match match = Regex.Match(npcHtml, "<a class=\"link-next\" href=\"#drops;page:([0-9]+)\">Next");

                if (!match.Success)
                    return;

                page = int.Parse(match.Groups[1].Value);
            }
        }
    }
}
