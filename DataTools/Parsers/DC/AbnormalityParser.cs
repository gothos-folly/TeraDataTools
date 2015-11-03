using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DCTools;
using DCTools.Structures;
using Data.Enums.SkillEngine;
using Data.Structures.SkillEngine;
using ProtoBuf;

namespace DataTools.Parsers.DC
{
    class AbnormalityParser
    {
        public static Dictionary<string, List<int>> ByNames;

        public static Dictionary<int, KeyValuePair<string, string>> Names;

        public static Dictionary<int, Abnormality> Abnormalities;

        public static DataCenter DC = DCT.GetDataCenter();

        public static void Parse()
        {
            ParseNames();

            List<string> names = ByNames.Keys.ToList();
            names.Sort();

            using (TextWriter writer = new StreamWriter("../../../../datapack/abnormalities.txt"))
            {
                foreach (string name in names)
                {
                    writer.WriteLine();
                    writer.WriteLine("//{0}", name);

                    foreach (int id in ByNames[name])
                        writer.WriteLine("case {0}: //{1}", id, Names[id].Value);

                    writer.WriteLine("  break;");
                }
            }

            ParseAbnormality();

            string binPath = Path.GetFullPath("../../../../datapack/gameserver/data/");

            using (FileStream fs = File.Create(binPath + "abnormalities.bin"))
            {
                Serializer.Serialize(fs, Abnormalities);
            }
        }

        private static void ParseNames()
        {
            ByNames = new Dictionary<string, List<int>>();

            Names = new Dictionary<int, KeyValuePair<string, string>>();

            foreach (var o in DC.GetMainObjectsByName("StrSheet_Abnormality"))
            {
                foreach (var data in (List<Dictionary<string, object>>) DC.GetValues(o)["String"])
                {
                    int id = int.Parse(data["id"].ToString());
                    
                    string name = data["name"].ToString();
                    string tooltip = data["tooltip"].ToString();

                    Names.Add(id, new KeyValuePair<string, string>(name, tooltip));

                    tooltip = tooltip.Replace("$BR", " ");

                    tooltip = Regex.Replace(tooltip, "\\$H_W_GOOD\\${0,1}[^\\$]+\\$COLOR_END", "[VAL]", RegexOptions.IgnoreCase);
                    tooltip = Regex.Replace(tooltip, "\\$H_W_BAD\\${0,1}[^\\$]+\\$COLOR_END", "[VAL]", RegexOptions.IgnoreCase);

                    if (!ByNames.ContainsKey(tooltip))
                        ByNames.Add(tooltip, new List<int>());

                    ByNames[tooltip].Add(id);
                }
            }
        }

        private static void ParseAbnormality()
        {
            Abnormalities = new Dictionary<int, Abnormality>();

            foreach (var o in DC.GetMainObjectsByName("Abnormality"))
            {
                foreach (var data in (List<Dictionary<string, object>>) DC.GetValues(o)["Abnormal"])
                {
                    Abnormality abnormality = new Abnormality();

                    abnormality.Id = int.Parse(data["id"].ToString());

                    abnormality.Infinity = bool.Parse(data["infinity"].ToString());

                    abnormality.IsBuff = bool.Parse(data["isBuff"].ToString());

                    abnormality.Kind = int.Parse(data["kind"].ToString());

                    abnormality.Level = int.Parse(data["level"].ToString());

                    abnormality.NotCareDeath = bool.Parse(data["notCareDeath"].ToString());

                    abnormality.Priority = int.Parse(data["priority"].ToString());

                    abnormality.Property = int.Parse(data["property"].ToString());

                    abnormality.Time = int.Parse(data["time"].ToString());

                    abnormality.IsShow = ParseEnum<AbnormalityShowType>(data["isShow"]);

                    abnormality.IsHideOnRefresh = bool.Parse(data["isHideOnRefresh"].ToString());

                    abnormality.BySkillCategory = int.Parse(data["bySkillCategory"].ToString());

                    if (data.ContainsKey("AbnormalityEffect"))
                    {
                        foreach (var effectData in (List<Dictionary<string, object>>) data["AbnormalityEffect"])
                        {
                            abnormality.Effects.Add(ParseAbnormalityEffect(effectData));
                        }
                    }

                    if (Names.ContainsKey(abnormality.Id))
                    {
                        abnormality.Name = Names[abnormality.Id].Key;
                        abnormality.Tooltip = Names[abnormality.Id].Value;
                    }
                    else
                    {
                        abnormality.Name = "Unk";
                        abnormality.Tooltip = "Unk";
                    }

                    Abnormalities.Add(abnormality.Id, abnormality);
                }
            }

            Console.WriteLine("Loaded {0} abnormalities...", Abnormalities.Count);
        }

        private static AbnormalityEffect ParseAbnormalityEffect(Dictionary<string, object> data)
        {
            AbnormalityEffect effect = new AbnormalityEffect();

            effect.AppearEffectId = int.Parse(data["appearEffectId"].ToString());

            effect.AttackEffectId = int.Parse(data["attackEffectId"].ToString());

            effect.DamageEffectId = int.Parse(data["damageEffectId"].ToString());
            
            effect.DisappearEffectId = int.Parse(data["disappearEffectId"].ToString());

            effect.EffectId = int.Parse(data["effectId"].ToString());

            effect.EffectPart = data["effectPart"].ToString();

            effect.Method = int.Parse(data["method"].ToString());

            effect.TickInterval = int.Parse(data["tickInterval"].ToString());

            effect.Type = int.Parse(data["type"].ToString());

            effect.Value = float.Parse(data["value"].ToString());

            effect.OverlayEffectId = int.Parse(data["overlayEffectId"].ToString());

            return effect;
        }

        private static T ParseEnum<T>(object o)
        {
            return (T)Enum.Parse(typeof(T), Regex.Replace(o.ToString(), "[^a-z0-9]", "", RegexOptions.IgnoreCase), true);
        }
    }
}
