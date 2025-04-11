using System.Collections.ObjectModel;

namespace DataBindingDemos
{
    public class MultiBindingConverterCodePage : ContentPage
    {
        public MultiBindingConverterCodePage()
        {
            BindingContext = new GroupViewModel();

            Grid grid = new Grid { Margin = new Thickness(20) };

            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.75, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.25, GridUnitType.Star) });

            grid.Add(new Label { Text = "Employee", FontAttributes = FontAttributes.Bold });
            grid.Add(new Label { Text = "Can drive?", FontAttributes = FontAttributes.Bold }, 1, 0);

            Label employee1 = new Label { VerticalTextAlignment = TextAlignment.Center };
            employee1.SetBinding(Label.TextProperty, static (GroupViewModel vm) => vm.Employee1.FullName);

            CheckBox checkBox1 = new CheckBox { HorizontalOptions = LayoutOptions.End };
            checkBox1.SetBinding(CheckBox.IsCheckedProperty, new MultiBinding
            {
                Bindings = new Collection<BindingBase>
                {
                    Binding.Create(static (GroupViewModel vm) => vm.Employee1.IsOver16),
                    Binding.Create(static (GroupViewModel vm) => vm.Employee1.HasPassedTest),
                    Binding.Create(static (GroupViewModel vm) => vm.Employee1.IsSuspended, converter: new InverterConverter())
                },
                Converter = new AllTrueMultiConverter()
            });

            Label employee2 = new Label { VerticalTextAlignment = TextAlignment.Center };
            employee2.SetBinding(Label.TextProperty, static (GroupViewModel vm) => vm.Employee2.FullName);

            CheckBox checkBox2 = new CheckBox { HorizontalOptions = LayoutOptions.End };
            checkBox2.SetBinding(CheckBox.IsCheckedProperty, new MultiBinding
            {
                Bindings = new Collection<BindingBase>
                {
                    Binding.Create(static (GroupViewModel vm) => vm.Employee2.IsOver16),
                    Binding.Create(static (GroupViewModel vm) => vm.Employee2.HasPassedTest),
                    Binding.Create(static (GroupViewModel vm) => vm.Employee2.IsSuspended, converter: new InverterConverter())
                },
                Converter = new AllTrueMultiConverter()
            });

            Label employee3 = new Label { VerticalTextAlignment = TextAlignment.Center };
            employee3.SetBinding(Label.TextProperty, static (GroupViewModel vm) => vm.Employee3.FullName);

            CheckBox checkBox3 = new CheckBox { HorizontalOptions = LayoutOptions.End };
            checkBox3.SetBinding(CheckBox.IsCheckedProperty, new MultiBinding
            {
                Bindings = new Collection<BindingBase>
                {
                    Binding.Create(static (GroupViewModel vm) => vm.Employee3.IsOver16),
                    Binding.Create(static (GroupViewModel vm) => vm.Employee3.HasPassedTest),
                    Binding.Create(static (GroupViewModel vm) => vm.Employee3.IsSuspended, converter: new InverterConverter())
                },
                Converter = new AllTrueMultiConverter()
            });

            Label employee4 = new Label { VerticalTextAlignment = TextAlignment.Center };
            employee4.SetBinding(Label.TextProperty, static (GroupViewModel vm) => vm.Employee4.FullName);

            CheckBox checkBox4 = new CheckBox { HorizontalOptions = LayoutOptions.End };
            checkBox4.SetBinding(CheckBox.IsCheckedProperty, new MultiBinding
            {
                Bindings = new Collection<BindingBase>
                {
                    Binding.Create(static (GroupViewModel vm) => vm.Employee4.IsOver16),
                    Binding.Create(static (GroupViewModel vm) => vm.Employee4.HasPassedTest),
                    Binding.Create(static (GroupViewModel vm) => vm.Employee4.IsSuspended, converter: new InverterConverter())
                },
                Converter = new AllTrueMultiConverter()
            });

            Label employee5 = new Label { VerticalTextAlignment = TextAlignment.Center };
            employee5.SetBinding(Label.TextProperty, static (GroupViewModel vm) => vm.Employee5.FullName);

            CheckBox checkBox5 = new CheckBox { HorizontalOptions = LayoutOptions.End };
            checkBox5.SetBinding(CheckBox.IsCheckedProperty, new MultiBinding
            {
                Bindings = new Collection<BindingBase>
                {
                    Binding.Create(static (GroupViewModel vm) => vm.Employee5.IsOver16),
                    Binding.Create(static (GroupViewModel vm) => vm.Employee5.HasPassedTest),
                    Binding.Create(static (GroupViewModel vm) => vm.Employee5.IsSuspended, converter: new InverterConverter())
                },
                Converter = new AllTrueMultiConverter()
            });

            grid.Add(employee1, 0, 1);
            grid.Add(checkBox1, 1, 1);
            grid.Add(employee2, 0, 2);
            grid.Add(checkBox2, 1, 2);
            grid.Add(employee3, 0, 3);
            grid.Add(checkBox3, 1, 3);
            grid.Add(employee4, 0, 4);
            grid.Add(checkBox4, 1, 4);
            grid.Add(employee5, 0, 5);
            grid.Add(checkBox5, 1, 5);

            Title = "MultiBinding converter demo";
            Content = grid;
        }
    }
}
