using System;
using AppKit;
using UIKit;

namespace PointOfSale.Common.Behaviors;

public partial class CursorBehavior
{
    UIHoverGestureRecognizer _hoverRecognizer;

    protected override void OnAttachedTo(View bindable, UIView platformView)
    {
        _hoverRecognizer = new UIHoverGestureRecognizer(HandlerHoverDetection);
        platformView.AddGestureRecognizer(_hoverRecognizer);
        base.OnAttachedTo(bindable, platformView);
    }

    protected override void OnDetachedFrom(View bindable, UIView platformView)
    {
        platformView.RemoveGestureRecognizer(_hoverRecognizer);
        _hoverRecognizer.Dispose();
        _hoverRecognizer = null;
        base.OnDetachedFrom(bindable, platformView);
    }

    void HandlerHoverDetection(UIHoverGestureRecognizer args)
    {
        if (args.State == UIGestureRecognizerState.Began ||
            args.State == UIGestureRecognizerState.Changed)
        {
            NSCursor.PointingHandCursor.Set();
        }
        else if (args.State == UIGestureRecognizerState.Ended)
        {
            NSCursor.ArrowCursor.Set();
        }
    }
}
