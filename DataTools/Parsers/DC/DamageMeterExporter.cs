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

        private static void ExportSkills()
        {
            var dc = DCT.DataCenter;
            var dcObjects = dc.Root.ChildrenByName("StrSheet_UserSkill").SelectMany(x => x.ChildrenByName("String"));

            var skills = dcObjects
                .Select(data => new
                    {
                        Id = data.Attribute<int>("id"),
                        Race = data.Attribute<string>("race"),
                        Gender = data.Attribute<string>("gender"),
                        Class = PlayerClassHelper.Parse(data.Attribute<string>("class")),
                        Name = data.Attribute<string>("name")
                    })
                .Where(x => !string.IsNullOrEmpty(x.Name))
                .OrderBy(x => x.Class.ToString())
                .ThenBy(x => x.Id);

            var lines = skills.Select(x => string.Join(" ", x.Id, x.Race, x.Gender, x.Class, x.Name));
            File.WriteAllLines(Utils.GetOutput("user_skills.txt"), lines);
        }

        public static void Export()
        {
            ExportSkills();
        }
    }
}
