using Microsoft.Maui.Controls;

namespace TeleCheckup.Views;

public partial class PharmaCheckerPage : ContentPage
{
    public PharmaCheckerPage()
    {
        InitializeComponent();
    }

    private async void OnBodyPartTapped(object sender, EventArgs e)
    {
        if (sender is Frame f)
        {
            var area = f.ClassId ?? "Sconosciuta";
            // Navigate to intensity page with area param
            await Shell.Current.GoToAsync($"{nameof(SymptomIntensityPage)}?area={Uri.EscapeDataString(area)}");
        }
    }
}
