using RecipeApp.ViewModel;
using System.Windows.Input;

namespace RecipeApp.Screens;

public partial class PurchasesScreen : ContentPage
{
    public string SearchQuery { get; set; }
    public ICommand SearchCommand { get; set; }
    public PurchasesScreen()
	{
		InitializeComponent();
        BindingContext = App.RecipesViewModel;

    }
}