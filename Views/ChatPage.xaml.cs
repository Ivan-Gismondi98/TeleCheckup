using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace TeleCheckup.Views;

[QueryProperty("Area","area")]
public partial class ChatPage : ContentPage
{
    private string _area;
    public string Area
    {
        get => _area;
        set
        {
            _area = value;
            AreaLabel.Text = $"Chat - Area: {_area}";
        }
    }

    public ObservableCollection<string> Messages { get; } = new ObservableCollection<string>();

    public ChatPage()
    {
        InitializeComponent();
        MessagesList.ItemsSource = Messages;
        // sample messages
        Messages.Add("Medico: Ciao, descrivimi il tuo problema.");
    }

    private void OnSend(object sender, EventArgs e)
    {
        var text = MessageEntry.Text;
        if (!string.IsNullOrWhiteSpace(text))
        {
            Messages.Add("Tu: " + text);
            MessageEntry.Text = string.Empty;
        }
    }
    private async void OnVideoCall(object sender, EventArgs e)
    {
        // Navigate to video call page
        await Shell.Current.GoToAsync(nameof(VideoCallPage));
    }

    private async void OnBackPressed(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnStartTelevisit(object sender, EventArgs e)
    {
        // For now navigate to the VideoCallPage
        await Shell.Current.GoToAsync(nameof(VideoCallPage));
    }
}
