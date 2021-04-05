using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Hosting;
using System;
using System.Diagnostics;

namespace HelloMaui
{
	public class Application : IApplication
	{
		public Application(IServiceProvider services)
		{
			Services = services;
		}

		public IServiceProvider Services { get; }

		public IWindow CreateWindow(IActivationState activationState)
		{
			Forms.Init(activationState);

			return Services.GetRequiredService<IWindow>();
		}
	}
}