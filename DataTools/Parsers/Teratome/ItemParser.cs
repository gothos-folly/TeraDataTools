using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Data.Enums;
using DataTools.Structures.Teratome;
using ProtoBuf;

namespace DataTools.Parsers.Teratome
{
    class ItemParser
    {
        public static List<Item> GetItems()
        {
            if (!File.Exists(Utils.DataDirectory + "/teratome/item_data.bin"))
                Parse();

            List<Item> items = new List<Item>();

            using (FileStream fileStream = File.OpenRead(Utils.DataDirectory + "/teratome/item_data.bin"))
            {
                while (fileStream.Position < fileStream.Length)
                {
                    items.Add(Serializer.DeserializeWithLengthPrefix<Item>(fileStream, PrefixStyle.Fixed32));
                }
            }

            return items;
        }

        public static void Parse()
        {
            if (!Directory.Exists(Utils.DataDirectory + "/teratome"))
                Directory.CreateDirectory(Utils.DataDirectory + "/teratome");

            using (FileStream fs = File.Create(Utils.DataDirectory + "/teratome/item_data.bin"))
            {
                for (int i = 0; i < 282; i++)
                {
                    string html = Utils.LoadPage("http://www.teratome.com/items/page/" + (i + 1));

                    MatchCollection collection = Regex.Matches(html, "<tr data-href=\"/item/([0-9]+)[^>]*><td[^>]*><div[^>]*q([0-9]+)\"><i></i><ins[^>]*></ins><del></del><a[^>]*></a></div><span><a[^>]*>([^<]+)</a></span></td><td[^>]*>([0-9]+)</td><td[^>]*>(?:[0-9]+)*</td><td[^>]*>((?:(?<!</td).)*)</td><td[^>]*><a[^>]*>([^<]+)</a></td></tr>", RegexOptions.IgnoreCase);

                    foreach (Match match in collection)
                    {
                        Item item = new Item
                                        {
                                            Id = int.Parse(match.Groups[1].Value),
                                            Name = match.Groups[3].Value,
                                            Quality = (ItemQuality) int.Parse(match.Groups[2].Value),
                                            Level = int.Parse(match.Groups[4].Value),
                                            Category = match.Groups[6].Value,
                                        };

                        string classesHtml = match.Groups[5].Value;

                        MatchCollection classesCollection = Regex.Matches(classesHtml, "<div[^>]*><i></i><ins[^>]*></ins><del></del><a href=\"/class/[0-9]+/([a-z]+)\"></a></div>", RegexOptions.IgnoreCase);

                        foreach (Match matchClass in classesCollection)
                        {
                            PlayerClass playerClass = (PlayerClass) Enum.Parse(typeof (PlayerClass), matchClass.Groups[1].Value, true);
                            item.Classes.Add(playerClass);
                        }

                        string tooltip = Utils.LoadPage(@"http://www.teratome.com/item/" + item.Id + "/tooltips");

                        Serializer.SerializeWithLengthPrefix(fs, item, PrefixStyle.Fixed32);
                    }

                    Console.WriteLine("Finded {0} items...", collection.Count);
                }
            }
        }
    }
}
