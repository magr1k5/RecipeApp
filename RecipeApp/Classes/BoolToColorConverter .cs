using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeApp.Classes
{
    public class BoolToColorConverter: IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is int ingredientId && values[1] is ObservableCollection<Ingredient> selectedIngredients)
            {
                return selectedIngredients.Any(i => i.IngredientID == ingredientId) ? Colors.Green : Colors.White;
            }

            return Colors.White;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
