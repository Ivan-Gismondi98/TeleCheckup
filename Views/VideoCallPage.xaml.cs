using Microsoft.Maui.Controls;
namespace TeleCheckup.Views;

public partial class VideoCallPage : ContentPage
{
    public VideoCallPage()
    {
        InitializeComponent();
        // Carica la pagina HTML locale WebRTC
#if ANDROID
        VideoCallWebView.Source = "file:///android_asset/webrtc.html";
#elif IOS
        VideoCallWebView.Source = "webrtc.html";
#else
        VideoCallWebView.Source = "Resources/Raw/webrtc.html";
#endif
    }

    private async void OnEndCallClicked(object sender, EventArgs e)
    {
        // Torna indietro o chiudi la pagina
        await Shell.Current.GoToAsync("..", true);
    }
}