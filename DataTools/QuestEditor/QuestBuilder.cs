using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data.Structures.Quest;
using ProtoBuf;

namespace DataTools.QuestEditor
{

    internal class QuestBuilder
    {
        public static string BinPath = Path.GetFullPath("../../../../datapack/gameserver/data/quests.bin");
        public static List<Quest> Quests; 

        public static List<Quest> GetQuests()
        {
            var q = new List<Quest>();

            if (!Directory.Exists("data"))
                Directory.CreateDirectory("data");

            if (File.Exists(BinPath))
            {
                if (File.Exists("data/quests.bin"))
                    File.Delete("data/quests.bin");

                File.Copy(BinPath, "data/quests.bin");
            }
            else
                File.Create("data/quests.bin").Dispose();

            using (FileStream fileStream = File.OpenRead(Directory.GetCurrentDirectory() + "/data/quests.bin"))
            {
                q = Serializer.Deserialize<Dictionary<int, Quest>>(fileStream).Values.ToList();
            }
            return q;
        }

        public static void SaveQuests(IEnumerable<Quest> quests)
        {
            if (File.Exists(Directory.GetCurrentDirectory() + "data/quests.bin"))
                File.Delete(Directory.GetCurrentDirectory() + "data/quests.bin");

            using (FileStream fileStream = File.Create(Directory.GetCurrentDirectory() + "/data/quests.bin"))
            {
                foreach (Quest quest in quests)
                    Serializer.SerializeWithLengthPrefix(fileStream, quest, PrefixStyle.Fixed32);
            }
        }
    }
}
