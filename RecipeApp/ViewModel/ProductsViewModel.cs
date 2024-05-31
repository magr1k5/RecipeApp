using RecipeApp.Classes;
using RecipeApp.Screens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RecipeApp.ViewModel
{


    public class ProductsViewModel : BindableObject
    {
        private DatabaseManager dbManager;

        public ObservableCollection<Ingredient> Ingredients { get; set; }
        public ObservableCollection<Ingredient> SelectedIngredients { get; set; }
        public ICommand NextCommand { get; }

        public ProductsViewModel()
        {
            dbManager = new DatabaseManager($"User Id=postgres;Host=localhost;Database=recipe_app_db;Port=5432;password=root");
            Ingredients = new ObservableCollection<Ingredient>(GetIngredients());
            SelectedIngredients = new ObservableCollection<Ingredient>();
            SelectedIngredients.CollectionChanged += OnSelectedIngredientsChanged;

            NextCommand = new Command(OnNext);
        }

        private void OnSelectedIngredientsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (var ingredient in Ingredients)
            {
                ingredient.IsMissing = SelectedIngredients.Contains(ingredient);
            }
        }

        private List<Ingredient> GetIngredients()
        {
            dbManager.OpenConnection();
            List<Ingredient> ingredients = dbManager.GetIngredients();
            dbManager.CloseConnection();
            return ingredients;
        }

        private async void OnNext()
        {
            var selectedIngredientIds = SelectedIngredients.Select(i => i.IngredientID).ToList();
            var selectedIngredientIdsString = string.Join(",", selectedIngredientIds);
            var route = $"///RecipesAfterChoose?SelectedIngredientIds={selectedIngredientIdsString}";
            await Shell.Current.GoToAsync(route);
        }

    }
}


