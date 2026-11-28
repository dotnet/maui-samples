namespace DataBindingDemos.Controls
{
    public class Separator : BoxView
    {
        public Separator()
        {
            this.SetAppThemeColor(ColorProperty, Color.FromArgb("#E3E0EC"), Color.FromArgb("#353146"));
            HeightRequest = 1;
        }
    }
}
