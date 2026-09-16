# 01 · TCP – scambio messaggi

**Classe:** TEPI5 · **Materia:** Sistemi e Reti · **Tecnologie:** C#, .NET 8

## Obiettivo didattico

Comprendere il modello client-server su TCP: apertura di una connessione,
invio e ricezione di dati tramite stream, chiusura della connessione.

## Prerequisiti

- Pila ISO/OSI e TCP/IP; concetto di porta e socket
- Differenza tra TCP e UDP
- Basi di C# (variabili, cicli, console)
- .NET 8 SDK installato (`dotnet --version`)

## Come si esegue

Aprire due terminali nella cartella del progetto.

```bash
# Terminale 1
dotnet run --project Server

# Terminale 2 (host e porta opzionali, default 127.0.0.1 5000)
dotnet run --project Client -- 127.0.0.1 5000
```

Scrivere messaggi nel client; il server risponde con `ECO: <messaggio>`.
`FINE` chiude la sessione.

## Spiegazione per la lezione

1. **Server** – `TcpListener` si mette in ascolto su una porta (`Start`),
   `AcceptTcpClient` blocca finché un client non si connette (three-way handshake).
2. **Client** – `TcpClient.Connect` avvia la connessione verso IP:porta.
3. **Stream** – entrambi ottengono un `NetworkStream`: TCP è un flusso di byte,
   non di messaggi. Per delimitare i messaggi usiamo il fine riga
   (`ReadLine`/`WriteLine`): è un semplice *protocollo applicativo*.
4. **Chiusura** – quando il client chiude, `ReadLine` restituisce `null`.

## Note per la classe

- Se il client non si connette: il server è avviato? La porta è corretta?
  Il firewall blocca la porta 5000?
- Provate tra due PC del laboratorio usando l'IP del server (`ipconfig`).
- Osservate il traffico con Wireshark (filtro `tcp.port == 5000`).

## Estensioni possibili

- Server multi-client con `Task`/thread (uno per connessione)
- Chat: il server inoltra i messaggi a tutti i client connessi
- Comandi applicativi (`ORA`, `SOMMA 3 4`, …)
- Versione UDP e confronto con TCP
