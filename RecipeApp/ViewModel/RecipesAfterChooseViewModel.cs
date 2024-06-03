using RecipeApp.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace RecipeApp.ViewModel
{
    [QueryProperty(nameof(SelectedIngredientIds), "SelectedIngredientIds")]
    public class RecipesAfterChooseViewModel : BindableObject
    {
        private DatabaseManager dbManager;

        public ObservableCollection<RecipeWithMissingIngredients> Recipes { get; set; }
        public ObservableCollection<Recipes> LoadingRecipes { get; set; }
        public ObservableCollection<string> ShoppingList { get; set; }

        public bool CanStartCooking => Recipes.Any(r => r.MissingIngredientsCount == 0);

        public ICommand RecipeSelectedCommand { get; set; }
        public ICommand AddToShoppingListCommand { get; set; }

        private string selectedIngredientIds;
        public string SelectedIngredientIds
        {
            get => selectedIngredientIds;
            set
            {
                selectedIngredientIds = Uri.UnescapeDataString(value ?? string.Empty);
                LoadRecipes();
            }
        }

        public RecipesAfterChooseViewModel()
        {
            dbManager = new DatabaseManager($"User Id=postgres;Host=localhost;Database=recipe_app_db;Port=5432;password=root");
            Recipes = new ObservableCollection<RecipeWithMissingIngredients>();
            LoadingRecipes = new ObservableCollection<Recipes>(GetRecipes());
            ShoppingList = new ObservableCollection<string>();

            RecipeSelectedCommand = new Command<RecipeWithMissingIngredients>(OnRecipeSelected);
            AddToShoppingListCommand = new Command<RecipeWithMissingIngredients>(OnAddToShoppingList);
        }

        private List<Recipes> GetRecipes()
        {
            dbManager.OpenConnection();
            List<Recipes> recipes = dbManager.GetRecipes();
            dbManager.CloseConnection();
            return recipes;
        }

        private void LoadRecipes()
        {
            if (string.IsNullOrWhiteSpace(selectedIngredientIds))
            {
                Recipes.Clear();
                OnPropertyChanged(nameof(CanStartCooking));
                return;
            }

            var selectedIngredientIdsList = selectedIngredientIds
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(id => int.TryParse(id, out var intId) ? intId : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToList();

            Recipes.Clear();
            var allRecipes = dbManager.GetRecipes();

            foreach (var recipe in allRecipes)
            {
                var recipeIngredients = dbManager.GetIngredientsForRecipe(recipe.recipeid)
                    .Select(i => i.IngredientID)
                    .ToList();

                var missingIngredients = recipeIngredients.Except(selectedIngredientIdsList).ToList();
                var missingIngredientNames = dbManager.GetIngredientsForRecipe(recipe.recipeid)
                    .Where(i => missingIngredients.Contains(i.IngredientID))
                    .Select(i => i.IngredientName)
                    .ToList();

                Recipes.Add(new RecipeWithMissingIngredients(recipe, missingIngredients.Count, missingIngredientNames));
            }

            Recipes = new ObservableCollection<RecipeWithMissingIngredients>(Recipes.OrderBy(r => r.MissingIngredientsCount));
            OnPropertyChanged(nameof(Recipes));
            OnPropertyChanged(nameof(CanStartCooking));
        }

        private async void OnRecipeSelected(RecipeWithMissingIngredients recipe)
        {
            if (recipe != null)
            {
                Debug.WriteLine($"Recipe selected: {recipe.recipename}");

                // Создаем список параметров для навигации
                var navigationParameters = new List<object>
        {
            recipe // Добавляем рецепт в список параметров
        };

                // Выполняем навигацию с параметрами
                await Shell.Current.GoToAsync("///RecipeDetails", true, new Dictionary<string, object>
        {
            { "NavigationParameters", navigationParameters }
        });
            }
        }


        private void OnAddToShoppingList(RecipeWithMissingIngredients recipe)
        {
            foreach (var ingredient in recipe.MissingIngredientNames)
            {
                Debug.WriteLine($"Ingredients names: {recipe.MissingIngredientNames}");
                if (!ShoppingList.Contains(ingredient))
                {
                    ShoppingList.Add(ingredient);
                }
            }
            OnPropertyChanged(nameof(ShoppingList));
        }

        public class RecipeWithMissingIngredients : Recipes
        {
            public int MissingIngredientsCount { get; }
            public List<string> MissingIngredientNames { get; }

            public string MissingIngredientsText => MissingIngredientsCount == 0
                ? "МОЖНО НАЧИНАТЬ ГОТОВИТЬ"
                : $"Не хватает {MissingIngredientsCount} продуктов";

            public string MissingIngredientNamesText => MissingIngredientNames != null && MissingIngredientNames.Any()
                ? string.Join(", ", MissingIngredientNames)
                : string.Empty;

            public RecipeWithMissingIngredients(Recipes recipe, int missingIngredientsCount, List<string> missingIngredientNames)
            {
                recipeid = recipe.recipeid;
                recipename = recipe.recipename;
                recipedescription = recipe.recipedescription;
                recipegroup = recipe.recipegroup;
                userid = recipe.userid;
                img = recipe.img;
                pending = recipe.pending;
                MissingIngredientsCount = missingIngredientsCount;
                MissingIngredientNames = missingIngredientNames;
            }
        }
    }
}
