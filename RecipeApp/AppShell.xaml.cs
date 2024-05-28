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
            BindingContext = ViewModel;
        }

        private async void OnAddRecipeButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddRecipeScreen());
            Routing.RegisterRoute("RecipeDetails", typeof(RecipeDetails));

        }
    }
}
