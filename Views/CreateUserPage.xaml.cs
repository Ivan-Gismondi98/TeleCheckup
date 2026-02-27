using System;
using Microsoft.Maui.Controls;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Firestore;

namespace TeleCheckup.Views
{
    public partial class CreateUserPage : ContentPage
    {
        public CreateUserPage()
        {
            InitializeComponent();
            RolePicker.SelectedIndex = 0;
        }

        private async void OnCreateUserSubmit(object sender, EventArgs e)
        {
            var name = NameEntry.Text?.Trim();
            var email = EmailEntry.Text?.Trim();
            var password = PasswordEntry.Text;
            var ruolo = RolePicker.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(ruolo))
            {
                await DisplayAlertAsync("Errore", "Compila tutti i campi.", "OK");
                return;
            }

            try
            {
                // Crea utente tramite Firebase REST API
                var apiKey = "INSERISCI_LA_TUA_API_KEY_FIREBASE";
                var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={apiKey}";
                var payload = new System.Collections.Generic.Dictionary<string, object>
                {
                    { "email", email },
                    { "password", password },
                    { "returnSecureToken", true }
                };
                var json = System.Text.Json.JsonSerializer.Serialize(payload);
                using var client = new System.Net.Http.HttpClient();
                var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    await DisplayAlertAsync("Errore", $"Creazione utente fallita: {responseString}", "OK");
                    return;
                }
                var result = System.Text.Json.JsonDocument.Parse(responseString);
                var localId = result.RootElement.GetProperty("localId").GetString();
                if (string.IsNullOrWhiteSpace(localId))
                {
                    await DisplayAlertAsync("Errore", "Creazione utente fallita.", "OK");
                    return;
                }

                // Salva dati su Firestore
                var firestore = Plugin.Firebase.Firestore.CrossFirebaseFirestore.Current;
                var userDoc = firestore.GetCollection("utenti").GetDocument(localId);
                await userDoc.SetDataAsync(new System.Collections.Generic.Dictionary<string, object>
                {
                    { "nome", name },
                    { "email", email },
                    { "ruolo", ruolo },
                    { "creatoIl", DateTime.UtcNow }
                });

                await DisplayAlertAsync("Successo", $"Utente creato con ruolo '{ruolo}'.", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Errore", $"Creazione utente fallita: {ex.Message}", "OK");
            }
        }
    }
}
