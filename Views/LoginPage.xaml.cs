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
            await ShowLoginAndAuthenticate("paziente");
        }


        public async void OnDoctorClicked(object sender, EventArgs e)
        {
            await ShowLoginAndAuthenticate("medico");
        }


        public async void OnAdminClicked(object sender, EventArgs e)
        {
            await ShowLoginAndAuthenticate("admin");
        }
        // Metodo generico per mostrare popup login e autenticare con Firebase
        private async Task ShowLoginAndAuthenticate(string ruolo)
        {
            string email = await Application.Current.MainPage.DisplayPromptAsync($"Login {ruolo}", "Inserisci email:");
            if (string.IsNullOrWhiteSpace(email)) return;
            string password = await Application.Current.MainPage.DisplayPromptAsync($"Login {ruolo}", "Inserisci password:", "OK", "Annulla", "", -1, true);
            if (string.IsNullOrWhiteSpace(password)) return;

            try
            {
                // Autenticazione Firebase
                var authResult = await Plugin.Firebase.Auth.CrossFirebaseAuth.Current.Instance.SignInWithEmailAndPasswordAsync(email, password);
                var user = authResult.User;
                if (user == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Errore", "Autenticazione fallita.", "OK");
                    return;
                }

                // Recupera ruolo da Firestore (o da custom claim, qui esempio Firestore)
                var firestore = Plugin.Firebase.Firestore.CrossFirebaseFirestore.Current.Instance;
                var doc = await firestore.Collection("utenti").Document(user.Uid).GetAsync();
                if (!doc.Exists)
                {
                    await Application.Current.MainPage.DisplayAlert("Errore", "Utente non trovato nel database.", "OK");
                    return;
                }
                var ruoloDb = doc.Get("ruolo")?.ToString();
                if (ruoloDb != ruolo)
                {
                    await Application.Current.MainPage.DisplayAlert("Errore", $"Non hai i permessi per accedere come {ruolo}.", "OK");
                    return;
                }

                // Naviga alla dashboard corretta
                switch (ruolo)
                {
                    case "paziente":
                        await Shell.Current.GoToAsync("//DashboardPage");
                        break;
                    case "medico":
                        await Shell.Current.GoToAsync(nameof(DoctorDashboardPage));
                        break;
                    case "admin":
                        await Shell.Current.GoToAsync(nameof(AdminDashboardPage));
                        break;
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Errore", $"Login fallito: {ex.Message}", "OK");
            }
        }

        public async void OnRegisterClicked(object sender, EventArgs e)
        {
            // Naviga alla pagina di registrazione pazienti
            await Shell.Current.GoToAsync(nameof(RegisterPage));
        }
    }
}