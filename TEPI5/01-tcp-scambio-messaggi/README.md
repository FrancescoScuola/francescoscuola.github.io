# 01 · TCP – scambio messaggi

**Materia:** TEPI5 · **Tecnologie:** C#, .NET Framework 4.8, Visual Studio

## Obiettivo didattico

Comprendere il modello client-server su TCP: apertura di una connessione,
invio e ricezione di dati tramite stream, chiusura della connessione.

## Prerequisiti

- Pila ISO/OSI e TCP/IP; concetto di porta e socket
- Differenza tra TCP e UDP
- Basi di C# (variabili, cicli, console)
- Visual Studio con il carico di lavoro "Sviluppo desktop .NET" (.NET Framework 4.8)

## Struttura

```
TcpScambioMessaggi.sln      ← aprire questo file in Visual Studio
├── Server/                 ← progetto console (.NET Framework 4.8)
│   └── Program.cs
└── Client/                 ← progetto console (.NET Framework 4.8)
    └── Program.cs
```

## Come si esegue

1. Aprire `TcpScambioMessaggi.sln` in Visual Studio.
2. Tasto destro sulla soluzione → **Imposta progetti di avvio…** →
   **Più progetti di avvio**: impostare *Server* e *Client* su **Avvio**
   (Server in cima alla lista).
3. Premere **F5**: si aprono due console.
4. Nella console del client premere INVIO per connettersi, poi scrivere i
   messaggi; il server risponde con `ECO: <messaggio>`. `FINE` chiude la sessione.

In alternativa si possono avviare a mano gli eseguibili in `Server\bin\Debug\`
e `Client\bin\Debug\`; al client si possono passare IP e porta del server:
`Client.exe 192.168.1.10 5000`.

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
