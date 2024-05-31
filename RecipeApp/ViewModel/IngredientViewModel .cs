using RecipeApp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeApp.ViewModel
{
    public class IngredientViewModel : BindableObject
    {
        private Ingredient _ingredient;

        public IngredientViewModel(Ingredient ingredient)
        {
            _ingredient = ingredient;
        }

        public string IngredientName
        {
            get => _ingredient.IngredientName;
            set
            {
                _ingredient.IngredientName = value;
                OnPropertyChanged();
            }
        }

        public string Quantity
        {
            get => _ingredient.Quantity;
            set
            {
                _ingredient.Quantity = value;
                OnPropertyChanged();
            }
        }

        public string IngredientGroup
        {
            get => _ingredient.IngredientGroup;
            set
            {
                _ingredient.IngredientGroup = value;
                OnPropertyChanged();
            }
        }

        public bool IsMissing
        {
            get => _ingredient.IsMissing;
            set
            {
                _ingredient.IsMissing = value;
                OnPropertyChanged();
            }
        }
    }
}
