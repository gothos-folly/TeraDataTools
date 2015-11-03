using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Data.Structures.Craft;

namespace DataTools.RecipeEditor
{
    /// <summary>
    /// Interaction logic for RecipeEditor.xaml
    /// </summary>
    public partial class RecipeEditor
    {
        public static void Start()
        {
            new RecipeEditor().Show();
        }
        public RecipeEditor()
        {
            InitializeComponent();

            TeratomeListBox.DataContext = RecipeBuilder.Recipes;
            TeratomeListBox.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            FilterName.TextChanged += FilterChanged;

            TeratomeListBox.SelectionChanged += SelectionChanged;
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Recipe quest = (Recipe)TeratomeListBox.SelectedItem;

            if (quest == null)
                return;

            propertyGrid.SelectedObject = quest;
        }

        private void FilterChanged(object sender, TextChangedEventArgs e)
        {
            TeratomeListBox.Items.Filter += FilterByName;
        }

        private bool FilterByName(object obj)
        {
            Recipe recipe = (Recipe)obj;

            if (FilterName.Text.Length > 0)
            {
                if (recipe.Name.IndexOf(FilterName.Text, StringComparison.OrdinalIgnoreCase) == -1)
                    return false;
            }

            return true;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            RecipeBuilder.SaveRecipes((IEnumerable<Recipe>)TeratomeListBox.DataContext);
            Reload(null, null);
        }

        private void Reload(object sender, RoutedEventArgs e)
        {
            TeratomeListBox.DataContext = RecipeBuilder.Recipes.ToList();
        }

        private void New(object sender, RoutedEventArgs e)
        {
            if (RecipeBuilder.Recipes.Any(q => q.Name == "NewRecipe" || q.RecipeId == 123456789))
            {
                MessageBox.Show(
                    "Вы уже добавили пустой рецепт и не редактировали его!\nПрежде чем добавлять новый, заполните старый!",
                    "Внимание!");
                return;
            }
            RecipeBuilder.Recipes.Add(new Recipe { Name = "NewRecipe", RecipeId = 123456789 });
            Reload(null, null);
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            RecipeBuilder.Recipes.Remove((Recipe)TeratomeListBox.SelectedItem);
            Reload(null, null);
        }
    }
}
