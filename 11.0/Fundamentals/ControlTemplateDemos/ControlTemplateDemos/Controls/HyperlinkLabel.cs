namespace ControlTemplateDemos.Controls
{
    public class HyperlinkLabel : Label
    {
        public static readonly BindableProperty UrlProperty =
            BindableProperty.Create(nameof(Url), typeof(string), typeof(HyperlinkLabel), null);

        public string Url
        {
            get => (string)GetValue(UrlProperty);
            set => SetValue(UrlProperty, value);
        }

        public HyperlinkLabel()
        {
            GestureRecognizers.Add(new TapGestureRecognizer
            {
                // Launcher.OpenAsync is provided by Essentials
                Command = new Command(async () => await Launcher.OpenAsync(Url))
            });
        }
    }
}
