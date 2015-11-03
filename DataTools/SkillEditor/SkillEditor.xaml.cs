using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Data.Structures.SkillEngine;

namespace DataTools.SkillEditor
{
    public partial class SkillEditor
    {
        public static void Start()
        {
            new SkillEditor().Show();
        }

        public SkillEditor()
        {
            InitializeComponent();

            //SkillBuilder.BuildAll();

            ListBox.ItemsSource = Data.Data.Skills.Values;

            FilterName.TextChanged += FilterChanged;

            ListBox.SelectionChanged += SelectionChanged;
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBox.SelectedItem == null)
                return;

            PropertyGrid.SelectedObject = ListBox.SelectedItem;
        }

        private void FilterChanged(object sender, TextChangedEventArgs e)
        {
            ListBox.Items.Filter += FilterByName;
        }

        private bool FilterByName(object obj)
        {
            Skill skill = (Skill) obj;

            if (FilterName.Text.Length > 0)
            {
                if (skill.Name.IndexOf(FilterName.Text, StringComparison.OrdinalIgnoreCase) == -1)
                    return false;
            }

            return true;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            //SkillBuilder.SaveData((IEnumerable<Skill>) ListBox.ItemsSource);
            Reload(null, null);
        }

        private void Reload(object sender, RoutedEventArgs e)
        {
            Data.Data.LoadSkills();
            ListBox.ItemsSource = Data.Data.Skills.Values;
        }
    }
}
