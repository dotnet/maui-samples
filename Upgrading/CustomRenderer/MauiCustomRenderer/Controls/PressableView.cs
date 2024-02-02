using System;
using Microsoft.Maui.Controls;

namespace XamarinCustomRenderer.Controls
{
    public class PressableView : ContentView
    {
        public event EventHandler Pressed;
        public event EventHandler Released;

        public bool IsPressable => Pressed != null || Released != null;

        public PressableView()
        {
        }

        public void RaisePressed() => Pressed?.Invoke(this, EventArgs.Empty);
        public void RaiseReleased() => Released?.Invoke(this, EventArgs.Empty);
    }
}

