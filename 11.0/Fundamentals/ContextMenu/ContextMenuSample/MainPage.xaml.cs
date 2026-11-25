namespace ContextMenuSample;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    void OnClear(System.Object sender, System.EventArgs e)
    {
        commentField.Text = string.Empty;
    }

    void OnDecrease(System.Object sender, System.EventArgs e)
    {
        commentField.FontSize -= 1;
    }

    void OnIncrease(System.Object sender, System.EventArgs e)
    {
        commentField.FontSize += 1;
    }
}


