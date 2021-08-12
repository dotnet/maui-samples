using System;
using Android.App;
using Android.Runtime;
using Microsoft.Maui;

namespace HelloMaui
{
	[Application]
	public class MainApplication : MauiApplication<Startup>
	{
		public MainApplication(IntPtr handle, JniHandleOwnership ownerShip) : base(handle, ownerShip)
		{
		}
	}
}