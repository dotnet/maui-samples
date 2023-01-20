using Android.Content;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XamarinCustomRenderer.Controls;
using XamarinCustomRenderer.Droid.Renderers;

[assembly: ExportRenderer(typeof(PressableView), typeof(PressableViewRenderer))]
namespace XamarinCustomRenderer.Droid.Renderers
{
    public class PressableViewRenderer : VisualElementRenderer<PressableView>
    {
        public PressableViewRenderer(Context context) : base(context)
        {
            Touch += Control_Touch;
        }

        private void Control_Touch(object sender, TouchEventArgs e)
        {
            switch (e.Event.Action)
            {
                case MotionEventActions.Down:
                    Element?.RaisePressed();
                    break;

                case MotionEventActions.Up:
                    Element?.RaiseReleased();
                    break;

                default:
                    break;
            }

        }
    }
}
