using RecipeApp.Classes;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeApp.Screens
{
    public partial class EventScreen : ContentPage
    {
        private DatabaseManager dbManager;
        private ObservableCollection<Recipes> allRecipes;
        private ObservableCollection<UserEventViewModel> events;

        public EventScreen()
        {
            InitializeComponent();
            dbManager = new DatabaseManager("User Id=postgres;Host=localhost;Database=recipe_app_db;Port=5432;password=root;");
            LoadRecipes();
            LoadUserEvents();
        }

        private void LoadRecipes()
        {
            var recipes = dbManager.GetRecipes();
            allRecipes = new ObservableCollection<Recipes>(recipes);
        }

        private void LoadUserEvents()
        {
            var userEvents = dbManager.GetUserEvents(App.CurrentUser.Id);
            events = new ObservableCollection<UserEventViewModel>();

            foreach (var userEvent in userEvents)
            {
                var eventViewModel = new UserEventViewModel
                {
                    EventId = userEvent.EventId,
                    EventName = userEvent.EventName,
                    Recipes = new ObservableCollection<Recipes>(userEvent.RecipeIds.Select(id => allRecipes.FirstOrDefault(r => r.recipeid == id)).Where(r => r != null))
                };

                // Логирование загруженных данных
                Debug.WriteLine($"Event ID: {userEvent.EventId}, Event Name: {userEvent.EventName}");
                foreach (var recipe in eventViewModel.Recipes)
                {
                    Debug.WriteLine($"Recipe ID: {recipe.recipeid}, Recipe Name: {recipe.recipename}");
                }

                events.Add(eventViewModel);
            }

            EventsCollectionView.ItemsSource = events;
        }


        private async void OnCreateEventButtonClicked(object sender, EventArgs e)
        {
            string eventName = await DisplayPromptAsync("Создать событие", "Введите название события:");
            if (string.IsNullOrEmpty(eventName)) return;

            var selectedRecipes = await ShowRecipesSelection();

            if (selectedRecipes == null || selectedRecipes.Count == 0) return;

            List<int> selectedRecipeIds = selectedRecipes.Select(r => r.recipeid).ToList();
            dbManager.CreateEvent(eventName, App.CurrentUser.Id, selectedRecipeIds);

            events.Add(new UserEventViewModel
            {
                EventId = events.Count + 1, // Assuming the new event gets a new unique ID
                EventName = eventName,
                Recipes = new ObservableCollection<Recipes>(selectedRecipes)
            });
        }

        private async Task<List<Recipes>> ShowRecipesSelection()
        {
            var tcs = new TaskCompletionSource<List<Recipes>>();
            var selectedRecipes = new List<Recipes>();

            var collectionView = new CollectionView
            {
                ItemsSource = allRecipes,
                SelectionMode = SelectionMode.Multiple,
                ItemTemplate = new DataTemplate(() =>
                {
                    var grid = new Grid { Padding = 5 };
                    var nameLabel = new Label { FontSize = 20 };
                    nameLabel.SetBinding(Label.TextProperty, "recipename");

                    grid.Children.Add(nameLabel);
                    return new ViewCell { View = grid };
                })
            };

            var page = new ContentPage
            {
                Title = "Выберите рецепты",
                Content = new StackLayout
                {
                    Padding = 20,
                    Children =
                    {
                        new Label { Text = "Выберите рецепты для события", FontSize = 20, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 0, 0, 10) },
                        collectionView,
                        new Button
                        {
                            Text = "Сохранить",
                            Command = new Command(async () =>
                            {
                                var selectedItems = collectionView.SelectedItems.Cast<Recipes>().ToList();
                                await Navigation.PopModalAsync();
                                tcs.SetResult(selectedItems);
                            })
                        }
                    }
                }
            };

            await Navigation.PushModalAsync(page);
            return await tcs.Task;
        }

        private void OnDeleteEventButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var eventId = (int)button.CommandParameter;
            dbManager.DeleteEvent(eventId);
            var eventToRemove = events.FirstOrDefault(ev => ev.EventId == eventId);
            if (eventToRemove != null)
            {
                events.Remove(eventToRemove);
            }
        }
    }

    public class UserEventViewModel
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public ObservableCollection<Recipes> Recipes { get; set; }
    }
}
