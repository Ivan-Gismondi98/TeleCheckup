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
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		Debug.WriteLine("CreateWindow: creating AppShell");
		var shell = new AppShell();
		Debug.WriteLine("CreateWindow: AppShell created");
		return new Window(shell);
	}
}