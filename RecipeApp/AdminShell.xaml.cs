using RecipesApp.ViewModel;
using RecipesApp.Screens;
using RecipeApp.Screens;
namespace RecipeApp
{
    public partial class AdminShell : Shell
    {
        public UserViewModel ViewModel { get; } = new UserViewModel();
        public AdminShell()
        {
            InitializeComponent();
            BindingContext = App.UserViewModel;
        }

        private async void OnAddRecipeButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddRecipeScreen());
            Routing.RegisterRoute("RecipeDetails", typeof(RecipeDetails));

        }

        private async void OnReviewRecipesButtonClicked(object sender , EventArgs e)
        {
            await Navigation.PushAsync(new AdminReviewsScreen());
        }

        private async void OnUserRecipesButtonClicked(object sender , EventArgs e)
        {
            await Navigation.PushAsync(new UsersRecipesScreen());
        }
    }
}
