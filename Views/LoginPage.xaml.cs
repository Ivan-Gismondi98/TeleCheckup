using TeleCheckup.Views;

namespace TeleCheckup.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnPatientClicked(object sender, EventArgs e)
    {
        // Naviga verso la dashboard rimuovendo il login dalla history
        await Shell.Current.GoToAsync($"//DashboardPage");
    }
}