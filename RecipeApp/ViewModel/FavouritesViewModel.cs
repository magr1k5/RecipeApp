using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using RecipeApp.Classes;
namespace RecipeApp.ViewModel
{
    public class FavouritesViewModel: BindableObject
    {
        private DatabaseManager _dbManager;
        public ObservableCollection<Recipes> FavoriteRecipes { get; set; }
        public ICommand RemoveFromFavoritesCommand { get; }
        public ICommand RecipeSelectedCommand { get; }
        public ICommand SearchCommand { get; }

        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
            }
        }

        public FavouritesViewModel()
        {
            _dbManager = new DatabaseManager("User Id=postgres;Host=localhost;Database=recipe_app_db;Port=5432;password=root");
            FavoriteRecipes = new ObservableCollection<Recipes>();
            RemoveFromFavoritesCommand = new Command<Recipes>(async (recipe) => await RemoveFromFavoritesAsync(recipe));
            RecipeSelectedCommand = new Command<Recipes>(OnRecipeSelected);
            SearchCommand = new Command(Search);
            LoadFavoriteRecipes();
        }

        public async void LoadFavoriteRecipes()
        {
            if (App.CurrentUser == null) return;

            _dbManager.OpenConnection();
            var favoriteRecipeIds = _dbManager.GetFavoriteRecipes(App.CurrentUser.Id);
            _dbManager.CloseConnection();

            FavoriteRecipes.Clear();
            foreach (var recipeId in favoriteRecipeIds)
            {
                var recipe = await GetRecipeByIdAsync(recipeId);
                if (recipe != null)
                {
                    FavoriteRecipes.Add(recipe);
                }
            }
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

        private async Task<Recipes> GetRecipeByIdAsync(int recipeId)
        {
            return await Task.Run(() =>
            {
                _dbManager.OpenConnection();
                var recipe = _dbManager.GetRecipeById(recipeId);
                _dbManager.CloseConnection();
                return recipe;
            });
        }

        private async Task RemoveFromFavoritesAsync(Recipes recipe)
        {
            if (App.CurrentUser == null || recipe == null) return;

            _dbManager.OpenConnection();
            bool removed = _dbManager.RemoveRecipeFromFavorites(App.CurrentUser.Id, recipe.recipeid);
            _dbManager.CloseConnection();

            if (removed)
            {
                FavoriteRecipes.Remove(recipe);
                await Application.Current.MainPage.DisplayAlert("Успех", "Рецепт удален из избранного.", "ОК");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Не удалось удалить рецепт из избранного.", "ОК");
            }
        }

        private void Search()
        {
            if (string.IsNullOrEmpty(SearchQuery))
            {
                LoadFavoriteRecipes();
            }
            else
            {
                var filteredRecipes = FavoriteRecipes.Where(r => r.recipename.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)).ToList();
                FavoriteRecipes.Clear();
                foreach (var recipe in filteredRecipes)
                {
                    FavoriteRecipes.Add(recipe);
                }
            }
        }
    }
}
