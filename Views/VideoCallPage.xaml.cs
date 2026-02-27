namespace TeleCheckup.Views;

public partial class VideoCallPage : ContentPage
{
        public VideoCallPage()
        {
                InitializeComponent();
        }

        private async void OnEndCallClicked(object sender, EventArgs e)
        {
                // Torna indietro alla Dashboard
                await Shell.Current.GoToAsync("..");
        }
}