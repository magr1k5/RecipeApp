using RecipesApp.ViewModel;
using RecipesApp.Screens;
using RecipeApp.Screens;
namespace RecipeApp
{
    public partial class AppShell : Shell
    {
        public UserViewModel ViewModel { get; } = new UserViewModel();
        public AppShell()
        {
            InitializeComponent();
            BindingContext = App.UserViewModel;
        }

        private async void OnAddRecipeButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddRecipeScreen());
            Routing.RegisterRoute("RecipeDetails", typeof(RecipeDetails));

        }

        private async void OnUserRecipesButtonClicked(object sender , EventArgs e)
        {
            await Navigation.PushAsync(new UsersRecipesScreen());
        }
    }
}
