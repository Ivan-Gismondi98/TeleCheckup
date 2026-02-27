using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;
using Plugin.Firebase.Firestore;

namespace TeleCheckup.Views
{
    public partial class GestioneRichiestePage : ContentPage
    {
        private readonly string _tipoRichiesta;
        private readonly string _collezioneFirestore;
        private readonly string _campoTipo;
        private readonly bool _isTelevisita;
        public ObservableCollection<RichiestaGenericaModel> Richieste { get; set; } = new();

        public GestioneRichiestePage(string tipoRichiesta)
        {
            InitializeComponent();
            _tipoRichiesta = tipoRichiesta.ToLower();
            _collezioneFirestore = _tipoRichiesta switch
            {
                "visita" => "richieste_visite",
                "analisi" => "richieste_analisi",
                "fisioterapia" => "richieste_fisioterapie",
                "televisita" => "richieste_televisite",
                _ => "richieste_generiche"
            };
            _campoTipo = _tipoRichiesta switch
            {
                "visita" => "tipo_visita",
                "analisi" => "tipo_analisi",
                "fisioterapia" => "tipo_fisioterapia",
                "televisita" => "tipo_televisita",
                _ => "tipo"
            };
            _isTelevisita = _tipoRichiesta == "televisita";
            TitoloLabel.Text = $"Le tue richieste di {_tipoRichiesta}";
            RichiesteCollectionView.ItemsSource = Richieste;
            LoadRichieste();
        }

        private async void LoadRichieste()
        {
            var auth = Plugin.Firebase.Auth.CrossFirebaseAuth.Current;
            var pazienteId = auth.CurrentUser?.Uid;
            if (string.IsNullOrWhiteSpace(pazienteId)) return;
            var firestore = Plugin.Firebase.Firestore.CrossFirebaseFirestore.Current;
            var snapshot = await firestore.GetCollection(_collezioneFirestore).GetDocumentsAsync<object>();
            Richieste.Clear();
            foreach (var doc in snapshot.Documents)
            {
                var data = doc.Data as Dictionary<string, object>;
                if (data != null && data.ContainsKey("paziente_id") && data["paziente_id"]?.ToString() == pazienteId)
                {
                    Richieste.Add(new RichiestaGenericaModel
                    {
                        Id = doc.Reference.Id,
                        MedicoId = data.ContainsKey("medico_id") ? data["medico_id"]?.ToString() : "",
                        MedicoNome = await GetMedicoNome((data.ContainsKey("medico_id") ? data["medico_id"]?.ToString() : string.Empty) ?? string.Empty),
                        Data = data.ContainsKey("data") ? data["data"]?.ToString() : "",
                        Tipo = data.ContainsKey(_campoTipo) ? data[_campoTipo]?.ToString() : "",
                        Stato = data.ContainsKey("stato") ? data["stato"]?.ToString() : "",
                        Note = data.ContainsKey("note") ? data["note"]?.ToString() : "",
                        LinkVideo = _isTelevisita && data.ContainsKey("link_video") ? data["link_video"]?.ToString() : "",
                        IsTelevisita = _isTelevisita
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
        }

        private async void OnNuovaRichiestaClicked(object sender, EventArgs e)
        {
            // TODO: Implementa NuovaRichiestaPage per la creazione richiesta generica
            // await Navigation.PushAsync(new NuovaRichiestaPage(_tipoRichiesta));
        }

        private async void OnModificaRichiestaClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is RichiestaGenericaModel richiesta)
            {
                // TODO: Implementa ModificaRichiestaGenericaPage per la modifica richiesta generica
                // await Navigation.PushAsync(new ModificaRichiestaGenericaPage(_tipoRichiesta, richiesta));
            }
        }

        private async void OnEliminaRichiestaClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is RichiestaGenericaModel richiesta)
            {
                var confirm = await DisplayAlertAsync("Conferma", $"Eliminare la richiesta per il {richiesta.Data}?", "Sì", "No");
                if (confirm)
                {
                    var firestore = Plugin.Firebase.Firestore.CrossFirebaseFirestore.Current;
                    await firestore.GetCollection(_collezioneFirestore).GetDocument(richiesta.Id).DeleteDocumentAsync();
                    LoadRichieste();
                }
            }
        }
    }

    public class RichiestaGenericaModel
    {
        public string? Id { get; set; }
        public string? MedicoId { get; set; }
        public string? MedicoNome { get; set; }
        public string? Data { get; set; }
        public string? Tipo { get; set; }
        public string? Stato { get; set; }
        public string? Note { get; set; }
        public string? LinkVideo { get; set; }
        public bool IsTelevisita { get; set; }
    }
}
