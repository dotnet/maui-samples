using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls;

namespace TiltMaze;

public class EllipseView : GraphicsView, IDrawable
{
    public static readonly BindableProperty ColorProperty =
        BindableProperty.Create(
            nameof(Color),
            typeof(Color),
            typeof(EllipseView),
            Colors.Transparent,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                if (bindable is EllipseView ev)
                    ev.Invalidate();
            });

    public Color Color
    {
        set { SetValue(ColorProperty, value); }
        get { return (Color)GetValue(ColorProperty); }
    }

    public EllipseView()
    {
        Drawable = this;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = Color;
        canvas.FillEllipse(dirtyRect);
    }
}
