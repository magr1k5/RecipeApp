using RecipeApp.ViewModel;

namespace RecipeApp.Screens;

public partial class PurchasesScreen : ContentPage
{
	public PurchasesScreen()
	{
		InitializeComponent();
        BindingContext = App.RecipesViewModel;

    }
}