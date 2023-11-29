using Microsoft.Maui.Layouts;

namespace CustomLayoutDemos.Layouts
{
    public class ZStackLayout : StackBase
    {
        protected override ILayoutManager CreateLayoutManager() => new ZStackLayoutManager(this);
    }
}

