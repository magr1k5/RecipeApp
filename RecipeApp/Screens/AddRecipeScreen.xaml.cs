namespace RecipesApp.Screens;

public partial class AddRecipeScreen : ContentPage
{
    Entry recipeNameEntry;
    Entry recipeDescriptionEntry;
    Picker recipeGroupPicker;
    Button submitButton;
    public AddRecipeScreen()
	{
        recipeNameEntry = new Entry { Placeholder = "Название рецепта" };
        recipeDescriptionEntry = new Entry { Placeholder = "Описание рецепта" };
        recipeGroupPicker = new Picker { Title = "Группа рецептов" };
        recipeGroupPicker.ItemsSource = new List<string> { "Супы", "Закуски", "Салаты", "Соусы", "На огне", "Напитки", "Детское меню", "Десерты", "Горячее", "Гарниры" };
        submitButton = new Button { Text = "Добавить рецепт" };
        submitButton.Clicked += OnSubmitButtonClicked;
        InitializeComponent();
	}

    private void OnSubmitButtonClicked(object sender, EventArgs e)
    {
        // Здесь вы можете добавить логику для отправки информации о рецепте в базу данных
        // Например:
        // string query = $"INSERT INTO public.recipes VALUES (default, '{recipeNameEntry.Text}', '{recipeDescriptionEntry.Text}', '{recipeGroupPicker.SelectedItem}', {AppShell.ViewModel.UserId}, 'test.jpg', true)";
        // dbManager.ExecuteQuery(query);
    }
}