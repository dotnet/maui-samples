using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace CustomRenderer
{
	public class App : Application
	{
		public App ()
		{	
			//MainPage = new MainPage ();   // uncomment this to test the code-behind version
			MainPage = new MainPageXaml (); 
		}
	}
}

