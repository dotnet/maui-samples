using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamarinCustomRenderer
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        void Handle_Pressed(object sender, System.EventArgs e)
        {
            (sender as VisualElement).FadeTo(0.7, 100);
        }

        void Handle_Released(object sender, System.EventArgs e)
        {
            (sender as VisualElement).FadeTo(1, 200);
        }
    }
}
