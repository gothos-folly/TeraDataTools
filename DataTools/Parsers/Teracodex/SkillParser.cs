using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Data.Enums;
using DataTools.Structures.Teracodex;
using ProtoBuf;

namespace DataTools.Parsers.Teracodex
{
    class SkillParser
    {
        public static List<Skill> GetSkills()
        {
            if (!File.Exists(Utils.DataDirectory + "/teracodex/skill_data.bin"))
                Parse();

            List<Skill> skills = new List<Skill>();

            using (FileStream fileStream = File.OpenRead(Utils.DataDirectory + "/teracodex/skill_data.bin"))
            {
                while (fileStream.Position < fileStream.Length)
                {
                    skills.Add(Serializer.DeserializeWithLengthPrefix<Skill>(fileStream, PrefixStyle.Fixed32));
                }
            }

            return skills;
        }

        public static void Parse()
        {
            List<Skill> skills = new List<Skill>();

            for (int i = 1; i < 9; i++)
            {
                string html = Regex.Replace(Utils.LoadPage("http://teracodex.com/skill.c=" + i), "[\\n]", "");

                Match m = Regex.Match(html, "<title>([a-z]+) Skills - TERA Codex</title>", RegexOptions.IgnoreCase);

                PlayerClass playerClass = (PlayerClass) Enum.Parse(typeof (PlayerClass), m.Groups[1].Value, true);

                MatchCollection collection = Regex.Matches(html, "<tr>[\\s\\n]*<td[^>]*><img[^>]*>[\\s\\n]*<a class=\"grade_common codexToolTip\" rel=\"([0-9]+)\"[^>]*>([^<]*)</a>");

                foreach (Match match in collection)
                {
                    Skill skill = new Skill
                                      {
                                          CodexId = int.Parse(match.Groups[1].Value),
                                          Name = match.Groups[2].Value,
                                          PlayerClass = playerClass,
                                      };

                    string skillPage = Utils.LoadPage("http://teracodex.com/skill/" + skill.CodexId);

                    m = Regex.Match(skillPage, "<td class=\"stls\">Cast Time</td><td>([^<]+)</td>");
                    if (!m.Success || m.Groups[1].Value.Trim().Equals("Instant"))
                        skill.CastTime = 0;
                    else
                        skill.CastTime =
                            (int)
                            (float.Parse(
                                m.Groups[1].Value.Trim().Replace(" second", "").Replace("s", "").Replace('.', ','))*
                             1000);


                    m = Regex.Match(skillPage, "<td class=\"stls\">Skill Power</td><td>([^<]+)</td>");
                    skill.Power = m.Success ? m.Groups[1].Value.Trim() : "0";

                    skills.Add(skill);
                }

                Console.WriteLine("Finded {0} skills...", collection.Count);
            }

            using (FileStream fileStream = File.Create(Utils.DataDirectory + "/teracodex/skill_data.bin"))
            {
                foreach (Skill skill in skills)
                {
                    Serializer.SerializeWithLengthPrefix(fileStream, skill, PrefixStyle.Fixed32);
                }
            }
        }
    }
}
