using RecipeApp.ViewModel;
namespace RecipeApp.Screens;

[XamlCompilation(XamlCompilationOptions.Compile)]
[QueryProperty(nameof(SelectedIngredientIds), "SelectedIngredientIds")]

public partial class RecipesAfterChoose : ContentPage
{
    public string SelectedIngredientIds { get; set; }

    public RecipesAfterChoose()
	{
		InitializeComponent();
        BindingContext = App.RecipesViewModel;

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is RecipesAfterChooseViewModel viewModel)
        {
            viewModel.SelectedIngredientIds = SelectedIngredientIds;
        }
    }
}