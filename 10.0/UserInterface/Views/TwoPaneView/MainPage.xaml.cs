﻿using Microsoft.Maui.Controls.Foldable;
using Microsoft.Maui.Foldable;

namespace MauiTwoPaneViewDemo;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();

        Pane1Length.ValueChanged += PaneLength_ValueChanged;
        Pane2Length.ValueChanged += PaneLength_ValueChanged;
        PanePriority.ItemsSource = System.Enum.GetValues(typeof(TwoPaneViewPriority));
        TallModeConfiguration.ItemsSource = System.Enum.GetValues(typeof(TwoPaneViewTallModeConfiguration));
        WideModeConfiguration.ItemsSource = System.Enum.GetValues(typeof(TwoPaneViewWideModeConfiguration));

        OnReset(null, null);
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        count++;

        if (count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);
    }

    private void PaneLength_ValueChanged(object sender, Microsoft.Maui.Controls.ValueChangedEventArgs e)
    {
        twoPaneView.Pane1Length = new GridLength(Pane1Length.Value, GridUnitType.Star);
        twoPaneView.Pane2Length = new GridLength(Pane2Length.Value, GridUnitType.Star);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        System.Diagnostics.Debug.Write("TwoPaneViewPage.OnAppearing - hinge angle prepped", "JWM");
        DualScreenInfo.Current.HingeAngleChanged += Current_HingeAngleChanged;
        DualScreenInfo.Current.PropertyChanged += Current_PropertyChanged;

        PanePriority.SelectedIndex = 0;
        TallModeConfiguration.SelectedIndex = 1;
        WideModeConfiguration.SelectedIndex = 1;

        hingeLabel.Text = "Hinge prepped " + await DualScreenInfo.Current.GetHingeAngleAsync();
    }

    private void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        spanLabel.Text += "Spanmode: " + DualScreenInfo.Current.SpanMode;
    }

    protected override void OnDisappearing()
    {
        DualScreenInfo.Current.HingeAngleChanged -= Current_HingeAngleChanged;
        DualScreenInfo.Current.PropertyChanged -= Current_PropertyChanged;
    }
    private void Current_HingeAngleChanged(object sender, HingeAngleChangedEventArgs e)
    {
        System.Diagnostics.Debug.Write("TwoPaneViewPage.Current_HingeAngleChanged - " + e.HingeAngleInDegrees, "JWM");

        hingeLabel.Text = "Hinge angle: " + e.HingeAngleInDegrees + " degrees";
    }

    void OnReset(object sender, System.EventArgs e)
    {
        PanePriority.SelectedIndex = 0;
        Pane1Length.Value = 0.5;
        Pane2Length.Value = 0.5;
        TallModeConfiguration.SelectedIndex = 1;
        WideModeConfiguration.SelectedIndex = 1;
        MinTallModeHeight.Value = 0;
        MinWideModeWidth.Value = 0;
    }
}

