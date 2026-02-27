using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace TeleCheckup;

public partial class App : Application
{
	public App()
	{
		Debug.WriteLine("App constructor: InitializeComponent start");
		InitializeComponent();
		Debug.WriteLine("App constructor: InitializeComponent completed");
		// Configura ricezione notifiche push Firebase
		// Plugin.Firebase v4: usa metodi GetTokenAsync e SubscribeToTopicAsync
		_ = SalvaTokenFcm();
	}

	private async Task SalvaTokenFcm()
	{
		var auth = Plugin.Firebase.Auth.CrossFirebaseAuth.Current;
		var uid = auth.CurrentUser?.Uid;
		if (string.IsNullOrWhiteSpace(uid)) return;
		var fcm = Plugin.Firebase.CloudMessaging.CrossFirebaseCloudMessaging.Current;
		var token = await fcm.GetTokenAsync();
		var firestore = Plugin.Firebase.Firestore.CrossFirebaseFirestore.Current;
		await Plugin.Firebase.Firestore.CrossFirebaseFirestore.Current
			.GetCollection("utenti")
			.GetDocument(uid)
			.UpdateDataAsync(new System.Collections.Generic.Dictionary<object, object> {
				{ "fcmToken", token }
			});
    
	}

	// TODO: Implementa ricezione notifiche push e deep link con Plugin.Firebase v4

	protected override Window CreateWindow(IActivationState? activationState)
	{
		Debug.WriteLine("CreateWindow: creating AppShell");
		var shell = new AppShell();
		Debug.WriteLine("CreateWindow: AppShell created");
		return new Window(shell);
	}
}