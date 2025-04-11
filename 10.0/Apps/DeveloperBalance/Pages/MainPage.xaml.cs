using DeveloperBalance.Models;
using DeveloperBalance.PageModels;

namespace DeveloperBalance.Pages;

public partial class MainPage : ContentPage
{
	public MainPage(MainPageModel model)
	{
		InitializeComponent();
		BindingContext = model;
	}
}