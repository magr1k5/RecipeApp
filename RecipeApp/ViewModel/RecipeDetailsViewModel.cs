using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using RecipeApp.Classes;

namespace RecipeApp.ViewModel
{
    public class RecipeDetailsViewModel : BindableObject
    {
        private Recipes _recipe;
        private DatabaseManager _dbManager;
        private Users currentUser;

        

        public RecipeDetailsViewModel()
        {
            Ingredients = new ObservableCollection<Ingredient>();
            Steps = new ObservableCollection<RecipesSteps>();
            _dbManager = new DatabaseManager($"User Id=postgres;Host=localhost;Database=recipe_app_db;Port=5432;password=root");
            currentUser = App.CurrentUser; // Предположим, что текущий пользователь хранится в App.CurrentUser
            AddToFavoritesCommand = new Command(async () => await AddToFavoriteAsync());

        }
        public ICommand AddToFavoritesCommand { get; }
        private async Task AddToFavoriteAsync()
        {
            if (Recipe == null || currentUser == null)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Рецепт или пользователь не определены.", "OK");
                return;
            }

            int recipeId = Recipe.recipeid;

            try
            {
                _dbManager.OpenConnection();

                if (!_dbManager.IsRecipeInFavorites(currentUser.Id, recipeId))
                {
                    bool added = _dbManager.AddRecipeToFavorites(currentUser.Id, recipeId);

                    if (added)
                    {
                        await Application.Current.MainPage.DisplayAlert("Успешно", "Рецепт добавлен в избранное.", "OK");
                        MessagingCenter.Send(this, "FavoriteRecipesUpdated");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Ошибка", "Не удалось добавить рецепт в избранное.", "OK");
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Предупреждение", "Рецепт уже в избранном.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Произошла ошибка: {ex.Message}", "OK");
            }
            finally
            {
                _dbManager.CloseConnection();
            }
        }

        public Recipes Recipe
        {
            get => _recipe;
            set
            {
                _recipe = value;
                OnPropertyChanged();
                LoadRecipeDetails();
            }
        }

        public ObservableCollection<Ingredient> Ingredients { get; set; }
        public ObservableCollection<RecipesSteps> Steps { get; set; }

        public async void LoadRecipeDetails()
        {
            if (Recipe == null) return;

            var ingredients = await Task.Run(() =>
            {
                _dbManager.OpenConnection();
                var result = _dbManager.GetIngredientsForRecipe(_recipe.recipeid);
                _dbManager.CloseConnection();
                return result;
            });

            var steps = await Task.Run(() =>
            {
                _dbManager.OpenConnection();
                var result = _dbManager.GetRecipeSteps(_recipe.recipeid);
                _dbManager.CloseConnection();
                return result;
            });

            Device.BeginInvokeOnMainThread(() =>
            {
                Ingredients.Clear();
                foreach (var ingredient in ingredients)
                {
                    Ingredients.Add(ingredient);
                }

                Steps.Clear();
                foreach (var step in steps)
                {
                    Steps.Add(step);
                }
            });
        }

    }
}
