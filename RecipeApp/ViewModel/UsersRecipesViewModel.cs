using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using RecipeApp.Classes;
namespace RecipeApp.ViewModel
{
    public class UsersRecipesViewModel : BindableObject
    {
        private DatabaseManager dbManager;

        public UsersRecipesViewModel()
        {
            dbManager = new DatabaseManager("User Id=postgres;Host=localhost;Database=recipe_app_db;Port=5432;password=root;");
            LoadUserRecipes();
            NavigateToRecipeDetailsCommand = new Command<Recipes>(NavigateToRecipeDetails);

        }

        private ObservableCollection<Recipes> _userRecipes;
        public ObservableCollection<Recipes> UserRecipes
        {
            get { return _userRecipes; }
            set
            {
                _userRecipes = value;
                OnPropertyChanged();
            }
        }
        public ICommand NavigateToRecipeDetailsCommand { get; }


        private void LoadUserRecipes()
        {
            var pendingRecipes = dbManager.GetPendingRecipesByUser(App.CurrentUser.Id);
            var publishedRecipes = dbManager.GetPendingRecipesByUser2(App.CurrentUser.Id);
            var allRecipes = pendingRecipes.Concat(publishedRecipes).ToList();
            UserRecipes = new ObservableCollection<Recipes>(allRecipes);
        }

        private async void NavigateToRecipeDetails(Recipes recipe)
        {
            if (recipe != null)
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "Recipe", recipe }
                };
                await Shell.Current.GoToAsync("///RecipeDetails", navigationParameter);
            }
        }
    }
}
