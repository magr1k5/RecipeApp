using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using RecipeApp.Classes;
using RecipeApp.Screens;
namespace RecipeApp.ViewModel
{
    public class AddRecipeViewModel : BindableObject
    {
        private DatabaseManager dbManager;

        public AddRecipeViewModel()
        {
            dbManager = new DatabaseManager("User Id=postgres;Host=localhost;Database=recipe_app_db;Port=5432;password=root;");
            Ingredients = new ObservableCollection<Ingredient>(dbManager.GetIngredients());
            RecipeGroups = new ObservableCollection<string>(dbManager.GetRecipeGroups());
            RecipeIngredients = new ObservableCollection<Ingredient>();
            RecipeSteps = new ObservableCollection<RecipesSteps>();
            Units = new ObservableCollection<string>
            {
                "ст. ложек", "шт.", "ч. ложек", "л.", "мл.", "гр.", "по вкусу", "щепотка", "для жарки"
            };

            AddIngredientCommand = new Command(AddIngredient);
            RemoveIngredientCommand = new Command<Ingredient>(RemoveIngredient);
            AddStepCommand = new Command(AddStep);
            RemoveStepCommand = new Command<RecipesSteps>(RemoveStep);
            SaveRecipeCommand = new Command(SaveRecipe);
        }

        public string RecipeName { get; set; }
        public string RecipeDescription { get; set; }
        public ObservableCollection<Ingredient> Ingredients { get; set; }
        public Ingredient SelectedIngredient { get; set; }
        private string ingredientQuantity;
        public string IngredientQuantity
        {
            get => ingredientQuantity;
            set
            {
                if (Regex.IsMatch(value, @"^\d*$")) // Только цифры
                {
                    ingredientQuantity = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<string> RecipeGroups { get; set; }
        public string SelectedRecipeGroup { get; set; }
        public ObservableCollection<Ingredient> RecipeIngredients { get; set; }
        public ObservableCollection<RecipesSteps> RecipeSteps { get; set; }
        public string StepDescription { get; set; }
        public ObservableCollection<string> Units { get; set; }
        public string SelectedUnit { get; set; }
        public ICommand AddIngredientCommand { get; }
        public ICommand RemoveIngredientCommand { get; }
        public ICommand AddStepCommand { get; }
        public ICommand RemoveStepCommand { get; }
        public ICommand SaveRecipeCommand { get; }

        private void AddIngredient()
        {
            if (SelectedIngredient != null && !string.IsNullOrEmpty(IngredientQuantity) && !string.IsNullOrEmpty(SelectedUnit))
            {
                var recipeIngredient = new Ingredient
                {
                    IngredientName = SelectedIngredient.IngredientName,
                    IngredientGroup = $"{IngredientQuantity} {SelectedUnit}"
                };
                RecipeIngredients.Add(recipeIngredient);
                OnPropertyChanged(nameof(RecipeIngredients));
            }
        }

        private void RemoveIngredient(Ingredient ingredient)
        {
            if (ingredient != null)
            {
                RecipeIngredients.Remove(ingredient);
                OnPropertyChanged(nameof(RecipeIngredients));
            }
        }

        private void AddStep()
        {
            if (!string.IsNullOrEmpty(StepDescription))
            {
                var recipeStep = new RecipesSteps
                {
                    stepdescription = StepDescription,
                    stepnumber = RecipeSteps.Count + 1
                };
                RecipeSteps.Add(recipeStep);
                OnPropertyChanged(nameof(RecipeSteps));
            }
        }

        private void RemoveStep(RecipesSteps step)
        {
            if (step != null)
            {
                RecipeSteps.Remove(step);
                // Обновляем нумерацию шагов
                int stepNumber = 1;
                foreach (var recipeStep in RecipeSteps)
                {
                    recipeStep.stepnumber = stepNumber;
                    stepNumber++;
                }
                OnPropertyChanged(nameof(RecipeSteps));
            }
        }

        private void SaveRecipe()
        {
            if (string.IsNullOrEmpty(RecipeName) || string.IsNullOrEmpty(RecipeDescription) || string.IsNullOrEmpty(SelectedRecipeGroup) || RecipeIngredients.Count == 0 || RecipeSteps.Count == 0)
            {
                Application.Current.MainPage.DisplayAlert("Ошибка", "Заполните все поля, добавьте ингредиенты и шаги.", "OK");
                return;
            }

            string img = "test.jpg"; // Placeholder image
            int userId = App.CurrentUser.Id; // Получаем текущего пользователя

            int recipeId = dbManager.AddRecipe(RecipeName, RecipeDescription, SelectedRecipeGroup, userId, img, true);

            foreach (var ingredient in RecipeIngredients)
            {
                int ingredientId = dbManager.GetIngredientIdByName(ingredient.IngredientName);
                dbManager.AddRecipeIngredient(recipeId, ingredientId, ingredient.IngredientGroup);
            }

            foreach (var step in RecipeSteps)
            {
                dbManager.AddRecipeStep(recipeId, step.stepnumber, step.stepdescription);
            }

            Application.Current.MainPage.DisplayAlert("Успех", "Рецепт успешно добавлен.", "OK");
            ClearForm();
        }

        private void ClearForm()
        {
            RecipeName = string.Empty;
            RecipeDescription = string.Empty;
            SelectedRecipeGroup = null;
            RecipeIngredients.Clear();
            RecipeSteps.Clear();
            IngredientQuantity = string.Empty;
            StepDescription = string.Empty;
            OnPropertyChanged(nameof(RecipeName));
            OnPropertyChanged(nameof(RecipeDescription));
            OnPropertyChanged(nameof(SelectedRecipeGroup));
            OnPropertyChanged(nameof(RecipeIngredients));
            OnPropertyChanged(nameof(RecipeSteps));
            OnPropertyChanged(nameof(IngredientQuantity));
            OnPropertyChanged(nameof(StepDescription));
        }
    }
}
