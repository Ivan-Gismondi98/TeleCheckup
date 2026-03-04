using System;
using Microsoft.Maui.Controls;
using Plugin.Firebase.Firestore;

namespace TeleCheckup.Views
{
    public partial class ModificaRichiestaPage : ContentPage
    {
        private readonly RichiestaVisitaModel _richiesta;
        public ModificaRichiestaPage(RichiestaVisitaModel richiesta)
        {
            InitializeComponent();
            TipoVisitaPicker.ItemsSource = new System.Collections.Generic.List<string>
            {
                "Cardiologica", "Odontoiatrica", "Fisioterapica", "Analisi del sangue"
            };
            _richiesta = richiesta;
            DataPicker.Date = DateTime.TryParse(richiesta.Data, out var d) ? d : DateTime.Today;
            TipoVisitaPicker.SelectedItem = richiesta.TipoVisita;
            NoteEntry.Text = richiesta.Note;
        }

        private async void OnSalvaModificheClicked(object sender, EventArgs e)
        {
            var data = DataPicker.Date;
            var tipoVisita = TipoVisitaPicker.SelectedItem?.ToString();
            var note = NoteEntry.Text;
            if (string.IsNullOrWhiteSpace(tipoVisita))
            {
                await DisplayAlert("Errore", "Seleziona il tipo di visita.", "OK");
                return;
            }
            var firestore = Plugin.Firebase.Firestore.CrossFirebaseFirestore.Current;
            await firestore.Collection("richieste_visite").Document(_richiesta.Id)
                .UpdateAsync(new System.Collections.Generic.Dictionary<string, object>
                {
                    { "data", data },
                    { "tipo_visita", tipoVisita },
                    { "note", note ?? "" }
                });
            await DisplayAlert("Successo", "Richiesta aggiornata.", "OK");
            await Navigation.PopAsync();
        }
    }
}
