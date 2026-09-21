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

### L'idea in una frase

Il programma fa quello che fate voi quando telefonate a un amico:
uno **aspetta la chiamata** (server), l'altro **compone il numero** (client),
poi i due **parlano a turno** finché uno dice "ciao" e **riaggancia**.

| Nella telefonata | Nel programma |
|------------------|---------------|
| Il numero di telefono | Indirizzo IP + porta (`127.0.0.1:5000`) |
| Il centralino che aspetta le chiamate | `TcpListener` |
| Il telefono con la linea aperta | `TcpClient` |
| Il filo in cui passa la voce | `NetworkStream` |
| L'orecchio (sente e capisce le parole) | `StreamReader` |
| La bocca (trasforma i pensieri in voce) | `StreamWriter` |
| Dire "ciao" e riagganciare | `FINE` e chiusura dei `using` |

### Le classi, una per una

#### `TcpListener` – il centralino

**Analogia.** È il centralino di un'azienda: sta fermo ad aspettare che
squilli il telefono. Quando arriva una chiamata la passa a un operatore e
torna ad aspettare la successiva.

**Nel dettaglio.** Esiste solo nel server. `new TcpListener(IPAddress.Any, 5000)`
dice "ascolto sulla porta 5000, su tutte le schede di rete del PC";
`Start()` apre davvero la porta. `AcceptTcpClient()` è **bloccante**: il
programma si ferma su quella riga finché un client non si connette (in quel
momento avviene il *three-way handshake* SYN → SYN/ACK → ACK). Quando ritorna,
restituisce un `TcpClient` che rappresenta **quella** connessione.

#### `TcpClient` – il telefono con la linea aperta

**Analogia.** È il telefono durante la chiamata: finché è acceso, voi e il
vostro amico siete collegati. Da solo però non serve a parlare: vi serve il
filo in cui passa la voce.

**Nel dettaglio.** Rappresenta **una connessione TCP** tra due programmi, cioè
una coppia di socket (IP:porta del client ↔ IP:porta del server).

- Nel **client** lo creiamo noi e chiamiamo `Connect(host, porta)`: se il
  server non è in ascolto viene lanciata una `SocketException`.
- Nel **server** non lo creiamo noi: ce lo restituisce `AcceptTcpClient()`.

`client.Client.RemoteEndPoint` ci dice chi c'è dall'altra parte (IP e porta
del client). Con `GetStream()` otteniamo il canale per scambiare dati.

#### `NetworkStream` – il filo

**Analogia.** È il tubo che collega i due telefoni: da una parte entra
qualcosa, dall'altra esce, **nello stesso ordine** e senza perdite. Il tubo
però non sa cosa trasporta: per lui sono solo "gocce", non parole.

**Nel dettaglio.** È il flusso di **byte** della connessione, in entrambe le
direzioni (si legge e si scrive sullo stesso oggetto). TCP garantisce che i
byte arrivino tutti e in ordine, ma **non conserva i confini dei messaggi**:
se il client invia "ciao" e poi "come va", il server potrebbe ricevere
"ciaocome va" tutto insieme, o a pezzi. Con `NetworkStream` da solo dovremmo
lavorare con array di `byte[]` e con `Read`/`Write`: scomodo. Per questo lo
"avvolgiamo" con un lettore e uno scrittore di testo.

#### `StreamReader` – l'orecchio

**Analogia.** Il filo porta solo vibrazioni; è l'orecchio che le trasforma in
parole e frasi che capiamo.

**Nel dettaglio.** Legge i byte dal `NetworkStream` e li **decodifica in
testo** secondo una codifica (qui `Encoding.UTF8`, così funzionano anche le
lettere accentate). Il metodo che usiamo è `ReadLine()`:

- aspetta (è bloccante) finché non arriva un **fine riga**, poi restituisce
  la riga come `string`, senza il carattere di a capo;
- restituisce **`null`** quando l'altra parte ha chiuso la connessione:
  è così che il server capisce che il client se n'è andato.

#### `StreamWriter` – la bocca

**Analogia.** È il contrario dell'orecchio: prende i pensieri (una `string`)
e li trasforma in "voce" (byte) da mandare nel filo.

**Nel dettaglio.** `WriteLine(testo)` **codifica** la stringa in byte UTF-8 e
aggiunge il fine riga (`\r\n`). Il fine riga è il nostro accordo per dire
"il messaggio è finito": è un piccolissimo **protocollo applicativo**, che
risolve il problema dei confini visto sopra.

Attenzione: per efficienza lo `StreamWriter` tiene i dati in un **buffer**
(come una lettera che si scrive ma non si imbuca subito). Con
`writer.AutoFlush = true` ogni `WriteLine` viene spedita immediatamente;
senza, l'altro programma resterebbe in attesa di un messaggio che non parte.

### Come si incastrano

```
 CLIENT                                                    SERVER
 string "ciao"                                             string "ciao"
     │ StreamWriter.WriteLine  (testo → byte + \r\n)           ▲ StreamReader.ReadLine (byte → testo)
     ▼                                                         │
 NetworkStream ════════════ byte su TCP (IP:porta) ═══════> NetworkStream
     ▲                                                         │
 TcpClient.Connect ─────── connessione ─────────── TcpListener.AcceptTcpClient
```

La risposta `ECO: ciao` fa lo stesso percorso al contrario.

### A cosa servono i blocchi `using`

**Analogia.** È la regola "quando hai finito, riaggancia e rimetti a posto".

**Nel dettaglio.** Connessioni e stream occupano risorse del sistema operativo
(la porta, il socket, memoria). Il blocco `using (...) { ... }` garantisce che,
all'uscita dalle graffe, venga chiamato `Dispose()` e la risorsa venga
chiusa, **anche in caso di errore**. I blocchi sono annidati: si apre dal più
esterno (`TcpClient`) al più interno (`StreamWriter`) e si chiude in ordine
inverso.

### Il flusso del programma

1. **Server** – crea il `TcpListener`, chiama `Start()` e si blocca su
   `AcceptTcpClient()`.
2. **Client** – dopo l'INVIO dell'utente chiama `Connect()`: la connessione
   è stabilita e il server "si sblocca".
3. **Scambio** – il client scrive una riga con `WriteLine` e aspetta la
   risposta con `ReadLine`; il server legge la riga, la stampa e risponde con
   `ECO: ...`. Si prosegue a turno.
4. **Chiusura** – se il messaggio è `FINE` entrambi escono dal ciclo; i
   `using` chiudono la connessione. Il server torna ad `AcceptTcpClient()` e
   aspetta il prossimo client.

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
