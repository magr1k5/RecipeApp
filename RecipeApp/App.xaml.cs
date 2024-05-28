using RecipeApp.Classes;
using RecipeApp.Screens;

namespace RecipeApp
{
    public partial class App : Application
    {
        public static Users CurrentUser { get; set; }
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
            Routing.RegisterRoute("RegisterScreen", typeof(RegisterScreen));
        }
    }
}
