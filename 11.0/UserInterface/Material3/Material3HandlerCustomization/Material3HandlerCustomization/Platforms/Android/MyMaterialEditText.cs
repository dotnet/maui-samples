#if ANDROID
using Android.Content;
using Android.Text;
using Android.Util;
using Java.Lang;
using Microsoft.Maui.Platform;

namespace Material3HandlerCustomization;

// <docregion_custom_edittext>
public class MyMaterialEditText : MauiMaterialEditText
{
    public MyMaterialEditText(Context context) : base(context)
    {
    }

    public MyMaterialEditText(Context context, IAttributeSet? attrs) : base(context, attrs)
    {
    }

    protected override void OnTextChanged(ICharSequence? text, int start, int lengthBefore, int lengthAfter)
    {
        base.OnTextChanged(text, start, lengthBefore, lengthAfter);

        // Force the displayed text to upper case as the user types.
        var current = text?.ToString() ?? string.Empty;
        var upper = current.ToUpperInvariant();
        if (current != upper)
        {
            var cursor = SelectionStart;
            SetText(upper, TextView.BufferType.Editable);
            SetSelection(System.Math.Min(cursor, upper.Length));
        }
    }
}
// </docregion_custom_edittext>
#endif
