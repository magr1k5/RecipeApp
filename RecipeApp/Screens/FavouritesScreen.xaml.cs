using RecipeApp.ViewModel;

namespace RecipeApp.Screens;

public partial class FavouritesScreen : ContentPage
{
    private FavouritesViewModel _viewModel;

    public FavouritesScreen()
	{
		InitializeComponent();
        _viewModel = new FavouritesViewModel();
        BindingContext = new FavouritesViewModel();
        MessagingCenter.Subscribe<RecipeDetailsViewModel>(this, "FavoriteRecipesUpdated", (sender) =>
        {
            _viewModel.LoadFavoriteRecipes();
        });
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        MessagingCenter.Unsubscribe<RecipeDetailsViewModel>(this, "FavoriteRecipesUpdated");
    }
}