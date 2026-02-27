# TeleCheckup

TeleCheckup è un'applicazione multipiattaforma sviluppata con .NET MAUI per la gestione di checkup e videochiamate tra pazienti, medici e amministratori. Supporta Android, Windows, iOS e MacCatalyst.

## Funzionalità principali
- Login per pazienti, medici e amministratori
- Dashboard dedicate per ogni ruolo
- Videochiamata integrata (WebRTC)
- Gestione profilo utente
- Visualizzazione e gestione farmaci (PharmaChecker)

## Struttura del progetto
- **Views/**: Pagine XAML per le varie schermate (Login, Dashboard, VideoCall, ecc.)
- **Resources/Styles/**: Dizionari di risorse XAML per colori e stili
- **Platforms/**: Codice specifico per Android, iOS, MacCatalyst, Windows
- **AppShell.xaml**: Navigazione e routing
- **MauiProgram.cs**: Configurazione servizi e toolkit

## Requisiti
- [.NET 8+ SDK](https://dotnet.microsoft.com/download)
- Android SDK (per emulatore Android)
- Visual Studio 2022+ o VS Code con estensioni MAUI

## Come eseguire l'app

### Windows
1. Apri una shell nella cartella del progetto
2. Esegui:
   ```
   dotnet build -f:net10.0-windows10.0.19041.0
   dotnet run -f:net10.0-windows10.0.19041.0
   ```

### Android
1. Avvia un emulatore Android (es. `emulator -avd Pixel_5_API_34`)
2. Esegui:
   ```
   dotnet build -f:net10.0-android
   dotnet run -f:net10.0-android
   ```

### iOS / MacCatalyst
- Richiede macOS con Xcode installato.
- Esegui:
   ```
   dotnet build -f:net10.0-ios
   dotnet run -f:net10.0-ios
   ```
   oppure
   ```
   dotnet build -f:net10.0-maccatalyst
   dotnet run -f:net10.0-maccatalyst
   ```

## Note
- Alcuni warning possono apparire in build (es. Frame obsoleto): non bloccano l'esecuzione.
- Per la videochiamata è richiesto il permesso di accesso a microfono e fotocamera.
- Per modifiche ai dizionari di risorse, aggiornare i file in `Resources/Styles/`.

## Autori
- Progetto generato e mantenuto con supporto di GitHub Copilot
