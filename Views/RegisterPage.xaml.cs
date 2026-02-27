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

            try
            {
                // Crea utente su Firebase Auth
                var authResult = await Plugin.Firebase.Auth.CrossFirebaseAuth.Current.Instance.CreateUserWithEmailAndPasswordAsync(email, password);
                var user = authResult.User;
                if (user == null)
                {
                    await DisplayAlert("Errore", "Registrazione fallita.", "OK");
                    return;
                }

                // Salva dati su Firestore
                var firestore = Plugin.Firebase.Firestore.CrossFirebaseFirestore.Current.Instance;
                var userDoc = firestore.Collection("utenti").Document(user.Uid);
                await userDoc.SetAsync(new System.Collections.Generic.Dictionary<string, object>
                {
                    { "nome", name },
                    { "email", email },
                    { "ruolo", "paziente" },
                    { "creatoIl", DateTime.UtcNow }
                });

                await DisplayAlert("Registrazione", "Registrazione completata con successo.", "OK");
                await Shell.Current.GoToAsync("//DashboardPage");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Errore", $"Registrazione fallita: {ex.Message}", "OK");
            }
        }
    }
}
