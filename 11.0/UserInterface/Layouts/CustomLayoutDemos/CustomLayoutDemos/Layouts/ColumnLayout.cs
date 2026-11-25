using Microsoft.Maui.Layouts;

namespace CustomLayoutDemos.Layouts
{
    public class ColumnLayout : VerticalStackLayout
    {
        // We'll use an attached property so we don't have to worry about tracking which items are in "Fill" mode locally.
        public static readonly BindableProperty FillProperty = BindableProperty.CreateAttached("Fill", typeof(bool), typeof(ColumnLayout), false);

        public static bool GetFill(BindableObject bindableObject)
        {
            return (bool)bindableObject.GetValue(FillProperty);
        }

        public static void SetFill(BindableObject bindableObject, bool value)
        {
            bindableObject.SetValue(FillProperty, value);
        }

        protected override ILayoutManager CreateLayoutManager()
        {
            return new ColumnLayoutManager(this);
        }

        // Convenience method for use from the layout manager
        public bool GetFill(IView view)
        {
            if (view is BindableObject bindableObject)
            {
                return GetFill(bindableObject);
            }

            return false;
        }
    }
}

