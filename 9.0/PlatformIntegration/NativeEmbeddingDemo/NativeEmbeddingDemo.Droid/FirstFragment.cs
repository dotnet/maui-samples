using Android.Runtime;
using Android.Views;
using AndroidX.Navigation.Fragment;
using Button = Android.Widget.Button;
using Fragment = AndroidX.Fragment.App.Fragment;
using View = Android.Views.View;
using static Android.Views.ViewGroup.LayoutParams;

namespace NativeEmbeddingDemo.Droid;

[Register("com.companyname.nativeembeddingdemo." + nameof(FirstFragment))]
public class FirstFragment : Fragment
{
    MyMauiContent? _mauiView;
    Android.Views.View? _nativeView;

    public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState) =>
        inflater.Inflate(Resource.Layout.fragment_first, container, false);

    public override void OnViewCreated(View view, Bundle? savedInstanceState)
    {
        base.OnViewCreated(view, savedInstanceState);

        // Create Android button
        var androidButton = new Android.Widget.Button(this);
        androidButton.Text = "Android button above .NET MAUI controls";
        androidButton.Click += OnAndroidButtonClicked;
        rootLayout.AddView(androidButton, new LinearLayout.LayoutParams(MatchParent, WrapContent));

    }

    public override void OnDestroyView()
    {
        base.OnDestroyView();
    }

    async void OnAndroidButtonClicked(object? sender, EventArgs e)
    {
        if (_mauiView?.DotNetBot is not Image bot)
            return;

        await bot.RotateTo(360, 1000);
        bot.Rotation = 0;

        bot.HeightRequest = 90;
    }

}