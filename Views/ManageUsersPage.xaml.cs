using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Plugin.Firebase.Firestore;

namespace TeleCheckup.Views
{
    public partial class ManageUsersPage : ContentPage
    {
        public ObservableCollection<UserModel> Users { get; set; } = new();

        public ManageUsersPage()
        {
            InitializeComponent();
            UsersCollectionView.ItemsSource = Users;
            LoadUsers();
        }

        private async void LoadUsers()
        {
            Users.Clear();
            var firestore = Plugin.Firebase.Firestore.CrossFirebaseFirestore.Current;
            var snapshot = await Plugin.Firebase.Firestore.CrossFirebaseFirestore.Current.GetCollection("utenti").GetDocumentsAsync<object>();
            foreach (var doc in snapshot.Documents)
            {
                var data = doc.Data as Dictionary<string, object>;
                Users.Add(new UserModel
                {
                    Uid = doc.Reference.Id,
                    nome = data != null && data.ContainsKey("nome") ? data["nome"]?.ToString() : "",
                    email = data != null && data.ContainsKey("email") ? data["email"]?.ToString() : "",
                    ruolo = data != null && data.ContainsKey("ruolo") ? data["ruolo"]?.ToString() : ""
                });
            }
        }

        private async void OnCreateUserClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreateUserPage());
            LoadUsers();
        }

        private async void OnEditUserClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is UserModel user)
            {
                await Navigation.PushAsync(new EditUserPage(user));
                LoadUsers();
            }
        }

        private async void OnDeleteUserClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is UserModel user)
            {
                var confirm = await DisplayAlertAsync("Conferma", $"Eliminare l'utente {user.email}? Questa azione è irreversibile e rimuoverà anche l'accesso Firebase.", "Sì", "No");
                if (confirm)
                {
                    var firestore = Plugin.Firebase.Firestore.CrossFirebaseFirestore.Current;
                    await Plugin.Firebase.Firestore.CrossFirebaseFirestore.Current.GetCollection("utenti").GetDocument(user.Uid).DeleteDocumentAsync();

                    // Elimina anche da Firebase Auth tramite Cloud Function REST
                    try
                    {
                        // Sostituisci con l'URL della tua Cloud Function di cancellazione utente
                        string functionUrl = "https://REGIONE-PROGETTO.cloudfunctions.net/deleteUser";
                        using var client = new System.Net.Http.HttpClient();
                        var response = await client.PostAsync(functionUrl, new System.Net.Http.StringContent($"{{\"uid\":\"{user.Uid}\"}}", System.Text.Encoding.UTF8, "application/json"));
                        if (!response.IsSuccessStatusCode)
                        {
                            await DisplayAlertAsync("Attenzione", $"Utente rimosso dal database, ma non da Firebase Auth. Errore: {response.StatusCode}", "OK");
                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlertAsync("Attenzione", $"Utente rimosso dal database, ma non da Firebase Auth. Errore: {ex.Message}", "OK");
                    }

                    LoadUsers();
                }
            }
        }
    }

    public class UserModel
    {
        public string? Uid { get; set; }
        public string? nome { get; set; }
        public string? email { get; set; }
        public string? ruolo { get; set; }
    }
}
