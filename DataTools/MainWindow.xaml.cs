using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Controls;
using Image = System.Drawing.Image;

namespace DataTools
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            Utils.Init();
            InitializeComponent();
            
            AddButton("Build GEO", Geo.GeoBuilder.Build);
            AddButton("NpcEditor", NpcEditor.NpcEditor.Start);
            AddButton("SkillEditor", SkillEditor.SkillEditor.Start);
            AddButton("QuestEditor", QuestEditor.QuestEditor.Start);
            AddButton("RecipeEditor", RecipeEditor.RecipeEditor.Start);
            AddButton("StatsEditor", StatsEditor.StatsEditor.Start);
            AddButton("MountEditor", MountEditor.MountEditor.Start);

            AddLabel("DataCenter:");
            AddButton("Abnormality parser", Parsers.DC.AbnormalityParser.Parse);
            AddButton("Skills parser", Parsers.DC.SkillParser.Parse);
            AddButton("Npc parser", Parsers.DC.NpcParser.Parse);
            AddButton("Continent parser", Parsers.DC.ContinentParser.Parse);
            AddButton("PegasusPath parser", Parsers.DC.PegasusPathParser.Parse);
            AddButton("CooldownGroups parser", Parsers.DC.CooldownGroupsParser.Parse);
            AddButton("Collections parser", Parsers.DC.GatherParser.Parse);

            AddLabel("teratome.com:");
            AddButton("Drop parser", Parsers.Teratome.DropParser.Parse);
            AddButton("NPCs parser", Parsers.Teratome.NpcParser.Parse);
            AddButton("Items parser", Parsers.Teratome.ItemParser.Parse);
            AddButton("Skills parser", Parsers.Teratome.SkillParser.Parse);
            //AddButton("Quest parser", Parsers.Teratome.QuestParser.Parse);
            AddButton("Recipe parser", Parsers.Teratome.RecipeParser.Parse);

            AddLabel("teracodex.com:");
            AddButton("NPCs parser", Parsers.Teracodex.NpcParser.Parse);
            AddButton("Skills parser", Parsers.Teracodex.SkillParser.Parse);

            return;

            int size = 512;

            int minX = 38;
            int maxX = 60;

            int minY = 33;
            int maxY = 59;

            string[] imagesFiles = Directory.GetFiles(@"J:\Tera dev\t-emu\trunk\Servers\datapack\img");
            foreach (var imageFile in imagesFiles)
            {
                string strCoords = Path.GetFileNameWithoutExtension(imageFile).Replace("_SL", "").Replace("_Diff", "");
                strCoords = strCoords.Substring(strCoords.LastIndexOf("_", System.StringComparison.Ordinal) + 1);

                int x = int.Parse(strCoords.Substring(0, 2));
                int y = int.Parse(strCoords.Substring(2, 2));

                if (x < minX) minX = x;
                if (x > maxX) maxX = x;
                if (y < minY) minY = y;
                if (y > maxY) maxY = y;
            }

            using (Bitmap bmp = new Bitmap((maxX - minX + 1) * 512, (maxY - minY + 1) * 512))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    foreach (var imageFile in imagesFiles)
                    {
                        string strCoords = Path.GetFileNameWithoutExtension(imageFile).Replace("_SL", "").Replace("_Diff", "");
                        strCoords = strCoords.Substring(strCoords.LastIndexOf("_", System.StringComparison.Ordinal) + 1);

                        int x = int.Parse(strCoords.Substring(0, 2));
                        int y = int.Parse(strCoords.Substring(2, 2));

                        Image i = Paloma.TargaImage.LoadTargaImage(imageFile);
                        i.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        g.DrawImage(i, (x - minX) * 512, (y - minY) * 512);
                    }
                }

                bmp.Save(@"J:\Tera dev\t-emu\trunk\Servers\datapack\map_" + minX + minY + ".jpg", ImageFormat.Jpeg);
            }
        }

        private void AddLabel(string text)
        {
            Panel.Children.Add(new Label {Content = text});
        }

        private void AddButton(string content, Action action)
        {
            Button btn = new Button {Content = content};
            btn.Click += (sender, args) =>
                             {
                                 Hide();
                                 action.Invoke();
                                 Show();
                             };

            Panel.Children.Add(btn);
        }
    }
}
