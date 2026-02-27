# TeleCheckup - Informazioni Tecniche

## Stack Tecnologico
- **Framework:** .NET MAUI (multipiattaforma)
- **Firebase:** Firestore (CRUD), Auth (login via REST API), Cloud Messaging (notifiche)
- **Videochiamata:** WebRTC (pagina VideoCall)
- **UI:** XAML, CommunityToolkit.Maui
- **Linguaggio:** C#

## Struttura del Progetto
- **Views/**: Tutte le pagine dell'app (Login, Dashboard, VideoCall, ecc.)
- **Resources/**: Icone, immagini, stili, splash
- **Platforms/**: Codice specifico per Android/iOS/MacCatalyst/Windows
- **AppShell.xaml**: Routing e navigazione
- **MauiProgram.cs**: Configurazione servizi

## Integrazione Firebase
- **Firestore:** CRUD tramite Plugin.Firebase.Firestore v4 (GetCollection, GetDocument, SetDataAsync, ecc.)
- **Auth:** Login/registrazione tramite REST API Firebase (HttpClient)
- **Cloud Messaging:** Gestione notifiche push (da completare)

## Funzionalità Implementate
- Login e registrazione (pazienti, medici, admin)
- Dashboard dedicate per ogni ruolo
- Videochiamata (WebRTC)
- Gestione richieste e profili
- Visualizzazione farmaci

## Cosa Rimane da Fare per la Pubblicazione
1. **Gestione errori e validazione input**: Migliorare i messaggi di errore e la validazione dei dati utente.
2. **Notifiche push**: Completare l'integrazione con Firebase Cloud Messaging.
3. **Test cross-platform**: Verificare il funzionamento su Android, iOS, Windows, MacCatalyst.
4. **Ottimizzazione UI/UX**: Migliorare layout, accessibilità e responsive design.
5. **Configurazione API Key**: Inserire la tua API key Firebase nei punti REST.
6. **Pulizia codice**: Rimuovere dipendenze inutili e refactor.
7. **Documentazione**: Aggiornare README principale e aggiungere guide per installazione/configurazione.
8. **Pubblicazione store**: Preparare icone, splash, privacy policy, e build per store.

## Note Tecniche
- La creazione utente ora avviene tramite REST API Firebase.
- Tutte le operazioni Firestore usano Plugin.Firebase.Firestore v4.
- Per notifiche push, assicurarsi che il progetto Firebase sia configurato correttamente.

## Contatti e Supporto
Per supporto tecnico o segnalazioni, contatta lo sviluppatore o apri una issue su GitHub.
