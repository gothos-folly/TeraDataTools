using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Data.Structures.Creature;

namespace DataTools.StatsEditor
{
    public partial class StatsEditor
    {
        public static void Start()
        {
            new StatsEditor().Show();
        }

        public StatsEditor()
        {
            InitializeComponent();

            ListBox.ItemsSource = StatsBuilder.GetStats();

            ListBox.SelectionChanged += SelectionChanged;
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBox.SelectedItem == null)
                return;

            PropertyGrid.SelectedObject = ListBox.SelectedItem;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            StatsBuilder.Save((List<CreatureBaseStats>) ListBox.ItemsSource);
            Reload(null, null);
        }

        private void Reload(object sender, RoutedEventArgs e)
        {
            ListBox.ItemsSource = StatsBuilder.GetStats();
        }
    }
}
