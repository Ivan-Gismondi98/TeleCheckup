using Microsoft.Maui.Controls;
namespace TeleCheckup.Views;

public partial class PatientDashboardPage : ContentPage
{
    public PatientDashboardPage()
    {
        InitializeComponent();
    }

    private async void OnPharmaCheckerTapped(object sender, EventArgs e)
    {
        // Navigazione a una pagina PharmaChecker (stub, da implementare)
        await Shell.Current.DisplayAlert("Pharma Checker", "Funzionalità non ancora implementata.", "OK");
    }

    private async void OnTelevisitaTapped(object sender, EventArgs e)
    {
        // Navigazione a VideoCallPage
        await Shell.Current.GoToAsync(nameof(VideoCallPage));
    }
}