using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    // Client TCP minimale: invia righe di testo al server e stampa la risposta.
    class Program
    {
        static void Main(string[] args)
        {
            string host = "127.0.0.1";
            int porta = 5000;
            if (args.Length > 0) host = args[0];
            if (args.Length > 1) porta = int.Parse(args[1]);

            // Attendo che l'utente sia pronto: cosi' il server ha il tempo di avviarsi
            Console.WriteLine("Premi INVIO per connetterti a " + host + ":" + porta);
            Console.ReadLine();

            try
            {
                using (TcpClient client = new TcpClient())
                {
                    client.Connect(host, porta);
                    Console.WriteLine("Connesso. Scrivi un messaggio (FINE per uscire).");

                    using (NetworkStream stream = client.GetStream())
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
                    {
                        writer.AutoFlush = true;

                        while (true)
                        {
                            Console.Write("> ");
                            string testo = Console.ReadLine();
                            if (testo == null)
                                break;

                            writer.WriteLine(testo);
                            Console.WriteLine(reader.ReadLine());

                            if (testo.ToUpper() == "FINE")
                                break;
                        }
                    }
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Impossibile connettersi: " + ex.Message);
            }

            Console.WriteLine("Premi INVIO per chiudere.");
            Console.ReadLine();
        }
    }
}
