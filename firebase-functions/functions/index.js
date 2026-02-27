// Funzione HTTP per invio notifiche push
exports.sendNotification = functions.https.onRequest(async (req, res) => {
  if (req.method !== "POST") {
    return res.status(405).send("Method Not Allowed");
  }

  const { token, title, body, data } = req.body;
  if (!token || !title || !body) {
    return res.status(400).send("Missing required fields");
  }

  const message = {
    token: token,
    notification: {
      title: title,
      body: body
    },
    data: data || {}
  };

  try {
    await admin.messaging().send(message);
    return res.status(200).send("Notification sent");
  } catch (error) {
    console.error("Error sending notification:", error);
    return res.status(500).send("Error: " + error.message);
  }
});

// Trigger Firestore automatico per tutte le richieste
const richiesteCollections = [
  "richieste_visite",
  "richieste_analisi",
  "richieste_fisioterapie",
  "richieste_televisite"
];

for (const col of richiesteCollections) {
  exports["notifyOn" + col.charAt(0).toUpperCase() + col.slice(1)] = functions.firestore
    .document(`${col}/{id}`)
    .onWrite(async (change, context) => {
      const data = change.after.exists ? change.after.data() : null;
      if (!data) return;
      // Recupera token FCM del destinatario (medico o paziente)
      let destinatarioId = data.medico_id;
      let tipoNotifica = "gestione";
      let title = `Nuova richiesta da ${data.nome_paziente || "paziente"}`;
      let body = `${data.tipo_visita || data.tipo_analisi || data.tipo_fisioterapia || data.tipo_televisita || "prenotazione"} per il ${data.data}`;
      // Se stato cambia (approvato, rifiutato, rischedulato), notifica al paziente
      if (change.before.exists && change.after.exists && change.before.data().stato !== data.stato) {
        destinatarioId = data.paziente_id;
        tipoNotifica = "cronologia";
        title = `Aggiornamento richiesta (${data.stato})`;
        body = `La tua richiesta è stata ${data.stato}. Data: ${data.data}`;
      }
      // Recupera token FCM
      const userDoc = await admin.firestore().collection("utenti").doc(destinatarioId).get();
      const token = userDoc.exists && userDoc.data().fcmToken ? userDoc.data().fcmToken : null;
      if (!token) return;
      // Invia notifica
      const message = {
        token: token,
        notification: {
          title: title,
          body: body
        },
        data: {
          tipo: tipoNotifica,
          id_richiesta: context.params.id,
          collezione: col
        }
      };
      try {
        await admin.messaging().send(message);
      } catch (error) {
        console.error("Error sending notification:", error);
      }
    });
}
const functions = require("firebase-functions");
const admin = require("firebase-admin");
admin.initializeApp();

exports.deleteUser = functions.https.onRequest(async (req, res) => {
  // Solo POST e solo admin autenticati!
  if (req.method !== "POST") {
    return res.status(405).send("Method Not Allowed");
  }

  // (Opzionale) Autenticazione tramite chiave segreta nell'header
  // if (req.headers['x-api-key'] !== 'LA_TUA_CHIAVE_SUPERSEGRETA') {
  //   return res.status(403).send("Forbidden");
  // }

  const uid = req.body.uid;
  if (!uid) {
    return res.status(400).send("Missing uid");
  }

  try {
    await admin.auth().deleteUser(uid);
    return res.status(200).send("User deleted");
  } catch (error) {
    console.error("Error deleting user:", error);
    return res.status(500).send("Error deleting user: " + error.message);
  }
});