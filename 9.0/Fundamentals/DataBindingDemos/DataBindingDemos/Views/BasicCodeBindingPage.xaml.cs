﻿namespace DataBindingDemos
{
    public partial class BasicCodeBindingPage : ContentPage
    {
        public BasicCodeBindingPage()
        {
            InitializeComponent();

            label.BindingContext = slider;
            label.SetBinding(Label.RotationProperty, static (Slider slider) => slider.Value);
        }
    }
}