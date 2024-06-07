using RecipeApp.Classes;
using RecipeApp.Screens;
using RecipeApp.ViewModel;
using RecipesApp.ViewModel;
namespace RecipeApp
{
    public partial class App : Application
    {
        public static Users CurrentUser { get; set; }
        public static UserViewModel UserViewModel { get; private set; }
        public static RecipesAfterChooseViewModel RecipesViewModel { get; private set; }


        public App()
        {
            InitializeComponent();
            UserViewModel = new UserViewModel();
            MainPage = new AppShell();
            Routing.RegisterRoute("RegisterScreen", typeof(RegisterScreen));
            
            RecipesViewModel = new RecipesAfterChooseViewModel();

            Resources.Add("BoolToColorConverter", new BoolToColorConverter());
            Resources.Add("InverseBoolConverter", new InverseBoolConverter());
        }
    }
}
