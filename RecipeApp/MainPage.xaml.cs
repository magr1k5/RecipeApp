using RecipeApp.Screens;

namespace RecipeApp
{
    public partial class MainPage : ContentPage
    {
       

        public MainPage()
        {
            InitializeComponent();
        }

        private async void ToRegisterScreen(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("RegisterScreen");
        }

        private async void ToLoginScreen(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginScreen());
        }
    }

}
