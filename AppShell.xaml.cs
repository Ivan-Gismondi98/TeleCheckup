
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
        Debug.WriteLine("AppShell constructor: routes registered");
    }
}
