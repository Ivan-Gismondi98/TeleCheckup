using TeleCheckup.Views;

namespace TeleCheckup.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnPatientLoginClicked(object sender, EventArgs e)
    {
        // Naviga alla dashboard paziente
        await Shell.Current.GoToAsync($"//{nameof(PatientDashboardPage)}");
    }
    private async void OnDoctorLoginClicked(object sender, EventArgs e)
    {
        // Naviga alla dashboard medico
        await Shell.Current.GoToAsync($"//{nameof(DoctorDashboardPage)}");
    }
    private async void OnAdminLoginClicked(object sender, EventArgs e)
    {
        // Naviga alla dashboard admin
        await Shell.Current.GoToAsync($"//{nameof(AdminDashboardPage)}");
    }
}