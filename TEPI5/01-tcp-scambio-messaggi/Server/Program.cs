using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    // Server TCP minimale: accetta un client alla volta e risponde a ogni messaggio.
    class Program
    {
        const int PORTA = 5000;

        static void Main(string[] args)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, PORTA);
            listener.Start();
            Console.WriteLine("Server in ascolto sulla porta " + PORTA + "...");

            while (true)
            {
                // Bloccante: attende che un client si connetta
                using (TcpClient client = listener.AcceptTcpClient())
                {
                    Console.WriteLine("Client connesso: " + client.Client.RemoteEndPoint);

                    using (NetworkStream stream = client.GetStream())
                    {
                        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
                            {
                                writer.AutoFlush = true;

                                string messaggio;
                                // ReadLine restituisce null quando il client chiude la connessione
                                while ((messaggio = reader.ReadLine()) != null)
                                {
                                    Console.WriteLine("Ricevuto: " + messaggio);
                                    writer.WriteLine("ECO: " + messaggio);

                                    if (messaggio.ToUpper() == "FINE")
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    Console.WriteLine("Client disconnesso.");
                }
            }
        }
    }
}
