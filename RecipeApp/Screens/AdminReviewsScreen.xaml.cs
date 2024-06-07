using RecipeApp.Classes;
using System.Collections.ObjectModel;
using System.Windows.Input;


namespace RecipeApp.Screens
{
    public partial class AdminReviewsScreen : ContentPage
    {
        private DatabaseManager _dbManager;
        public ObservableCollection<Recipes> PendingRecipes { get; set; }

        public ICommand ApproveCommand { get; }
        public ICommand RejectCommand { get; }

        public AdminReviewsScreen()
        {
            InitializeComponent();
            var localhost = "localhost";
            var android_local = "10.0.2.2";
            _dbManager = new DatabaseManager($"User Id=postgres;Host={android_local};Database=recipe_app_db;Port=5432;password=root");
            PendingRecipes = new ObservableCollection<Recipes>();

            ApproveCommand = new Command<int>(ApproveRecipe);
            RejectCommand = new Command<int>(RejectRecipe);

            LoadPendingRecipes();
            BindingContext = this;
        }

        private void LoadPendingRecipes()
        {
            var pendingRecipes = _dbManager.GetPendingRecipes();
            foreach (var recipe in pendingRecipes)
            {
                var user = _dbManager.GetUserById(recipe.userid);
                if (user != null)
                {
                    recipe.CreatorName = $"{user.Name} {user.Surname}";
                }
                else
                {
                    
                    recipe.CreatorName = "Unknown User"; 
                }
                PendingRecipes.Add(recipe);
            }
        }


        private void ApproveRecipe(int recipeId)
        {
            bool success = _dbManager.UpdateRecipePendingStatus(recipeId, false);
            if (success)
            {
                var recipe = PendingRecipes.First(r => r.recipeid == recipeId);
                PendingRecipes.Remove(recipe);
            }
        }

        private void RejectRecipe(int recipeId)
        {
            bool success = _dbManager.DeleteRecipe(recipeId);
            if (success)
            {
                var recipe = PendingRecipes.First(r => r.recipeid == recipeId);
                PendingRecipes.Remove(recipe);
            }
        }
    }
}
