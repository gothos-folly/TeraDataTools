using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using DataTools.Structures.Teratome;
using ProtoBuf;
using Npc = DataTools.Structures.Teracodex.Npc;

namespace DataTools.Parsers.Teracodex
{
    class NpcParser
    {
        public static List<Npc> GetNpcs()
        {
            if (!File.Exists(Utils.DataDirectory + "/teracodex/npc_data.bin"))
                Parse();

            List<Npc> npcs = new List<Npc>();

            using (FileStream fileStream = File.OpenRead(Utils.DataDirectory + "/teracodex/npc_data.bin"))
            {
                while (fileStream.Position < fileStream.Length)
                {
                    npcs.Add(Serializer.DeserializeWithLengthPrefix<Npc>(fileStream, PrefixStyle.Fixed32));
                }
            }

            return npcs;
        }

        public static void Parse()
        {
            List<Item> teratomeItems = Teratome.ItemParser.GetItems();

            if (!Directory.Exists(Utils.DataDirectory + "/teracodex"))
                Directory.CreateDirectory(Utils.DataDirectory + "/teracodex");

            using (FileStream fs = File.Create(Utils.DataDirectory + "/teracodex/npc_data.bin"))
            {
                for (int i = 1; i <= 6; i++)
                {
                    string html = Utils.LoadPage(@"http://teracodex.com/npc.t=" + i);

                    MatchCollection matches = Regex.Matches(html, "<tr>[\\s\\n]*<td[^>]*><img[^>]*>[\\s\\n]*<a[^>]*href=\"/npc/([0-9]+)\">([^<]+)</a>[\\s\\n]*</td>[\\s\\n]*<td>([0-9]*)</td>[\\s\\n]*<td>([^<]*)</td>[\\s\\n]*<td><a href=\"/area/([0-9]+)\">([^<]*)</a></td>[\\s\\n]*</tr>");

                    foreach (Match match in matches)
                    {
                        Npc npc = new Npc
                                      {
                                          CodexId = int.Parse(match.Groups[1].Value),
                                          Name = match.Groups[2].Value,
                                          Level = int.Parse("0" + match.Groups[3].Value),
                                          Title = match.Groups[4].Value,
                                          CodexLocationId = int.Parse(match.Groups[5].Value),
                                          LocationName = match.Groups[4].Value,
                                      };

                        string npcHtml = Utils.LoadPage(@"http://teracodex.com/npc/" + npc.CodexId);

                        MatchCollection dropMatches = Regex.Matches(npcHtml, "<tr><td[^>]*><span[^>]*><a[^>]*><img[^>]*><span>([^<]*)</span></a></span></td>[\\s\\n]*<td>[^<]*</td>[\\s\\n]*<td>([^<]*)</td>[\\s\\n]*</tr>");

                        foreach (Match dropMatch in dropMatches)
                        {
                            int itemId = -1;

                            for (int j = 0; j < teratomeItems.Count; j++)
                            {
                                if(teratomeItems[j].Name.Equals(dropMatch.Groups[1].Value, StringComparison.OrdinalIgnoreCase))
                                {
                                    itemId = teratomeItems[j].Id;
                                    break;
                                }
                            }

                            if (itemId != -1)
                                npc.Drop.Add(new KeyValuePair<int, string>(itemId, dropMatch.Groups[2].Value));
                            else
                                Console.WriteLine("Unknown item {0}...", dropMatch.Groups[1].Value);
                        }

                        Serializer.SerializeWithLengthPrefix(fs, npc, PrefixStyle.Fixed32);
                    }
                }
            }
        }
    }
}