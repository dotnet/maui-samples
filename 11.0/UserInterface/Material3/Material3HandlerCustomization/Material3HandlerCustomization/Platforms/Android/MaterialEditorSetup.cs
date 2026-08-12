#if ANDROID
using Android.Views;
using Android.Views.InputMethods;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace Material3HandlerCustomization;

internal static class MaterialEditorSetup
{
    // <docregion_platformviewfactory>
    public static void RegisterCustomEditor()
    {
        EditorHandler2.PlatformViewFactory = handler =>
        {
            var context = MauiMaterialContextThemeWrapper.Create(handler.Context!);
            var editText = new MyMaterialEditText(context)
            {
                ImeOptions = ImeAction.Done,
                Gravity = GravityFlags.Top,
                TextAlignment = global::Android.Views.TextAlignment.ViewStart,
            };
            editText.SetSingleLine(false);
            editText.SetHorizontallyScrolling(false);
            return editText;
        };
    }
    // </docregion_platformviewfactory>
}
#endif
