using Microsoft.Maui.Layouts;

namespace CustomLayoutDemos.Layouts
{
    public class CascadeLayout : StackLayout
    {
        protected override ILayoutManager CreateLayoutManager()
        {
            return new CascadeLayoutManager(this);
        }
    }
}

