using System.Net;
using System.Net.Sockets;
using System.Text;

// Server TCP minimale: accetta un client alla volta e risponde a ogni messaggio.
const int porta = 5000;
var listener = new TcpListener(IPAddress.Any, porta);
listener.Start();
Console.WriteLine($"Server in ascolto sulla porta {porta}...");

while (true)
{
    using TcpClient client = listener.AcceptTcpClient();   // bloccante: attende una connessione
    Console.WriteLine($"Client connesso: {client.Client.RemoteEndPoint}");

    using NetworkStream stream = client.GetStream();
    using var reader = new StreamReader(stream, Encoding.UTF8);
    using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

    string? messaggio;
    while ((messaggio = reader.ReadLine()) != null)       // null = il client ha chiuso
    {
        Console.WriteLine($"Ricevuto: {messaggio}");
        writer.WriteLine($"ECO: {messaggio}");
        if (messaggio.Equals("FINE", StringComparison.OrdinalIgnoreCase)) break;
    }
    Console.WriteLine("Client disconnesso.");
}
