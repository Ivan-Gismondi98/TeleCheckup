using System;
using Microsoft.Maui.Controls;
using Plugin.Firebase.Firestore;

namespace TeleCheckup.Views
{
    public partial class EditUserPage : ContentPage
    {
        private readonly UserModel _user;

        public EditUserPage(UserModel user)
        {
            InitializeComponent();
            RolePicker.ItemsSource = new System.Collections.Generic.List<string> { "paziente", "medico", "admin" };
            _user = user;
            NameEntry.Text = user.nome;
            EmailEntry.Text = user.email;
            RolePicker.SelectedItem = user.ruolo;
        }

        private async void OnSaveUserClicked(object sender, EventArgs e)
        {
            var name = NameEntry.Text?.Trim();
            var ruolo = RolePicker.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(ruolo))
            {
                await DisplayAlertAsync("Errore", "Compila tutti i campi.", "OK");
                return;
            }

            try
            {
                var firestore = Plugin.Firebase.Firestore.CrossFirebaseFirestore.Current;
                var userDoc = firestore.GetCollection("utenti").GetDocument(_user.Uid);
                await userDoc.UpdateDataAsync(new System.Collections.Generic.Dictionary<object, object>
                {
                    { "nome", name },
                    { "ruolo", ruolo }
                });

                await DisplayAlertAsync("Successo", "Utente aggiornato.", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Errore", $"Modifica fallita: {ex.Message}", "OK");
            }
        }
    }
}
