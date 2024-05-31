using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RecipeApp.Classes
{
    public class Ingredient : BindableObject
    {
        private bool isSelected;

        public int IngredientID { get; set; }
        public string IngredientName { get; set; }
        public string IngredientGroup { get; set; }
        public string Quantity { get; set; }

        public string Img { get; set; }

        public bool IsMissing
        {
            get => isSelected;
            set
            {
                isSelected = value;
                OnPropertyChanged();
            }
        }
    }
}
