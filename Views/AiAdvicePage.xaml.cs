using System;
using Microsoft.Maui.Controls;

namespace TeleCheckup.Views;

[QueryProperty("Area","area")]
public partial class AiAdvicePage : ContentPage
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

    public AiAdvicePage()
    {
        InitializeComponent();
    }

    private async void OnBack(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//DashboardPage");
    }
}
