using Microsoft.Maui.Layouts;

namespace CustomLayoutDemos.Layouts
{
    public class HorizontalWrapLayout : HorizontalStackLayout
    {
        protected override ILayoutManager CreateLayoutManager()
        {
            return new HorizontalWrapLayoutManager(this);
        }
    }
}
