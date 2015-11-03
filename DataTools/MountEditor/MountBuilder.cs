using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data.Structures.World;
using ProtoBuf;

namespace DataTools.MountEditor
{
    class MountBuilder
    {
        public static string BinPath = Path.GetFullPath("../../../../datapack/gameserver/data/mounts.bin");
        public static List<Mount> Mounts = GetMounts();

        public static List<Mount> GetMounts()
        {
            var q = new List<Mount>();

            if (!Directory.Exists("data"))
                Directory.CreateDirectory("data");

            if (File.Exists(BinPath))
            {
                if (File.Exists("data/mounts.bin"))
                    File.Delete("data/mounts.bin");

                File.Copy(BinPath, "data/mounts.bin");
            }
            else
                File.Create("data/mounts.bin").Dispose();

            using (FileStream fileStream = File.OpenRead(Directory.GetCurrentDirectory() + "/data/mounts.bin"))
            {
                q = Serializer.Deserialize<List<Mount>>(fileStream).ToList();
            }
            return q;
        }

        public static void SaveMounts(IEnumerable<Mount> mounts)
        {
            if (File.Exists(Directory.GetCurrentDirectory() + "data/mounts.bin"))
                File.Delete(Directory.GetCurrentDirectory() + "data/mounts.bin");

            using (FileStream fileStream = File.Create(Directory.GetCurrentDirectory() + "/data/mounts.bin"))
            {
                Serializer.Serialize(fileStream, Mounts);
            }
        }
    }
}
