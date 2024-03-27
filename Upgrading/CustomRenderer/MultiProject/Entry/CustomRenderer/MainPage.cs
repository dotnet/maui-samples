using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace CustomRenderer
{
	public class MainPage : ContentPage
	{
		public MainPage ()
		{
			Content = new StackLayout {
				Children = {
					new Label {
						Text = "Hello, Custom Renderer !",
					}, 
					new MyEntry {
						Text = "In Shared Code",
					}
				},
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center,
			};
		}
	}
}

