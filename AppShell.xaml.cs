
using TeleCheckup.Views;
namespace TeleCheckup;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		// Registrazione delle pagine per la navigazione
		Routing.RegisterRoute(nameof(VideoCallPage), typeof(VideoCallPage));
		Routing.RegisterRoute(nameof(PatientDashboardPage), typeof(PatientDashboardPage));
		Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
		Routing.RegisterRoute(nameof(DoctorDashboardPage), typeof(DoctorDashboardPage));
		Routing.RegisterRoute(nameof(AdminDashboardPage), typeof(AdminDashboardPage));
		// PharmaCheckerPage non implementata, rimuovo la registrazione
	}
}
