using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Data.Structures.World;

namespace DataTools.MountEditor
{
    /// <summary>
    /// Логика взаимодействия для MountEditor.xaml
    /// </summary>
    public partial class MountEditor
    {

        public static void Start()
        {
            new MountEditor().Show();
        }

        public MountEditor()
        {
            InitializeComponent();

            MountBuilder.Mounts = MountBuilder.GetMounts();
            MListBox.DataContext = MountBuilder.Mounts;
            MListBox.Items.SortDescriptions.Add(new SortDescription("MountId", ListSortDirection.Ascending));

            FilterName.TextChanged += FilterChanged;
            FilterLevel.TextChanged += FilterChanged;

            MListBox.SelectionChanged += SelectionChanged;
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Mount mount = (Mount) MListBox.SelectedItem;

            if (mount == null)
                return;

            propertyGrid.SelectedObject = mount;
        }

        private void FilterChanged(object sender, TextChangedEventArgs e)
        {
            MListBox.Items.Filter += FilterByName;
        }

        private bool FilterByName(object obj)
        {
            return true;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            MountBuilder.SaveMounts((IEnumerable<Mount>) MListBox.DataContext);
            Reload(null, null);
        }

        private void Reload(object sender, RoutedEventArgs e)
        {
            MListBox.DataContext = MountBuilder.Mounts.ToList();
        }

        private void New(object sender, RoutedEventArgs e)
        {

            MountBuilder.Mounts.Add(new Mount {MountId = 123456789, Name = "NewMount"});
            Reload(null, null);
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            MountBuilder.Mounts.Remove((Mount) MListBox.SelectedItem);
            Reload(null, null);
        }
    }
}
