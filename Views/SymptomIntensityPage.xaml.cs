using System;
using Microsoft.Maui.Controls;

namespace TeleCheckup.Views;

[QueryProperty("Area","area")]
public partial class SymptomIntensityPage : ContentPage
{
    private string _area;
    public string Area
    {
        get => _area;
        set
        {
            _area = value;
            AreaLabel.Text = $"Area: {_area}";
        }
    }

    public SymptomIntensityPage()
    {
        InitializeComponent();
    }

    private async void OnMildClicked(object sender, EventArgs e)
    {
        // Navigate to AI advice page
        await Shell.Current.GoToAsync($"{nameof(AiAdvicePage)}?area={Uri.EscapeDataString(_area)}&severity=mild");
    }

    private async void OnSevereClicked(object sender, EventArgs e)
    {
        // Navigate to chat with doctor
        await Shell.Current.GoToAsync($"{nameof(ChatPage)}?area={Uri.EscapeDataString(_area)}&severity=severe");
    }
}
