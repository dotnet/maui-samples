using System.Collections.ObjectModel;

namespace DataBindingDemos
{
    public class MultiBindingStringFormatCodePage : ContentPage
    {
        public MultiBindingStringFormatCodePage()
        {
            BindingContext = new GroupViewModel();

            Grid grid = new Grid { Margin = new Thickness(20) };

            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });            

            Label employee1 = new Label();
            employee1.SetBinding(Label.TextProperty, new MultiBinding
            {
                Bindings = new Collection<BindingBase>
                {
                    Binding.Create(static (GroupViewModel vm) => vm.Employee1.Forename),
                    Binding.Create(static (GroupViewModel vm) => vm.Employee1.MiddleName),
                    Binding.Create(static (GroupViewModel vm) => vm.Employee1.Surname)
                },
                StringFormat = "{0} {1} {2}"
            });

            Label employee2 = new Label();
            employee2.SetBinding(Label.TextProperty, new MultiBinding
            {
                Bindings = new Collection<BindingBase>
                {
                    Binding.Create(static (GroupViewModel vm) => vm.Employee2.Forename),
                    Binding.Create(static (GroupViewModel vm) => vm.Employee2.MiddleName),
                    Binding.Create(static (GroupViewModel vm) => vm.Employee2.Surname)
                },
                StringFormat = "{0} {1} {2}"
            });

            Label employee3 = new Label();
            employee3.SetBinding(Label.TextProperty, new MultiBinding
            {
                Bindings = new Collection<BindingBase>
                {
                    Binding.Create(static (GroupViewModel vm) => vm.Employee3.Forename),
                    Binding.Create(static (GroupViewModel vm) => vm.Employee3.MiddleName),
                    Binding.Create(static (GroupViewModel vm) => vm.Employee3.Surname)
                },
                StringFormat = "{0} {1} {2}"
            });
            Label employee4 = new Label();
            employee4.SetBinding(Label.TextProperty, new MultiBinding
            {
                Bindings = new Collection<BindingBase>
                {
                    Binding.Create(static (GroupViewModel vm) => vm.Employee4.Forename),
                    Binding.Create(static (GroupViewModel vm) => vm.Employee4.MiddleName),
                    Binding.Create(static (GroupViewModel vm) => vm.Employee4.Surname)
                },
                StringFormat = "{0} {1} {2}"
            });

            Label employee5 = new Label();
            employee5.SetBinding(Label.TextProperty, new MultiBinding
            {
                Bindings = new Collection<BindingBase>
                {
                    Binding.Create(static (GroupViewModel vm) => vm.Employee5.Forename),
                    Binding.Create(static (GroupViewModel vm) => vm.Employee5.MiddleName),
                    Binding.Create(static (GroupViewModel vm) => vm.Employee5.Surname)
                },
                StringFormat = "{0} {1} {2}"
            });

            grid.Add(new Label { Text = "Employee", FontAttributes = FontAttributes.Bold });
            grid.Add(employee1, 0, 1);
            grid.Add(employee2, 0, 2);
            grid.Add(employee3, 0, 3);
            grid.Add(employee4, 0, 4);
            grid.Add(employee5, 0, 5);

            Title = "MultiBinding code demo";
            Content = grid;
        }
    }
}
