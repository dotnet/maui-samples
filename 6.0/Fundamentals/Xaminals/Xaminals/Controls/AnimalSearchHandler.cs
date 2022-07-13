using Xaminals.Models;

namespace Xaminals.Controls
{
    public class AnimalSearchHandler : SearchHandler
    {
        public IList<Animal> Animals { get; set; }
        public Type SelectedItemNavigationTarget { get; set; }

        protected override void OnQueryChanged(string oldValue, string newValue)
        {
            base.OnQueryChanged(oldValue, newValue);

            if (string.IsNullOrWhiteSpace(newValue))
            {
                ItemsSource = null;
            }
            else
            {
                ItemsSource = Animals
                    .Where(animal => animal.Name.ToLower().Contains(newValue.ToLower()))
                    .ToList<Animal>();
            }
        }

        protected override async void OnItemSelected(object item)
        {
            base.OnItemSelected(item);

            Animal animal = item as Animal;
            string navigationTarget = GetNavigationTarget();

            if (navigationTarget.Equals("catdetails") || navigationTarget.Equals("dogdetails"))
            {
                await Shell.Current.GoToAsync($"{navigationTarget}?name={((Animal)item).Name}");
            }
            else if (navigationTarget.Equals("monkeydetails"))
            {
                var navigationParameters = new Dictionary<string, object>
                {
                    { "Monkey", animal }
                };
                await Shell.Current.GoToAsync($"{navigationTarget}", navigationParameters);
            }
            else if (navigationTarget.Equals("elephantdetails"))
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "Elephant", animal }
                };

                await Shell.Current.GoToAsync($"{navigationTarget}?name={animal.Name}", navigationParameter);
            }
            else if (navigationTarget.Equals("beardetails"))
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "Bear", animal }
                };
                await Shell.Current.GoToAsync($"{navigationTarget}", navigationParameter);
            }
        }

        string GetNavigationTarget()
        {
            return (Shell.Current as AppShell).Routes.FirstOrDefault(route => route.Value.Equals(SelectedItemNavigationTarget)).Key;
        }
    }
}
