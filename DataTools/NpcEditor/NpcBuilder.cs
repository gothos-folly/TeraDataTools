using System.Collections.Generic;
using System.IO;
using Data.Structures.Template;
using ProtoBuf;

namespace DataTools.NpcEditor
{
    class NpcBuilder
    {
        /*public static string BinPath = Path.GetFullPath("../../../../datapack/gameserver/data/npc_templates.bin");

        public static List<NpcTemplate> GetNpcs()
        {
            Dictionary<int, SpawnTemplate> spawnTemplates = new Dictionary<int, SpawnTemplate>();
            foreach (string fileName in Directory.GetFiles(Path.GetFullPath("../../../../datapack/gameserver/data/spawn")))
            {
                if (fileName.Contains("_spawn.bin"))
                {
                    using (FileStream stream = File.OpenRead(fileName))
                    {
                        while (stream.Position < stream.Length)
                        {
                            SpawnTemplate spawnTemplate = Serializer.DeserializeWithLengthPrefix<SpawnTemplate>(stream,PrefixStyle.Fixed32);
                            int fullId = spawnTemplate.NpcId + spawnTemplate.Type*100000;

                            if (!spawnTemplates.ContainsKey(fullId))
                                spawnTemplates.Add(fullId, spawnTemplate);
                        }
                    }
                }
            }

            if (!Directory.Exists("data"))
                Directory.CreateDirectory("data");

            if (File.Exists(BinPath))
            {
                if (File.Exists("data/npc_templates.bin"))
                    File.Delete("data/npc_templates.bin");

                File.Copy(BinPath, "data/npc_templates.bin");
            }
            else
                File.Create("data/npc_templates.bin").Dispose();

            Data.Data.LoadNpcTemplates();

            foreach (var npc in Parsers.Teratome.NpcParser.GetNpcs())
            {
                if (Data.Data.NpcTemplates.ContainsKey(npc.FullId))
                    continue;

                int id = npc.FullId%100000;
                int type = npc.FullId/100000;

                if (spawnTemplates.ContainsKey(npc.FullId))
                {
                    id = spawnTemplates[npc.FullId].NpcId;
                    type = spawnTemplates[npc.FullId].Type;
                }

                Data.Data.NpcTemplates.Add(
                    npc.FullId,
                    new NpcTemplate
                        {
                            Name = npc.Name,
                            Id = id,
                            Level = npc.Level,
                            Drop = npc.Drop,
                        });
            }

            foreach (var npc in Parsers.Teracodex.NpcParser.GetNpcs())
            {
                NpcTemplate template = null;
                foreach (var npcTemplate in Data.Data.NpcTemplates)
                {
                    if (npcTemplate.Value.Name.Equals(npc.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        template = npcTemplate.Value;
                        break;
                    }
                }

                if (template == null)
                    continue;

                //template.Drop = npc.Drop;
            }

            foreach (var spawnTemplate in spawnTemplates.Values)
            {
                int fullId = spawnTemplate.NpcId + spawnTemplate.Type*100000;

                if (!Data.Data.NpcTemplates.ContainsKey(fullId))
                {
                    Data.Data.NpcTemplates.Add(fullId, new NpcTemplate
                                                           {
                                                               FullId = fullId,
                                                               Name = "Unknown",
                                                               Id = spawnTemplate.NpcId,
                                                               Type = spawnTemplate.Type,
                                                               Level = 0,
                                                           });
                }
            }

            return new List<NpcTemplate>(Data.Data.NpcTemplates.Values);
        }

        public static void SaveNpcs(IEnumerable<NpcTemplate> npcs)
        {
            using (FileStream fileStream = File.Create(BinPath))
            {
                foreach (NpcTemplate npcTemplate in npcs)
                    Serializer.SerializeWithLengthPrefix(fileStream, npcTemplate, PrefixStyle.Fixed32);
            }

            File.Delete("data/npc_templates.bin");
            File.Copy(BinPath, "data/npc_templates.bin");
        }*/
    }
}
