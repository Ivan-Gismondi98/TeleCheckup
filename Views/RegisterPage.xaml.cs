using System;
using Microsoft.Maui.Controls;

namespace TeleCheckup.Views
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private async void OnRegisterSubmit(object sender, EventArgs e)
        {
            var name = NameEntry.Text?.Trim();
            var email = EmailEntry.Text?.Trim();
            var password = PasswordEntry.Text;
            var confirm = ConfirmPasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Errore", "Compila tutti i campi.", "OK");
                return;
            }

            if (password != confirm)
            {
                await DisplayAlert("Errore", "Le password non coincidono.", "OK");
                return;
            }

            await DisplayAlert("Registrazione", "Registrazione completata con successo.", "OK");
            await Shell.Current.GoToAsync("//DashboardPage");
        }
    }
}
