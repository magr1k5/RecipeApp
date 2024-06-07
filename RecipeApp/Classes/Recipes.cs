using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeApp.Classes
{
    public class Recipes
    {
        public int recipeid { get; set; }
        public string recipename { get; set; }
        public string recipedescription { get; set; }
        public string recipegroup { get; set; }
        public int userid { get; set; }

        public string img { get; set; }

        public bool pending { get; set; }

        public string CreatorName { get; set; } // Для отображения имени создателя

    }
}
