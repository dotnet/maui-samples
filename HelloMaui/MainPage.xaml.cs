using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;

namespace HelloMaui
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

		int count = 0;

		private void OnButtonClicked(object sender, EventArgs e)
		{
			count++;
			CounterLabel.Text = $"You clicked {count} times!";
		}
	}
}
