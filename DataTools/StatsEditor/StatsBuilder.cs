using System;
using System.Collections.Generic;
using System.IO;
using Data.Enums;
using Data.Structures.Creature;
using Data.Structures.Template.Item;
using Data.Structures.Template.Item.CategorieStats;
using ProtoBuf;

namespace DataTools.StatsEditor
{
    class StatsBuilder
    {
        public static string BinPath = Path.GetFullPath("../../../../datapack/gameserver/data/stats.bin");

        public static List<CreatureBaseStats> GetStats()
        {
            List<CreatureBaseStats> stats = new List<CreatureBaseStats>();

            if (!File.Exists(BinPath))
                BuilStats(stats);
            else
            {
                using (FileStream fs = File.OpenRead(BinPath))
                {
                    stats = Serializer.Deserialize<List<CreatureBaseStats>>(fs);
                }

                stats.RemoveRange(8, stats.Count - 8); 
                AppendNpcStats(stats);
            }

            return stats;
        }

        public static void Save(List<CreatureBaseStats> stats)
        {
            using (FileStream fs = File.Create(BinPath))
            {
                Serializer.Serialize(fs, stats);
            }
        }

        //

        private static void BuilStats(List<CreatureBaseStats> stats)
        {
            for (int i = 0; i < 8; i++)
                stats.Add(new CreatureBaseStats {PlayerClass = (PlayerClass) i});

            AppendNpcStats(stats);
        }

        private static List<int> GetMaxByLevel(Func<ItemTemplate, int> func)
        {
            List<int> results = new List<int>();
            for (int i = 0; i < 100; i++)
                results.Add(1);

            foreach (ItemTemplate itemTemplate in Data.Data.ItemTemplates.Values)
            {
                int value = func(itemTemplate);

                for (int i = itemTemplate.RequiredLevel; i < 100; i++)
                    if (results[i] < value)
                        results[i] = value;
            }

            return results;
        }

        private static void AppendNpcStats(List<CreatureBaseStats> stats)
        {
            if (File.Exists("data/items.bin"))
                File.Delete("data/items.bin");
            File.Copy(Path.GetFullPath("../../../../datapack/gameserver/data/npc_templates.bin"), "data/items.bin");

            Data.Data.LoadItemTemplates();

            List<int> maxDefense = GetMaxByLevel(
                t =>
                    {
                        EquipmentStat equipmentStat = t.CatigorieStat as EquipmentStat;
                        if (equipmentStat != null)
                            return equipmentStat.Def;
                        return -1;
                    });

            List<int> maxImpact = GetMaxByLevel(
                t =>
                {
                    EquipmentStat equipmentStat = t.CatigorieStat as EquipmentStat;
                    if (equipmentStat != null)
                        return equipmentStat.Impact;
                    return -1;
                });

            List<int> maxBalance = GetMaxByLevel(
                t =>
                {
                    EquipmentStat equipmentStat = t.CatigorieStat as EquipmentStat;
                    if (equipmentStat != null)
                        return equipmentStat.Balance;
                    return -1;
                });

            if (File.Exists("data/npc_templates.bin"))
                File.Delete("data/npc_templates.bin");
            File.Copy(Path.GetFullPath("../../../../datapack/gameserver/data/npc_templates.bin"), "data/npc_templates.bin");

            Data.Data.LoadNpcTemplates();

            int critChanse = 0,
                critResist = 0,
                power = 0,
                endurance = 0,
                impactFactor = 0,
                balanceFactor = 0,
                resist = 0;

            for (int i = 0; i < 8; i++)
            {
                critChanse += stats[i].CritChanse;
                critResist += stats[i].CritResist;
                power += stats[i].Power;
                endurance += stats[i].Endurance;
                impactFactor += stats[i].ImpactFactor;
                balanceFactor += stats[i].BalanceFactor;
                resist += stats[i].WeakeningResist;
            }

            critChanse /= 8;
            critResist /= 8;
            power /= 8;
            endurance /= 8;
            impactFactor /= 8;
            balanceFactor /= 8;
            resist /= 8;

            foreach (var f1 in Data.Data.NpcTemplates.Values)
            {
                foreach (var template in f1.Values)
                {
                    stats.Add(
                        new CreatureBaseStats
                            {
                                NpcHuntingZoneId = template.HuntingZoneId,
                                NpcId = template.Id,
                                NpcName = template.Name,
                                Level = template.Level,

                                HpBase = (int) (1000*(1 + template.Level*0.33)
                                                *(template.Size == NpcSize.Small ? 0.26 : 1)
                                                *(template.Size == NpcSize.Large ? 1.41 : 1)
                                                *(template.IsFreeNamed ? 1.71 : 1)
                                                *(template.IsElite ? 2.121 : 1)
                                                *(template.IsVillager ? 50 : 1)
                                               ),

                                Attack = 1,

                                Defense = (int) (maxDefense[template.Level] * 0.79
                                                 *(template.Size == NpcSize.Small ? 0.59 : 1)
                                                ),

                                Impact = maxImpact[template.Level]*2,
                                Balance = maxBalance[template.Level]*2,
                                CritChanse = critChanse,
                                CritResist = critResist,
                                CritPower = 2,

                                Power = power,
                                Endurance = endurance,
                                ImpactFactor = impactFactor,
                                BalanceFactor = balanceFactor,
                                AttackSpeed = 100,
                                Movement = (short) template.Shape.WalkSpeed,

                                WeakeningResist = resist,
                                PeriodicResist = resist,
                                StunResist = resist,
                            });
                }
            }
        }
    }
}
