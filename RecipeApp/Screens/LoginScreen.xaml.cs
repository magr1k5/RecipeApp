namespace RecipeApp.Screens;
using Npgsql;
using RecipeApp.Classes;

public partial class LoginScreen : ContentPage
{
    private Entry usernameEntry;
    private Entry passwordEntry;
    private DatabaseManager dbManager;

    public LoginScreen()
    {
        InitializeComponent();

        usernameEntry = new Entry { Placeholder = "Введите ваше имя пользователя" };
        passwordEntry = new Entry { Placeholder = "Введите ваш пароль", IsPassword = true };
        var localhost = "localhost";
        var android_local = "10.0.2.2";
        Button loginButton = new Button { Text = "Войти" };
        loginButton.Clicked += OnLoginButtonClicked;

        this.Content = new StackLayout
        {
            Children = { usernameEntry, passwordEntry, loginButton }
        };

        dbManager = new DatabaseManager($"User Id=postgres;Host={android_local};Database=recipe_app_db;Port=5432;password=root");
    }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        string username = usernameEntry.Text;
        string password = Hashing.toSHA256(passwordEntry.Text);

        dbManager.OpenConnection();
        Users currentUser = dbManager.GetUserByUsernameAndPassword(username, password);
        dbManager.CloseConnection();

        if (currentUser != null)
        {
            App.CurrentUser = currentUser;
            string welcomeMessage = $"Добро пожаловать {currentUser.Name} {currentUser.Surname}";
            await DisplayAlert("Добро пожаловать", welcomeMessage, "Ок");

            App.UserViewModel.FullName = $"{currentUser.Name} {currentUser.Surname}";
            App.UserViewModel.UserName = currentUser.Username;

            if (currentUser.RoleName == "Admin")
            {
                App.Current.MainPage = new AdminShell(); // Навигационное меню для администратора
            }
            else
            {
                App.Current.MainPage = new AppShell(); // Обычное навигационное меню
            }
        }
        else
        {
            await DisplayAlert("Ошибка", "Ошибка. Пароль или логин неверные", "Ок");
        }
    }
}
