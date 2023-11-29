using Microsoft.Maui.Layouts;

namespace CustomLayoutDemos.Layouts
{
    public class HorizontalWrapLayout : StackLayout
    {
        protected override ILayoutManager CreateLayoutManager()
        {
            return new HorizontalWrapLayoutManager(this);
        }
    }
}

