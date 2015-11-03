using System;
using System.Collections.Generic;
using System.IO;
using Data.Structures.SkillEngine;
using ProtoBuf;

namespace DataTools.SkillEditor
{
    class SkillBuilder
    {
        /*public static void BuildAll()
        {
            if (!Directory.Exists("data"))
                Directory.CreateDirectory("data");

            string binPath = Path.GetFullPath("../../../../datapack/gameserver/data/skills.bin");
            if (File.Exists(binPath))
            {
                if (File.Exists("data/skills.bin"))
                    File.Delete("data/skills.bin");
                File.Copy(binPath, "data/skills.bin");
            }
            else
                File.Create("data/skills.bin").Dispose();

            Data.Data.LoadSkills();

            List<Skill> skills = new List<Skill>(Data.Data.Skills.Values);
            List<Structures.Teratome.Skill> teratomeSkills = Parsers.Teratome.SkillParser.GetSkills();
            List<Structures.Teracodex.Skill> teracodexSkills = Parsers.Teracodex.SkillParser.GetSkills();

            foreach (var teratomeSkill in teratomeSkills)
            {
                Skill skill = FindExist(skills, teratomeSkill.ToString());

                if (skill == null)
                {
                    skill = new Skill();
                    skills.Add(skill);
                }

                skill.Id = teratomeSkill.Id;
                skill.Name = teratomeSkill.Name;
                skill.PlayerClass = teratomeSkill.PlayerClass;
                skill.Level = teratomeSkill.Level;
                skill.Cost = teratomeSkill.Cost;
                skill.MpCost = teratomeSkill.MpCost;
                skill.HpCost = teratomeSkill.HpCost;
                skill.Cooldown = teratomeSkill.Cooldown;
                skill.Description = teratomeSkill.Description;

                if (skill.Power == null || skill.Power.Count < 1)
                    skill.Power = new List<int> {0};
            }

            foreach (var teracodexSkill in teracodexSkills)
            {
                Skill skill = FindExist(skills, teracodexSkill.ToString());
                if (skill == null)
                    continue;

                skill.CastTime = teracodexSkill.CastTime;
                
                if (skill.Power == null || skill.Power.Count < 1 || skill.Power[0] == 0)
                {
                    skill.Power = new List<int>();
                    string[] powers = teracodexSkill.Power.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string power in powers)
                        skill.Power.Add(int.Parse(power));

                    if (skill.Power.Count < 1)
                        skill.Power.Add(0);
                }
            }

            SaveData(skills);
            Data.Data.LoadSkills();
        }

        public static void SaveData(IEnumerable<Skill> skills)
        {
            string binPath = Path.GetFullPath("../../../../datapack/gameserver/data/skills.bin");

            using (FileStream fs = File.Create(binPath))
            {
                foreach (var skill in skills)
                {
                    Serializer.SerializeWithLengthPrefix(fs, skill, PrefixStyle.Fixed32);
                }
            }

            File.Delete("data/skills.bin");
            File.Copy(binPath, "data/skills.bin");
        }

        private static Skill FindExist(IEnumerable<Skill> skills, string skillName)
        {
            foreach (Skill skill in skills)
                if (skill.ToString().Equals(skillName, StringComparison.OrdinalIgnoreCase))
                    return skill;

            return null;
        }*/
    }
}
