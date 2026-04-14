namespace InvalidateStyleDemo;

public partial class MainPage : ContentPage
{
    private bool _styleToggled;
    private bool _visualStateToggled;

    public MainPage()
    {
        InitializeComponent();
    }

    // <docregion:invalidate-style>
    private void OnChangeStyleClicked(object? sender, EventArgs e)
    {
        var style = (Style)Resources["StyledButtonStyle"];

        if (!_styleToggled)
        {
            // Mutate existing setter values
            style.Setters[0].Value = Colors.OrangeRed;   // BackgroundColor
            style.Setters[2].Value = 24d;                 // FontSize
        }
        else
        {
            // Revert to original values
            style.Setters[0].Value = Colors.DodgerBlue;
            style.Setters[2].Value = 18d;
        }

        // Reapply the mutated style
        StyledButton.InvalidateStyle();

        _styleToggled = !_styleToggled;
        StyleStatusLabel.Text = _styleToggled
            ? "Current: OrangeRed background, FontSize 24"
            : "Current: DodgerBlue background, FontSize 18";
    }
    // </docregion:invalidate-style>

    // <docregion:invalidate-visual-states>
    private void OnChangeVisualStateClicked(object? sender, EventArgs e)
    {
        var style = (Style)Resources["StatefulLabelStyle"];

        // The VisualStateGroups are in the last setter of the style
        var vsgSetter = style.Setters[^1];
        var groups = (VisualStateGroupList)vsgSetter.Value!;
        var commonStates = groups[0];
        var normalState = commonStates.States[0];
        var pointerOverState = commonStates.States[1];

        if (!_visualStateToggled)
        {
            // Mutate the Normal state background
            normalState.Setters[0].Value = Colors.MistyRose;
            normalState.Setters[1].Value = Colors.DarkRed;

            // Mutate the PointerOver state background
            pointerOverState.Setters[0].Value = Colors.LightGreen;
            pointerOverState.Setters[1].Value = Colors.DarkGreen;
        }
        else
        {
            // Revert to original values
            normalState.Setters[0].Value = Colors.LightGray;
            normalState.Setters[1].Value = Colors.Black;

            pointerOverState.Setters[0].Value = Colors.LightBlue;
            pointerOverState.Setters[1].Value = Colors.DarkBlue;
        }

        // Reapply the mutated visual states
        VisualStateManager.InvalidateVisualStates(StatefulLabel);

        _visualStateToggled = !_visualStateToggled;
        VisualStateStatusLabel.Text = _visualStateToggled
            ? "Normal: MistyRose bg / PointerOver: LightGreen bg"
            : "Normal: LightGray bg / PointerOver: LightBlue bg";
    }
    // </docregion:invalidate-visual-states>
}
