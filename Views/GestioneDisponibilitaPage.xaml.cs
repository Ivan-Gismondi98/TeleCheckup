using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace TeleCheckup.Views
{
    public partial class GestioneDisponibilitaPage : ContentPage
    {
        public ObservableCollection<IntervalloDisponibilita> Intervalli { get; set; } = new();
        public GestioneDisponibilitaPage()
        {
            InitializeComponent();
            IntervalliCollectionView.ItemsSource = Intervalli;
            LoadDisponibilita();
        }

        private async void LoadDisponibilita()
        {
            var auth = Plugin.Firebase.Auth.CrossFirebaseAuth.Current.Instance;
            var medicoId = auth.CurrentUser?.Uid;
            if (string.IsNullOrWhiteSpace(medicoId)) return;
            var firestore = Plugin.Firebase.Firestore.CrossFirebaseFirestore.Current.Instance;
            var doc = await firestore.Collection("disponibilita_medici").Document(medicoId).GetAsync();
            Intervalli.Clear();
            if (doc.Exists)
            {
                var intervalli = doc.Get("intervalli") as IEnumerable<object>;
                if (intervalli != null)
                {
                    foreach (var intervalloObj in intervalli)
                    {
                        var intervallo = intervalloObj as IDictionary<string, object>;
                        if (intervallo != null && intervallo.ContainsKey("start") && intervallo.ContainsKey("end"))
                        {
                            Intervalli.Add(new IntervalloDisponibilita
                            {
                                Start = intervallo["start"].ToString(),
                                End = intervallo["end"].ToString()
                            });
                        }
                    }
                }
            }
        }

        private async void OnAggiungiIntervalloClicked(object sender, EventArgs e)
        {
            var start = StartDatePicker.Date;
            var end = EndDatePicker.Date;
            if (start > end)
            {
                await DisplayAlert("Errore", "La data di inizio deve essere prima della data di fine.", "OK");
                return;
            }
            var nuovo = new IntervalloDisponibilita { Start = start.ToString("yyyy-MM-dd"), End = end.ToString("yyyy-MM-dd") };
            Intervalli.Add(nuovo);

            // Salva su Firestore
            var auth = Plugin.Firebase.Auth.CrossFirebaseAuth.Current.Instance;
            var medicoId = auth.CurrentUser?.Uid;
            if (string.IsNullOrWhiteSpace(medicoId)) return;
            var firestore = Plugin.Firebase.Firestore.CrossFirebaseFirestore.Current.Instance;
            var intervalliFirestore = Intervalli.Select(i => new Dictionary<string, object>
            {
                { "start", i.Start },
                { "end", i.End }
            }).ToList();
            await firestore.Collection("disponibilita_medici").Document(medicoId)
                .SetAsync(new Dictionary<string, object> { { "intervalli", intervalliFirestore } });
        }

        private async void OnEliminaIntervalloClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is IntervalloDisponibilita intervallo)
            {
                Intervalli.Remove(intervallo);

                // Aggiorna Firestore
                var auth = Plugin.Firebase.Auth.CrossFirebaseAuth.Current.Instance;
                var medicoId = auth.CurrentUser?.Uid;
                if (string.IsNullOrWhiteSpace(medicoId)) return;
                var firestore = Plugin.Firebase.Firestore.CrossFirebaseFirestore.Current.Instance;
                var intervalliFirestore = Intervalli.Select(i => new Dictionary<string, object>
                {
                    { "start", i.Start },
                    { "end", i.End }
                }).ToList();
                await firestore.Collection("disponibilita_medici").Document(medicoId)
                    .SetAsync(new Dictionary<string, object> { { "intervalli", intervalliFirestore } });
            }
        }
    }

    public class IntervalloDisponibilita
    {
        public string Start { get; set; }
        public string End { get; set; }
    }
}
