using Microsoft.Maui.Controls;
using RecipeApp.Classes;
using RecipeApp.ViewModel;

namespace RecipeApp.Screens
{
    [QueryProperty(nameof(Recipe), "Recipe")]
    public partial class RecipeDetails : ContentPage
    {
        private Recipes _recipe;
        public Recipes Recipe
        {
            get => _recipe;
            set
            {
                _recipe = value;
                OnPropertyChanged();
                if (BindingContext is RecipeDetailsViewModel viewModel)
                {
                    viewModel.Recipe = _recipe;
                }
            }
        }

        public RecipeDetails()
        {
            InitializeComponent();
            BindingContext = new RecipeDetailsViewModel();
        }

        
    }
}
