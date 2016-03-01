using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DCTools;
using DCTools.Structures;
using Data.Enums;
using Newtonsoft.Json;

namespace DataTools.Parsers.DC
{
    internal class DamageMeterNpc
    {
        public int huntingZoneId { get; set; }
        public int templateId { get; set; }

        public string name { get; set; }
    }

    class DamageMeterExporter
    {
        private static void ExportNpcs()
        {
            var dc = DCT.DataCenter;
            var strings = dc.Root.ChildrenByName("StrSheet_Creature")
                            .SelectMany(x => x.Children)
                            .SelectMany(
                                hz =>
                                hz.ChildrenByName("String").Select(
                                    str =>
                                    new
                                    {
                                        huntingZoneId = hz.Attribute<int>("id"),
                                        templateId = str.Attribute<int>("templateId"),
                                        String = str
                                    }))
                            .ToDictionary(x => x.huntingZoneId + "-" + x.templateId, x => x.String);
            var npcs = dc.Root.ChildrenByName("NpcData")
                        .SelectMany(x => x.Children)
                        .SelectMany(hz => hz.ChildrenByName("Template").Select(template => new
                        {
                            huntingZoneId = hz.Attribute("id"),
                            templateId = template.Attribute("id"),
                            Template = template
                        }));

            foreach (var npcData in npcs)
            {
                var stringData = strings[npcData.huntingZoneId + "-" + npcData.templateId];
                var templateData = npcData.Template;
                var npc = new DamageMeterNpc();
                npc.name = stringData.Attribute<string>("name");
                //npc.
            }
        }

        private static string NullCommon(object o)
        {
            var s = o.ToString();
            if (string.IsNullOrEmpty(s) || s == "Common")
                return null;
            return s;
        }

        private static void ExportSkills()
        {
            var dc = DCT.DataCenter;
            var dcObjects = dc.Root.ChildrenByName("StrSheet_UserSkill").SelectMany(x => x.ChildrenByName("String"));

            var skills = dcObjects
                .Select(data => new
                    {
                        id = NullCommon(data.Attribute<int>("id")),
                        race = NullCommon(data.Attribute<string>("race")),
                        gender = NullCommon(data.Attribute<string>("gender")),
                        @class = NullCommon(PlayerClassHelper.Parse(data.Attribute<string>("class"))),
                        name = NullCommon(data.Attribute<string>("name"))
                    })
                .Where(x => !string.IsNullOrEmpty(x.name))
                .OrderBy(x => x.@class)
                .ThenBy(x => x.id);

            var lines = skills.Select(o => JsonConvert.SerializeObject(o, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            File.WriteAllLines(Utils.GetOutput("user_skills.json"), lines);
        }

        public static void Export()
        {
            ExportSkills();
        }
    }
}
