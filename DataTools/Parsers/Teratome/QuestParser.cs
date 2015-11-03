using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Data.Enums;
using Data.Interfaces;
using Data.Structures.Quest;
using ProtoBuf;

namespace DataTools.Parsers.Teratome
{
    class QuestParser
    {
        /*public static string BinPath = Path.GetFullPath("../../../../datapack/gameserver/data/quests.bin");

        public static Dictionary<int, Quest> Quests = new Dictionary<int, Quest>();
 
        //public Dictionary<int, Quest> LoadQuests()
        //{
        //    var q = new Dictionary<int, Quest>();

        //    if (!Directory.Exists("data"))
        //        Directory.CreateDirectory("data");

        //    if (File.Exists(BinPath))
        //    {
        //        if (File.Exists("data/quests.bin"))
        //            File.Delete("data/quests.bin");

        //        File.Copy(BinPath, "data/quests.bin");
        //    }
        //    else
        //        File.Create("data/quests.bin").Dispose();

        //    using (FileStream fileStream = File.OpenRead(Directory.GetCurrentDirectory() + "/data/quests.bin"))
        //    {
        //        while (fileStream.Position < fileStream.Length)
        //        {
        //            Quest qst = Serializer.DeserializeWithLengthPrefix<Quest>(fileStream, PrefixStyle.Fixed32);
        //            q.Add(qst.QuestId, qst);
        //        }
        //    }
        //    return q;
        //}
        public static void Parse()
        {
            List<int> questIds = new List<int>();

            const string url = "http://www.teratome.com/quests/page/";

            for (int i = 1; i < 40; i++)
            {
                string page = Utils.LoadPage(url + i);

                MatchCollection m = Regex.Matches(page, "<tr data-href=\"/quest/([0-9]+)");

// ReSharper disable StringLastIndexOfIsCultureSpecific.1
                questIds.AddRange(from Match mtc in m select Convert.ToInt32(mtc.Value.Replace("<tr data-href=\"/quest/", "")));
// ReSharper restore StringLastIndexOfIsCultureSpecific.1
            }

            const string questUrl = "http://www.teratome.com/quest/";

            foreach (int questId in questIds)
            {
                string questStr = Utils.LoadPage(questUrl + questId);

                Quest fulQuest = new Quest
                                     {
                                         QuestId = questId,
                                         NeedLevel =
                                             Convert.ToInt32(
                                                 Regex.Match(questStr, "Required level: ([0-9]+)").Value.Replace(
                                                     "Required level: ", "")),
                                         Name =
                                             Regex.Match(questStr, "<h1>([^<]+)</h1>").Value.Replace("<h1>", "").
                                             Replace("</h1>", ""),
                                     };

                QuestType type = QuestType.Mission;

                switch (Regex.Match(questStr, "Type: ").Value)
                {
                    case "Zone":
                        type = QuestType.Normal;
                        break;
                    case "Guild":
                        type = QuestType.Guild;
                        break;
                }
                fulQuest.Type = type;

                List<IQuestStep> steps = new List<IQuestStep>();

                string[] stps = Regex.Match(questStr, "<h3>Step 1</h3><ul><li>(.*)</li></ul>").Value.Replace("<h3>", "~").Split('~');

                foreach (string match in stps)
                {
                    if(match.Length < 2)
                        continue;

                    if (match.Contains(" unknown"))
                    {
                        steps.Add(new QStepNotImpliment{Description = match});
                    }

                    else if (match.Contains("<li>Kill "))
                    {
                        steps.Add(new QStepKillNpc
                                      {
                                          NpcName = 
                                                  Regex.Match(match, "<a href=\"/npc/([0-9]+)/").Value.Replace(
                                                      "<a href=\"/npc/", "").Replace("/", ""),
                                          Counter =
                                              Convert.ToInt32(Regex.Match(match, "Kill ([0-9]+)").Value.Replace("Kill ", ""))
                                      });
                    }
                    else if (match.Contains("Travel to "))
                    {
                        steps.Add(new QStepDialogWithNpc
                                      {
                                          NpcName = new List<string> 
                                                  {
                                                          Regex.Match(match, "<a href=\"/npc/([0-9]+)/").Value.Replace(
                                                              "<a href=\"/npc/", "").Replace("/", "")
                                                  }
                                      });
                    }
                    else if (match.Contains("Play cinematic"))
                    {
                        steps.Add(new QStepShowMovie{MovieId = 123456789});
                    }
                    else if (match.Contains("Acquire "))
                    {
                        QStepAcquireItems st = new QStepAcquireItems();

                        st.NpcName = 
                            Regex.Match(match, "<a href=\"/quest/([0-9]+)/").Value.Replace(
                                "<a href=\"/quest/", "").Replace("/", "");

                        string val = Regex.Match(match, "Acquire ([0-9]+) #([0-9]+)").Value;
                        st.Items = new List<KeyValuePair<int, int>>
                                       {
                                           new KeyValuePair<int, int>(
                                               Convert.ToInt32(
                                                   val.Replace("Acquire ", "").Substring(val.Replace("Acquire ", "").IndexOf('#') + 1)),
                                               Convert.ToInt32(
                                                   Regex.Match(match, "Acquire ([0-9]+)").Value.Replace("Acquire ", "")))
                                       };
                        steps.Add(st);

                    }
                    else if (match.Contains("Deliver "))
                    {
                        QStepAcquireItems st = new QStepAcquireItems();

                        st.NpcName = 
                            Regex.Match(match, "<a href=\"/quest/([0-9]+)/").Value.Replace(
                                "<a href=\"/quest/", "").Replace("/", "");

                        string val = Regex.Match(match, "Deliver ([0-9]+)").Value;
                        st.Items = new List<KeyValuePair<int, int>>
                                       {
                                           new KeyValuePair<int, int>(0,
                                               Convert.ToInt32(
                                                   val.Replace("Deliver ", "")))
                                       };
                        steps.Add(st);
                    }

                    else if (match.Contains("Hunt for "))
                    {
                        if (match.Contains("unknown"))
                            steps.Add(new QStepNotImpliment { Description = match });
                        else
                        {
                            steps.Add(new QStepDropNpc
                                          {
                                              NpcName = 
                                                      Regex.Match(match, "<a href=\"/quest/([0-9]+)/").Value.Replace(
                                                          "<a href=\"/quest/", "").Replace("/", ""),
                                              ItemCounter =
                                                  Convert.ToInt32(
                                                      Regex.Match(match, "Hunt for ([0-9]+)").Value.Replace(
                                                          "Hunt for ", ""))
                                          });
                        }
                    }
                    else if(match.Contains("Rewards"))
                    {
                        int money = 0;

                        if (Regex.Match(match, "([0-9,]+) experience").Value.Length > 1)
                            fulQuest.RewardExp =
                                Convert.ToInt32(
                                    Regex.Match(match, "([0-9,]+) experience").Value.Replace(",", "").Replace(
                                        " experience", ""));

                        if (Regex.Match(match, "tt-money gold\">([0-9,]+)").Value.Length > 1)
                            money +=
                                Convert.ToInt32(
                                    Regex.Match(match, "tt-money gold\">([0-9,]+)").Value.Replace("tt-money gold\">", ""))*
                                10000;

                        if (Regex.Match(match, "tt-money silver\">([0-9,]+)").Value.Length > 1)
                            money +=
                                Convert.ToInt32(
                                    Regex.Match(match, "tt-money silver\">([0-9,]+)").Value.
                                        Replace("tt-money silver\">", ""))*
                                100;

                        if (Regex.Match(match, "tt-money copper\">([0-9,]+)").Value.Length > 1)
                            money +=
                                Convert.ToInt32(
                                    Regex.Match(match, "tt-money copper\">([0-9,]+)").Value.
                                        Replace("tt-money copper\">", ""));

                        fulQuest.RewardMoney = money;

                        string[] rwdItms = match.Replace("<ins", "~").Split('~');

                        int rewardCounter = 0;
                        foreach (string rwdItm in rwdItms)
                        {
                            if (rwdItm.Contains("Rewards"))
                                continue;

                            if (rwdItm.Contains("</a> only"))
                            {
                                string prof = Regex.Match(rwdItm, "([a-zA-Z]+)</a> only").Value.Replace("</a> only", "");
                                int itemId =
                                    Convert.ToInt32(
                                        Regex.Match(rwdItm, "<a href=\"/item/([0-9]+)").Value.Replace(
                                            "<a href=\"/item/", ""));
                                int itemCounter =
                                    Convert.ToInt32(Regex.Match(rwdItm, "<b>([0-9]+)<b>").Value.Replace("<b>", ""));

                                if (!fulQuest.Rewards.ContainsKey(prof))
                                    fulQuest.Rewards.Add(prof, new QuestReward {Counter = itemCounter, ItemId = itemId});
                                continue;
                            }

                            rewardCounter++;
// ReSharper disable SpecifyACultureInStringConversionExplicitly
                            fulQuest.Rewards.Add(rewardCounter.ToString(), new QuestReward
// ReSharper restore SpecifyACultureInStringConversionExplicitly
                            {
                                ItemId = Convert.ToInt32(
                                    Regex.Match(rwdItm, "<a href=\"/item/([0-9]+)").Value.Replace(
                                    "<a href=\"/item/", "")),
                                    Counter = Convert.ToInt32(Regex.Match(rwdItm, "<b>([0-9]+)<b>").Value.Replace("<b>", ""))
                            });

                        }
                    }
                    else
                    {
                        steps.Add(new QStepNotImpliment { Description = match });
                    }

                }

                if (steps.Count == 1)
                {
                    foreach (IQuestStep questStep in steps)
                    {
                        if(questStep is QStepDialogWithNpc)
                        {
                            fulQuest.RewardNpc = ((QStepDialogWithNpc) questStep).NpcName[0];
                            steps.Remove(questStep);
                            break;
                        }
                    }
                }
                fulQuest.Steps = steps;
                Quests.Add(fulQuest.QuestId, fulQuest);
            }

            using (FileStream fs = File.Create("QuestTest.bin"))
                foreach (KeyValuePair<int, Quest> keyValuePair in Quests)
                    Serializer.SerializeWithLengthPrefix(fs, keyValuePair.Value, PrefixStyle.Fixed32);
        }*/
    }
}
