using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;

namespace TeleCheckup.Views
{
    public partial class RichiestaVisitaPage : ContentPage
    {
        public RichiestaVisitaPage()
        {
            InitializeComponent();
            TipoVisitaPicker.ItemsSource = new System.Collections.Generic.List<string>
            {
                "Cardiologica", "Odontoiatrica", "Fisioterapica", "Analisi del sangue"
            };
            LoadMedici();
        }

        private async void LoadMedici()
        {
            var firestore = Plugin.Firebase.Firestore.CrossFirebaseFirestore.Current;
            var mediciSnapshot = await firestore.Collection("utenti").WhereEqualTo("ruolo", "medico").GetAsync();
            var mediciList = mediciSnapshot.Documents
                .Select(doc => new MedicoModel {
                    Id = doc.Id,
                    Nome = doc.Get("nome")?.ToString() ?? ""
                }).ToList();
            MedicoPicker.ItemsSource = mediciList;
            MedicoPicker.ItemDisplayBinding = new Binding("Nome");
        }

        public class MedicoModel {
            public string Id { get; set; }
            public string Nome { get; set; }
        }

        // Handler placeholder per invio richiesta
        private async void OnInviaRichiestaClicked(object sender, EventArgs e)
        {
            var medico = MedicoPicker.SelectedItem as MedicoModel;
            var data = DataPicker.Date;
            var tipoVisita = TipoVisitaPicker.SelectedItem?.ToString();
            var note = NoteEntry.Text;

            if (medico == null || string.IsNullOrWhiteSpace(tipoVisita))
            {
                await DisplayAlert("Errore", "Seleziona medico e tipo visita.", "OK");
                return;
            }

            // Verifica disponibilità medico
            var firestore = Plugin.Firebase.Firestore.CrossFirebaseFirestore.Current;
            var dispDoc = await firestore.Collection("disponibilita_medici").Document(medico.Id).GetAsync();
            bool disponibile = false;
            if (dispDoc.Exists)
            {
                var intervalli = dispDoc.Get("intervalli") as IEnumerable<object>;
                if (intervalli != null)
                {
                    foreach (var intervalloObj in intervalli)
                    {
                        var intervallo = intervalloObj as IDictionary<string, object>;
                        if (intervallo != null && intervallo.ContainsKey("start") && intervallo.ContainsKey("end"))
                        {
                            DateTime start = DateTime.Parse(intervallo["start"].ToString());
                            DateTime end = DateTime.Parse(intervallo["end"].ToString());
                            if (data >= start && data <= end)
                            {
                                disponibile = true;
                                break;
                            }
                        }
                    }
                }
            }
            if (!disponibile)
            {
                await DisplayAlert("Non disponibile", $"Il medico {medico.Nome} non è disponibile per la data selezionata.", "OK");
                return;
            }

            // Recupera UID paziente (utente loggato)
            var auth = Plugin.Firebase.Auth.CrossFirebaseAuth.Current;
            var pazienteId = auth.CurrentUser?.Uid;
            if (string.IsNullOrWhiteSpace(pazienteId))
            {
                await DisplayAlert("Errore", "Utente non autenticato.", "OK");
                return;
            }

            // Salva richiesta su Firestore
            await firestore.Collection("richieste_visite").AddAsync(new Dictionary<string, object>
            {
                { "paziente_id", pazienteId },
                { "medico_id", medico.Id },
                { "data", data },
                { "tipo_visita", tipoVisita },
                { "stato", "in_attesa" },
                { "note", note ?? "" }
            });

            await DisplayAlert("Richiesta", $"Richiesta inviata a {medico.Nome} per il {data:d} ({tipoVisita})", "OK");
        }
    }
}
