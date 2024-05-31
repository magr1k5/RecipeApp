using RecipeApp.Classes;
using RecipeApp.ViewModel;

namespace RecipeApp.Screens;

public partial class ProductsScreen : ContentPage
{
	public ProductsScreen()
	{
		InitializeComponent();
	}
    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var viewModel = BindingContext as ProductsViewModel;
        foreach (var removedItem in e.PreviousSelection)
        {
            viewModel.SelectedIngredients.Remove(removedItem as Ingredient);
        }
        foreach (var addedItem in e.CurrentSelection)
        {
            viewModel.SelectedIngredients.Add(addedItem as Ingredient);
        }
    }
}