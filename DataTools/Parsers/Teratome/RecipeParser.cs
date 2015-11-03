using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Data.Enums.Craft;
using Data.Structures.Craft;
using ProtoBuf;

namespace DataTools.Parsers.Teratome
{
    internal class RecipeParser
    {
        public static Dictionary<int, Recipe> Recipes = new Dictionary<int, Recipe>();

        public static void Parse()
        {
            const string url = "http://www.teratome.com/recipes/page/";

            for (int i = 1; i < 37; i++)
            {
                string page = Utils.LoadPage(url + i);

                string mainstr = Regex.Match(page, "<tbody>(.*)</tbody>").Value;

                string[] valss = mainstr.Replace("<tr ", "~").Split('~');

                for (int j = 0; j < valss.Count(); j++)
                {

                    string[] vals = valss[j].Replace("<td", "~").Split('~');

                    if (vals.Count() <= 1)
                        continue;

                    Recipe recipe = new Recipe();

                    recipe.RecipeId =
                        Convert.ToInt32(
                            Regex.Match(vals[0], "data-href=\"/recipe/([0-9]+)").Value.Replace("data-href=\"/recipe/",
                                                                                               ""));
                    recipe.Name =
                        Regex.Match(vals[1], "class=\"cleartext\">([^<]+)</a></span></td>").
                            Value.Replace("class=\"cleartext\">", "").Replace("</a></span></td>", "");
                    recipe.CraftStat =
                        (CraftStat)
                        Enum.Parse((typeof (CraftStat)),
                                   Regex.Match(vals[2],
                                               "class=\"cleartext\">([a-zA-Z]+)</a></td>").Value
                                       .Replace("class=\"cleartext\">", "").Replace(
                                           "</a></td>", ""));
                    recipe.ReqMin =
                        Convert.ToInt16(
                            Regex.Match(vals[3], "class=\"nobr\">([0-9]+)").Value.Replace(
                                "class=\"nobr\">", ""));
                    recipe.ReqMax =
                        Convert.ToInt16(
                            Regex.Match(vals[3], "([0-9]+)</td>").Value.Replace("</td>", ""));

                    MatchCollection m = Regex.Matches(vals[4], "href=\"/item/([0-9]+)/([^<]+)<b>([0-9]+)<b>");

                    Dictionary<int, int> neededItems = new Dictionary<int, int>();

                    foreach (Match match in m)
                        neededItems.Add(
                            Convert.ToInt32(
                                Regex.Match(match.Value, "href=\"/item/([0-9]+)").Value.Replace("href=\"/item/", "")),
                            Convert.ToInt32(Regex.Match(match.Value, "<b>([0-9]+)<b>").Value.Replace("<b>", "")));

                    recipe.NeededItems = neededItems;

                    if (!vals[5].Contains("class=\"nobr\"></td></tr>"))
                    {
                        recipe.CriticalResultItem =
                            new KeyValuePair<int, int>(
                                Convert.ToInt32(Regex.Match(vals[5], "href=\"/item/([0-9]+)").Value.Replace(
                                    "href=\"/item/", "")),
                                Convert.ToInt32(Regex.Match(vals[5], "<b>([0-9]+)<b>").Value.Replace("<b>", "")));

                        recipe.CriticalChancePercent =
                            Convert.ToByte(
                                Regex.Match(vals[5], "<b>([0-9]+)%</b></td>").Value.Replace("</b>", "").Replace(
                                    "<b>",
                                    "").
                                    Replace("</td>", "").Replace("%", ""));
                    }

                    if (!Recipes.ContainsKey(recipe.RecipeId))
                        Recipes.Add(recipe.RecipeId, recipe);
                }
            }

            foreach (KeyValuePair<int, Recipe> keyValuePair in Recipes)
            {
                string recipePage = Utils.LoadPage("http://www.teratome.com/recipe/" + keyValuePair.Key);
                string full =
                    Regex.Match(recipePage,
                                "Creates: <div class=([^>]+)><i></i><ins style=([^>]+)></ins><del></del><a href=\"/item/([0-9]+)/([^<]+)<b>([0-9]+)<b>")
                        .Value;

                if (full == "")
                    keyValuePair.Value.ResultItem = new KeyValuePair<int, int>(125, 1);
                else
                    keyValuePair.Value.ResultItem =
                        new KeyValuePair<int, int>(
                            Convert.ToInt32(
                                Regex.Match(full, "<a href=\"/item/([0-9]+)").Value.Replace("<a href=\"/item/",
                                                                                            "")),
                            Convert.ToInt32(Regex.Match(full, "<b>([0-9]+)<b>").Value.Replace("<b>", "")));
            }

            using (FileStream fileStream = File.Create("data/recipes.bin"))
            {
                foreach (KeyValuePair<int, Recipe> keyValuePair in Recipes)
                {
                    Serializer.SerializeWithLengthPrefix(fileStream, keyValuePair.Value, PrefixStyle.Fixed32);
                }
            }
        }
    }
}
