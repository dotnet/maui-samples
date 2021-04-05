using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Compatibility.Platform.UWP;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;
using Microsoft.Maui.Hosting;
using System;
using System.Diagnostics;

namespace HelloMaui
{
	public class Application : Microsoft.Maui.Controls.Application
	{
		public Application(IServiceProvider services)
		{
			Services = services;
		}

		public IServiceProvider Services { get; }

        public override IWindow CreateWindow(IActivationState activationState)
        {            
            Forms.Init(activationState);

            this.On<Microsoft.Maui.Controls.PlatformConfiguration.Windows>()
                .SetImageDirectory("Assets");

            return Services.GetRequiredService<IWindow>();
        }
    }
}