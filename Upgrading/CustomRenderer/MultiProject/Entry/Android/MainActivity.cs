using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Microsoft.Maui;
using Microsoft.Maui.Controls;


namespace CustomRenderer.Android
{
	[Activity (Label = "CustomRenderer.Android.Android", Icon = "@drawable/icon", 
		Theme = "@style/MainTheme", MainLauncher = true, 
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : MauiAppCompatActivity
	{
	}

}

