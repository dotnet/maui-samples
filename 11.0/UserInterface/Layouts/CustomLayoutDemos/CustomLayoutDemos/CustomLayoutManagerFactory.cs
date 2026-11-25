using Microsoft.Maui.Layouts;
using CustomLayoutDemos.Layouts;

namespace CustomLayoutDemos
{
    public class CustomLayoutManagerFactory : ILayoutManagerFactory
    {
        public ILayoutManager CreateLayoutManager(Layout layout)
        {
            if (layout is Grid)
            {
                return new CustomGridLayoutManager(layout as IGridLayout);
            }
            return null;
        }
    }
}

