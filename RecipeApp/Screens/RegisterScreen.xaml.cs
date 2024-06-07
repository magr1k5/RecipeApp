using Microsoft.Maui.Controls;
using Npgsql;
using System;
using RecipeApp.Classes;
namespace RecipeApp.Screens
{
    public partial class RegisterScreen : ContentPage
    {
        private Entry fullNameEntry;
        private Entry nicknameEntry;
        private Entry passwordEntry;

        public RegisterScreen()
        {
            InitializeComponent();

            fullNameEntry = new Entry { Placeholder = "Enter your full name" };
            nicknameEntry = new Entry { Placeholder = "Enter your nickname" };
            passwordEntry = new Entry { Placeholder = "Create a password", IsPassword = true };

            Button registerButton = new Button { Text = "Register" };
            registerButton.Clicked += OnRegisterButtonClicked;

            this.Content = new StackLayout
            {
                Children = { fullNameEntry, nicknameEntry, passwordEntry, registerButton }
            };
        }

        private async void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            string fullName = fullNameEntry.Text;
            string[] nameParts = fullName.Split(' ');
            string name = nameParts[0];
            string surname = nameParts.Length > 1 ? nameParts[1] : string.Empty; // Если введена только одна часть имени, фамилия будет пустой строкой

            string username = nicknameEntry.Text;
            string password = passwordEntry.Text;

            password = Hashing.toSHA256(password);
            var localhost = "localhost";
            var android_local = "10.0.2.2";
            string connectionString = $"User Id=postgres;Host={android_local};Database=recipe_app_db;Port=5432;password=root"; 
            using (DatabaseManager dbManager = new DatabaseManager(connectionString))
            {
                dbManager.OpenConnection();

                string insertQuery = "INSERT INTO users (username, password, roleid, name, surname) VALUES (@Username, @Password, @RoleID, @Name, @Surname)";
                using (NpgsqlCommand cmd = new NpgsqlCommand(insertQuery, dbManager.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@RoleID", 1); 
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Surname", surname);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        await DisplayAlert("Success", "Registration successful. You can now log in.", "OK");
                        // TODO: Navigate to login screen
                    }
                    else
                    {
                        await DisplayAlert("Error", "Registration error. Please try again.", "OK");
                    }
                }

                dbManager.CloseConnection();
            }
        }

    }
}
