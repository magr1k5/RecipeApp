using RecipeApp.Classes;
using RecipeApp.Screens;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using System.Linq;

namespace RecipeApp.ViewModel
{
    public class RecipesViewModel : BindableObject
    {


        private DatabaseManager dbManager;
        public ICommand SearchCommand { get; set; }
        public string SearchQuery { get; set; }
        public ObservableCollection<Recipes> Recipes { get; set; }
        public ICommand RecipeSelectedCommand { get; set; }

        public RecipesViewModel()
        {
            dbManager = new DatabaseManager($"User Id=postgres;Host=localhost;Database=recipe_app_db;Port=5432;password=root");
            Recipes = new ObservableCollection<Recipes>(GetRecipes());
            RecipeSelectedCommand = new Command<Recipes>(OnRecipeSelected);
            SearchCommand = new Command(Search);
            LoadRecipes();
        }

        private List<Recipes> GetRecipes()
        {
            dbManager.OpenConnection();
            List<Recipes> recipes = dbManager.GetRecipes();
            dbManager.CloseConnection();
            return recipes;
        }

        private async void OnRecipeSelected(Recipes recipe)
        {
            if (recipe != null)
            {
                Debug.WriteLine($"Recipe selected: {recipe.recipename}");


                var navigationParameter = new Dictionary<string, object>
                {
                    { "Recipe", recipe }
                };

                await Shell.Current.GoToAsync("///RecipeDetails", navigationParameter);
            }
        }
        

        private void LoadRecipes()
        {
            dbManager.OpenConnection();
            List<Recipes> recipes = dbManager.GetRecipes();
            dbManager.CloseConnection();

            Recipes.Clear();
            foreach (var recipe in recipes)
            {
                Recipes.Add(recipe);
            }
        }

        private void Search()
        {
            // Получите текст поискового запроса из свойства SearchQuery
            string searchQuery = SearchQuery;

            // Если поисковой запрос пустой, загрузите все рецепты
            if (string.IsNullOrEmpty(searchQuery))
            {
                LoadRecipes();
            }
            else
            {
                // Выполните поиск рецептов по названию, используя searchQuery
                List<Recipes> searchResults = GetRecipes().Where(r => r.recipename.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

                // Обновите свойство Recipes, чтобы отобразить результаты поиска
                Recipes.Clear();
                foreach (var recipe in searchResults)
                {
                    Recipes.Add(recipe);
                }
                
            }
        }

        

        
    }
}
