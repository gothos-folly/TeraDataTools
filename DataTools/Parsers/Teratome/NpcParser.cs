using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using DataTools.Structures.Teratome;
using ProtoBuf;

namespace DataTools.Parsers.Teratome
{
    class NpcParser
    {
        public static List<Npc> GetNpcs()
        {
            if (!File.Exists(Utils.DataDirectory + "/teratome/npc_data.bin"))
                Parse();

            List<Npc> npcs = new List<Npc>();

            using (FileStream fileStream = File.OpenRead(Utils.DataDirectory + "/teratome/npc_data.bin"))
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
            int total = 0;

            if (!Directory.Exists(Utils.DataDirectory + "/teratome"))
                Directory.CreateDirectory(Utils.DataDirectory + "/teratome");

            using (FileStream fs = File.Create(Utils.DataDirectory + "/teratome/npc_data.bin"))
            {
                for (int i = 0; i < 127; i++)
                {
                    string html = Utils.LoadPage("http://www.teratome.com/npcs/page/" + (i + 1));

                    MatchCollection collection = Regex.Matches(html,
                                                               "<tr data-href=\"/npc/([0-9]+)/[^\"]+\"[^>]{0,}><td[^>]{0,}><a href=\"/npc/[0-9]+/[^\"]+\"[^>]{0,}>([^<]+)</a>(?:(?<!</td).)*</td><td[^>]{0,}>([0-9]+)</td><td[^>]{0,}>(?:<a href=\"/zone/([0-9]+)[^\"]{0,}\"[^>]{0,}>([^<]{0,})</a>){0,1}</td></tr>");

                    Console.WriteLine("Finded {0} npcs...", collection.Count);

                    foreach (Match match in collection)
                    {
                        Npc npc = new Npc
                                      {
                                          FullId = int.Parse(match.Groups[1].Value),
                                          Name = match.Groups[2].Value,
                                          Level = int.Parse(match.Groups[3].Value),
                                      };

                        string npcHtml = Utils.LoadPage("http://www.teratome.com/npc/" + npc.FullId);

                        foreach (Match itemMatch in Regex.Matches(npcHtml, "<tr data-href=\"/item/([0-9]+)/"))
                            npc.Drop.Add(int.Parse(itemMatch.Groups[1].Value));

                        Serializer.SerializeWithLengthPrefix(fs, npc, PrefixStyle.Fixed32);
                    }
                }
            }
        }
    }
}
