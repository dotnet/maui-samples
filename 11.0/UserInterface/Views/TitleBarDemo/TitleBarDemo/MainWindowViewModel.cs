namespace TitleBarDemo;

public class MainWindowViewModel : BaseViewModel
{
    private string _title = "My App";
    public string Title
    {
        get { return _title; }
        set
        {
            _title = value;
            OnPropertyChanged();
        }
    }

    private string _subtitle = "TitleBar demo";
    public string Subtitle
    {
        get { return _subtitle; }
        set
        {
            _subtitle = value;
            OnPropertyChanged();
        }
    }

    private bool _showTitleBar = true;
    public bool ShowTitleBar
    {
        get { return _showTitleBar; }
        set
        {
            _showTitleBar = value;
            OnPropertyChanged();
        }
    }
}
