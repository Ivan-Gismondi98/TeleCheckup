using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;
using Plugin.Firebase.Firestore;

namespace TeleCheckup.Views
{
    public partial class VisualizzaRichiestePage : ContentPage
    {
        public ObservableCollection<RichiestaVisitaModel> Richieste { get; set; } = new();
        public VisualizzaRichiestePage()
        {
            InitializeComponent();
            RichiesteCollectionView.ItemsSource = Richieste;
            LoadRichieste();
        }

        private async void LoadRichieste()
        {
            var auth = Plugin.Firebase.Auth.CrossFirebaseAuth.Current;
            var pazienteId = auth.CurrentUser?.Uid;
            if (string.IsNullOrWhiteSpace(pazienteId)) return;
            var firestore = Plugin.Firebase.Firestore.CrossFirebaseFirestore.Current;
            var snapshot = await firestore.GetCollection("richieste_visite").GetDocumentsAsync<object>();
            Richieste.Clear();
            foreach (var doc in snapshot.Documents)
            {
                var data = doc.Data as Dictionary<string, object>;
                if (data != null && data.ContainsKey("paziente_id") && data["paziente_id"]?.ToString() == pazienteId)
                {
                    Richieste.Add(new RichiestaVisitaModel
                    {
                        Id = doc.Reference.Id,
                        MedicoId = data.ContainsKey("medico_id") ? data["medico_id"]?.ToString() : "",
                        MedicoNome = await GetMedicoNome((data.ContainsKey("medico_id") ? data["medico_id"]?.ToString() : string.Empty) ?? string.Empty),
                        Data = data.ContainsKey("data") ? data["data"]?.ToString() : "",
                        TipoVisita = data.ContainsKey("tipo_visita") ? data["tipo_visita"]?.ToString() : "",
                        Stato = data.ContainsKey("stato") ? data["stato"]?.ToString() : "",
                        Note = data.ContainsKey("note") ? data["note"]?.ToString() : "",
                        PazienteId = data.ContainsKey("paziente_id") ? data["paziente_id"]?.ToString() : ""
                    });
                }
            }
        }

        private async Task<string> GetMedicoNome(string medicoId)
        {
            if (string.IsNullOrWhiteSpace(medicoId)) return "";
            var firestore = Plugin.Firebase.Firestore.CrossFirebaseFirestore.Current;
            var doc = await Plugin.Firebase.Firestore.CrossFirebaseFirestore.Current.GetDocument($"utenti/{medicoId}").GetDocumentSnapshotAsync<object>();
            if (doc?.Data is Dictionary<string, object> medicoData && medicoData.ContainsKey("nome") && medicoData["nome"] != null)
                return medicoData["nome"]?.ToString() ?? string.Empty;
            return string.Empty;
            // ...existing code...
        }

        private async void OnModificaRichiestaClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is RichiestaVisitaModel richiesta)
            {
                await Navigation.PushAsync(new ModificaRichiestaPage(richiesta));
            }
        }

        private async void OnEliminaRichiestaClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is RichiestaVisitaModel richiesta)
            {
                    var confirm = await DisplayAlertAsync("Conferma", $"Eliminare la richiesta per il {richiesta.Data}?", "Sì", "No");
                if (confirm)
                {
                    var firestore = Plugin.Firebase.Firestore.CrossFirebaseFirestore.Current;
                    await firestore.GetCollection("richieste_visite").GetDocument(richiesta.Id).DeleteDocumentAsync();
                    LoadRichieste();
                }
            }
        }
    }

    public class RichiestaVisitaModel
    {
        public string? Id { get; set; }
        public string? MedicoId { get; set; }
        public string? MedicoNome { get; set; }
        public string? Data { get; set; }
        public string? TipoVisita { get; set; }
        public string? Stato { get; set; }
        public string? Note { get; set; }
        public string? PazienteId { get; set; }
    }
}
