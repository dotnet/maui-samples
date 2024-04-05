using System;
using CommunityToolkit.Maui.Behaviors;

namespace PointOfSale.Common.Behaviors;

public partial class CursorBehavior : PlatformBehavior<View>
{
    public static readonly BindableProperty AttachBehaviorProperty =
            BindableProperty.CreateAttached("AttachBehavior", typeof(bool), typeof(CursorBehavior), false, propertyChanged: OnAttachBehaviorChanged);

    public static bool GetAttachBehavior(BindableObject view)
    {
        return (bool)view.GetValue(AttachBehaviorProperty);
    }

    public static void SetAttachBehavior(BindableObject view, bool value)
    {
        view.SetValue(AttachBehaviorProperty, value);
    }

    static void OnAttachBehaviorChanged(BindableObject view, object oldValue, object newValue)
    {
        var btn = view as Button;
        if (btn == null)
        {
            return;
        }

        bool attachBehavior = (bool)newValue;
        if (attachBehavior)
        {
            btn.Behaviors.Add(new CursorBehavior());
        }
        else
        {
            var toRemove = btn.Behaviors.FirstOrDefault(b => b is CursorBehavior);
            if (toRemove != null)
            {
                btn.Behaviors.Remove(toRemove);
            }
        }
    }
}