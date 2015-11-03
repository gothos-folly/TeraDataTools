using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Data.Structures.Template;
using System.Collections.Generic;

namespace DataTools.NpcEditor
{
    public partial class NpcEditor
    {
        public static void Start()
        {
            new NpcEditor().Show();
        }

        public NpcEditor()
        {
            InitializeComponent();
            //TeratomeListBox.DataContext = NpcBuilder.GetNpcs();
            TeratomeListBox.Items.SortDescriptions.Add(new SortDescription("Level", ListSortDirection.Ascending));

            FilterName.TextChanged += FilterChanged;
            FilterLevel.TextChanged += FilterChanged;

            TeratomeListBox.SelectionChanged += SelectionChanged;
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NpcTemplate npcTemplate = (NpcTemplate) TeratomeListBox.SelectedItem;

            if (npcTemplate == null)
                return;

            propertyGrid.SelectedObject = npcTemplate;
        }

        private void FilterChanged(object sender, TextChangedEventArgs e)
        {
            TeratomeListBox.Items.Filter += FilterByName;
        }

        private bool FilterByName(object obj)
        {
            NpcTemplate npcTemplate = (NpcTemplate) obj;

            if (FilterName.Text.Length > 0)
            {
                if (npcTemplate.Name.IndexOf(FilterName.Text, StringComparison.OrdinalIgnoreCase) == -1)
                    return false;
            }

            if (FilterLevel.Text.Length > 0)
            {
                bool textIsValid = true;

                for (int i = 0; i < FilterLevel.Text.Length; i++)
                    if (FilterLevel.Text[i] < '0' || FilterLevel.Text[i] > '9')
                    {
                        textIsValid = false;
                        break;
                    }

                if (textIsValid)
                {
                    int level = int.Parse(FilterLevel.Text);
                    if (npcTemplate.Level != level)
                        return false;
                }
            }

            return true;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            //NpcBuilder.SaveNpcs((IEnumerable<NpcTemplate>)TeratomeListBox.DataContext);
            Reload(null, null);
        }

        private void Reload(object sender, RoutedEventArgs e)
        {
            //TeratomeListBox.DataContext = NpcBuilder.GetNpcs();
        }

        private void New(object sender, RoutedEventArgs e)
        {
            List<NpcTemplate> npcs = (List<NpcTemplate>) TeratomeListBox.DataContext;

            if (npcs.Any(q => q.Name == "NewNpc" || q.FullId == 123456789))
            {
                MessageBox.Show(
                    "Вы уже добавили пустого NPC и не редактировали его!\nПрежде чем добавлять нового, заполните старого!",
                    "Внимание!");
                return;
            }
            //npcs.Add(new NpcTemplate { Name = "NewNpc", FullId = 123456789 });
            Reload(null, null);
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            List<NpcTemplate> npcs = (List<NpcTemplate>)TeratomeListBox.DataContext;

            npcs.Remove((NpcTemplate)TeratomeListBox.SelectedItem);
            Reload(null, null);
        }
    }
}
