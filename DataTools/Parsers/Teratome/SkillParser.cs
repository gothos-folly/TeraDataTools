using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Data.Enums;
using DataTools.Structures.Teratome;
using ProtoBuf;

namespace DataTools.Parsers.Teratome
{
    class SkillParser
    {
        public static List<Skill> GetSkills()
        {
            if (!File.Exists(Utils.DataDirectory + "/teratome/skill_data.bin"))
                Parse();

            List<Skill> skills = new List<Skill>();

            using (FileStream fileStream = File.OpenRead(Utils.DataDirectory + "/teratome/skill_data.bin"))
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

            for (int i = 1; i < 21; i++)
            {
                string html = Regex.Replace(Utils.LoadPage("http://www.teratome.com/skills/page/" + i), "[\\n]", "");

                MatchCollection collection = Regex.Matches(html, "<tr data-href=\"/skill/([0-9]+)[^>]{0,}><td[^>]{0,}><div[^>]{0,}><i></i><ins[^>]{0,}></ins><del></del><a[^>]{0,}></a></div><span><a[^>]{0,}>([^<]{0,})</a></span></td><td[^>]{0,}><div[^>]{0,}><i></i><ins[^>]{0,}></ins><del></del><a href=\"/class/[0-9]/([^\"]{0,})\"></a></div></td><td[^>]{0,}></td><td[^>]{0,}>([0-9]+)</td><td[^>]{0,}>[^<]{0,}</td><td[^>]{0,}>[^<]{0,}</td><td[^>]{0,}>((?:(?<!</td).)*)</td></tr>");

                foreach (Match match in collection)
                {
                    Skill skill = new Skill
                                      {
                                          Id = int.Parse(match.Groups[1].Value),
                                          Name = match.Groups[2].Value,
                                          PlayerClass = (PlayerClass) Enum.Parse(typeof(PlayerClass), match.Groups[3].Value, true),
                                          Level = int.Parse(match.Groups[4].Value),
                                          Cost = 0,
                                          HpCost = 0,
                                          MpCost = 0,
                                      };

                    MatchCollection costCollection = Regex.Matches(match.Groups[5].Value, "<span class=\"tt-money ([a-z]+)\">([0-9]+)</span>");
                    foreach (Match costMatch in costCollection)
                    {
                        switch (costMatch.Groups[1].Value)
                        {
                            case "copper":
                                skill.Cost += int.Parse(costMatch.Groups[2].Value);
                                break;
                            case "silver":
                                skill.Cost += int.Parse(costMatch.Groups[2].Value) * 100;
                                break;
                            case "gold":
                                skill.Cost += int.Parse(costMatch.Groups[2].Value) * 10000;
                                break;
                            default:
                                Console.WriteLine("Unknown cost type: {0}", costMatch.Groups[1].Value);
                                break;
                        }
                    }

                    string skillPage = Utils.LoadPage("http://www.teratome.com/skill/" + skill.Id);

                    Match m = Regex.Match(skillPage, "<td>MP cost</td><td>([^<]+)</td>");
                    if (m.Success)
                        skill.MpCost = int.Parse(m.Groups[1].Value);

                    m = Regex.Match(skillPage, "<td>HP cost</td><td>([^<]+)</td>");
                    if (m.Success)
                        skill.HpCost = int.Parse(m.Groups[1].Value);

                    m = Regex.Match(skillPage, "<td>Cooldown</td><td>([^<]+)</td>");
                    if (m.Success)
                    {
                        if (m.Groups[1].Value.IndexOf(" sec", StringComparison.OrdinalIgnoreCase) != -1)
                            skill.Cooldown = (int) (float.Parse(m.Groups[1].Value.Replace(" sec", "")) * 1000);
                        else
                            skill.Cooldown = int.Parse(m.Groups[1].Value);
                    }

                    m = Regex.Match(skillPage, "<div class=\"tttt-desc\">((?:(?<!</div).)*)</div>");
                    skill.Description = m.Groups[1].Value;

                    skills.Add(skill);
                }

                Console.WriteLine("Finded {0} skills...", collection.Count);
            }

            using (FileStream fileStream = File.Create(Utils.DataDirectory + "/teratome/skill_data.bin"))
            {
                foreach (Skill skill in skills)
                {
                    Serializer.SerializeWithLengthPrefix(fileStream, skill, PrefixStyle.Fixed32);
                }
            }
        }
    }
}
