namespace RecipeApp.Screens;
using RecipeApp.ViewModel;
public partial class RecipesScreen : ContentPage
{
	public RecipesScreen()
	{
		InitializeComponent();
        BindingContext = new RecipesViewModel();
    }

    
}