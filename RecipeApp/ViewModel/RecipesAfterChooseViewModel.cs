using RecipeApp.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RecipeApp.ViewModel
{
    [QueryProperty(nameof(SelectedIngredientIds), "SelectedIngredientIds")]
    public class RecipesAfterChooseViewModel : BindableObject
    {
        private DatabaseManager dbManager;

        public ObservableCollection<RecipeWithMissingIngredients> Recipes { get; set; }
        public bool CanStartCooking => Recipes.Any(r => r.MissingIngredientsCount == 0);
       
        public ICommand RecipeSelectedCommand { get; set; }

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
            RecipeSelectedCommand = new Command<RecipeWithMissingIngredients>(OnRecipeSelected);
        }

        private void LoadRecipes()
        {
            // Проверка, что selectedIngredientIds не пустая
            if (string.IsNullOrWhiteSpace(selectedIngredientIds))
            {
                Recipes.Clear();
                OnPropertyChanged(nameof(CanStartCooking));
                return;
            }

            // Разбиваем строку на список идентификаторов и игнорируем пустые элементы
            var selectedIngredientIdsList = selectedIngredientIds
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(id => int.TryParse(id, out var intId) ? intId : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToList();

            // Очищаем предыдущие результаты
            Recipes.Clear();

            // Загрузите все рецепты
            var allRecipes = dbManager.GetRecipes();

            foreach (var recipe in allRecipes)
            {
                var recipeIngredients = dbManager.GetIngredientsForRecipe(recipe.recipeid)
                    .Select(i => i.IngredientID)
                    .ToList();

                var missingIngredients = recipeIngredients.Except(selectedIngredientIdsList).Count();

                Recipes.Add(new RecipeWithMissingIngredients(recipe, missingIngredients));
            }

            // Сортировка Recipes по количеству недостающих ингредиентов
            Recipes = new ObservableCollection<RecipeWithMissingIngredients>(Recipes.OrderBy(r => r.MissingIngredientsCount));

            OnPropertyChanged(nameof(Recipes));
            OnPropertyChanged(nameof(CanStartCooking));
        }

        private async void OnRecipeSelected(RecipeWithMissingIngredients recipe)
        {
            if (recipe != null)
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "Recipe", recipe },
                    { "SelectedIngredientIds", SelectedIngredientIds }
                };

                await Shell.Current.GoToAsync("///RecipeDetails", navigationParameter);
            }
        }

        




        public class RecipeWithMissingIngredients : Recipes
        {
            public int MissingIngredientsCount { get; }

            public string MissingIngredientsText => MissingIngredientsCount == 0
                ? "МОЖНО НАЧИНАТЬ ГОТОВИТЬ"
                : $"Не хватает {MissingIngredientsCount} продуктов";

            public RecipeWithMissingIngredients(Recipes recipe, int missingIngredientsCount)
            {
                recipeid = recipe.recipeid;
                recipename = recipe.recipename;
                recipedescription = recipe.recipedescription;
                recipegroup = recipe.recipegroup;
                userid = recipe.userid;
                img = recipe.img;
                pending = recipe.pending;
                MissingIngredientsCount = missingIngredientsCount;
            }
        }
    }
}
