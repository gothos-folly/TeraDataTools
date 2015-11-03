using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Structures.Craft;
using ProtoBuf;

namespace DataTools.RecipeEditor
{
    class RecipeBuilder
    {
        public static string BinPath = Path.GetFullPath("../../../../datapack/gameserver/data/recipes.bin");
        public static List<Recipe> Recipes = GetRecipes();

        private static List<Recipe> GetRecipes()
        {
            var q = new List<Recipe>();

            if (!Directory.Exists("data"))
                Directory.CreateDirectory("data");

            if (File.Exists(BinPath))
            {
                if (File.Exists("data/recipes.bin"))
                    File.Delete("data/recipes.bin");

                File.Copy(BinPath, "data/recipes.bin");
            }
            else
                File.Create("data/recipes.bin").Dispose();

            using (FileStream fileStream = File.OpenRead(Directory.GetCurrentDirectory() + "/data/recipes.bin"))
            {
                while (fileStream.Position < fileStream.Length)
                    q.Add(Serializer.DeserializeWithLengthPrefix<Recipe>(fileStream, PrefixStyle.Fixed32));
            }
            return q;
        }

        public static void SaveRecipes(IEnumerable<Recipe> quests)
        {
            if (File.Exists(Directory.GetCurrentDirectory() + "data/recipes.bin"))
                File.Delete(Directory.GetCurrentDirectory() + "data/recipes.bin");

            using (FileStream fileStream = File.Create(Directory.GetCurrentDirectory() + "/data/recipes.bin"))
            {
                foreach (Recipe quest in quests)
                    Serializer.SerializeWithLengthPrefix(fileStream, quest, PrefixStyle.Fixed32);
            }
        }
    }
}
