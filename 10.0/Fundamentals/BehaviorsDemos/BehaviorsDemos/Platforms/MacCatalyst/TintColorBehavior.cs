using Microsoft.Maui.Platform;
using UIKit;

namespace BehaviorsDemos
{
    public partial class TintColorBehavior : PlatformBehavior<Image, UIImageView>
    {
        protected override void OnAttachedTo(Image bindable, UIImageView platformView)
        {
            base.OnAttachedTo(bindable, platformView);

            if (platformView is null || platformView.Image is null)
                return;
            if (TintColor is null)
                ClearColor(platformView);
            else
                ApplyColor(platformView, TintColor);
        }

        protected override void OnDetachedFrom(Image bindable, UIImageView platformView)
        {
            base.OnDetachedFrom(bindable, platformView);

            if (platformView is null)
                return;
            ClearColor(platformView);
        }

        void ApplyColor(UIImageView imageView, Color color)
        {
            imageView.Image = imageView.Image?.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            imageView.TintColor = color.ToPlatform();
        }

        void ClearColor(UIImageView imageView)
        {
            imageView.Image = imageView.Image?.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
        }
    }
}
