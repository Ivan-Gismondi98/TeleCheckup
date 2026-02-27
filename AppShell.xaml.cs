
using TeleCheckup.Views;
using System.Diagnostics;
namespace TeleCheckup;

public partial class AppShell : Shell
{
    public AppShell()
    {
        Debug.WriteLine("AppShell constructor: InitializeComponent start");
        InitializeComponent();
        Debug.WriteLine("AppShell constructor: InitializeComponent completed");
        // Registrazione delle rotte nascoste dal menu laterale
        Debug.WriteLine("AppShell constructor: registering routes");
        Routing.RegisterRoute(nameof(PharmaCheckerPage), typeof(PharmaCheckerPage));
        Routing.RegisterRoute(nameof(VideoCallPage), typeof(VideoCallPage));
        // Registrazione di rotte per le dashboard specifiche e la registrazione paziente
        Routing.RegisterRoute(nameof(DoctorDashboardPage), typeof(DoctorDashboardPage));
        Routing.RegisterRoute(nameof(AdminDashboardPage), typeof(AdminDashboardPage));
        Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
        Routing.RegisterRoute(nameof(Views.SymptomIntensityPage), typeof(Views.SymptomIntensityPage));
        Routing.RegisterRoute(nameof(Views.AiAdvicePage), typeof(Views.AiAdvicePage));
        Routing.RegisterRoute(nameof(Views.ChatPage), typeof(Views.ChatPage));
        Debug.WriteLine("AppShell constructor: routes registered");
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        // Simple logout: navigate to login page root
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
