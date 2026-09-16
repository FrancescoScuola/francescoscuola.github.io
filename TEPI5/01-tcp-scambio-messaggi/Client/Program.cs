using System.Net.Sockets;
using System.Text;

// Client TCP minimale: invia righe di testo al server e stampa la risposta.
string host = args.Length > 0 ? args[0] : "127.0.0.1";
int porta = args.Length > 1 ? int.Parse(args[1]) : 5000;

using var client = new TcpClient();
client.Connect(host, porta);
Console.WriteLine($"Connesso a {host}:{porta}. Scrivi un messaggio (FINE per uscire).");

using NetworkStream stream = client.GetStream();
using var reader = new StreamReader(stream, Encoding.UTF8);
using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

while (true)
{
    Console.Write("> ");
    string? testo = Console.ReadLine();
    if (testo == null) break;
    writer.WriteLine(testo);
    Console.WriteLine(reader.ReadLine());
    if (testo.Equals("FINE", StringComparison.OrdinalIgnoreCase)) break;
}
