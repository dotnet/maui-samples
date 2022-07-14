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
                // Navigate, passing a string
                await Shell.Current.GoToAsync($"{navigationTarget}?name={((Animal)item).Name}");
            }
            else
            {
                string lowerCasePropertyName = navigationTarget.Replace("details", string.Empty);
                // Capitalise the property name
                string propertyName = char.ToUpper(lowerCasePropertyName[0]) + lowerCasePropertyName.Substring(1);
                
                var navigationParameters = new Dictionary<string, object>
                {
                    { propertyName, animal }
                };

                // Navigate, passing an object
                await Shell.Current.GoToAsync($"{navigationTarget}", navigationParameters);
            }
        }

        string GetNavigationTarget()
        {
            return (Shell.Current as AppShell).Routes.FirstOrDefault(route => route.Value.Equals(SelectedItemNavigationTarget)).Key;
        }
    }
}
