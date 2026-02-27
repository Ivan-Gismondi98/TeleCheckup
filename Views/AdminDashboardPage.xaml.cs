using Microsoft.Maui.Controls;
namespace TeleCheckup.Views;

public partial class AdminDashboardPage : ContentPage
{
    public AdminDashboardPage()
    {
        InitializeComponent();
    }

    private async void OnSystemBugClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Sistema Bug", "Apri dettagli sistema (placeholder).", "OK");
    }

    private async void OnManageUsersClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Gestione Utenti", "Apri gestione utenti (placeholder).", "OK");
    }

    private async void OnCrudBlogClicked(object sender, EventArgs e)
    {
        await DisplayAlert("CRUD Blog", "Apri CRUD Blog (placeholder).", "OK");
    }

    private async void OnAppPermissionsClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Permessi App", "Apri permessi app (placeholder).", "OK");
    }

    private async void OnStatisticsClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Statistiche", "Apri statistiche (placeholder).", "OK");
    }
}
