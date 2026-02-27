using System;
using Microsoft.Maui.Controls;

namespace TeleCheckup.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        public async void OnPatientClicked(object sender, EventArgs e)
        {
            // Naviga verso la dashboard rimuovendo il login dalla history
            await Shell.Current.GoToAsync("//DashboardPage");
        }

        public async void OnDoctorClicked(object sender, EventArgs e)
        {
            // Per ora accesso diretto: naviga alla dashboard medico
            await Shell.Current.GoToAsync(nameof(DoctorDashboardPage));
        }

        public async void OnAdminClicked(object sender, EventArgs e)
        {
            // Accesso admin - nella versione base nav direttamente alla pagina admin
            await Shell.Current.GoToAsync(nameof(AdminDashboardPage));
        }

        public async void OnRegisterClicked(object sender, EventArgs e)
        {
            // Naviga alla pagina di registrazione pazienti
            await Shell.Current.GoToAsync(nameof(RegisterPage));
        }
    }
}