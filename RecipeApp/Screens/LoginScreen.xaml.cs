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

        usernameEntry = new Entry { Placeholder = "������� ���� ��� ������������" };
        passwordEntry = new Entry { Placeholder = "������� ��� ������", IsPassword = true };
        var localhost = "localhost";
        var android_local = "10.0.2.2";
        Button loginButton = new Button { Text = "�����" };
        loginButton.Clicked += OnLoginButtonClicked;

        this.Content = new StackLayout
        {
            Children = { usernameEntry, passwordEntry, loginButton }
        };

        dbManager = new DatabaseManager($"User Id=postgres;Host={localhost};Database=recipe_app_db;Port=5432;password=root"); // 192.168.0.101

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
            string welcomeMessage = $"����� ���������� {currentUser.Name} {currentUser.Surname}";
            await DisplayAlert("����� ����������", welcomeMessage, "��");
            await Navigation.PushAsync(new MainScreen());
            //SetTopBarVisibility(true);
        }
        else
        {
            await DisplayAlert("������", "������. ������ ��� ����� ��������", "��");
        }

    }

    /*private void SetTopBarVisibility(bool isVisible)
    {
        if (Application.Current.MainPage is Shell shell)
        {
            if (shell.CurrentItem is ShellItem shellItem)
            {
                if (shellItem.CurrentItem is ShellSection shellSection)
                {
                    shellSection.TitleViewVisibility = isVisible ? ShellTitleViewVisibility.Default : ShellTitleViewVisibility.Hidden;
                }
            }
        }
    }*/
}