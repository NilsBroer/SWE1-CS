using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using BIF.SWE1.Interfaces;

using System.Net; // For IPAddress
using System.Net.Sockets; // For TcpListener, TcpClient
using System.Threading;

namespace MyWebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            int server_port = 8080; //change here
            Server server = new Server(server_port);
            //Server exc_server = new Server(server_port); //Will fail

            //DEMO.DB_Populator populator = new DEMO.DB_Populator(); //Populate DataBase, !do not uncomment!

            server.listen();
            Console.ReadKey(); //hol' up
        }
    }

    class Server
    {
        private int port = 8080; //8080 is default
        TcpListener listener = null;
        private bool is_running = false;

        public Server(int port_in)
        {
            if (port_in > 0)
                port = port_in;

            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
            }
            catch (SocketException exc)
            {
                Console.WriteLine(exc.ErrorCode + ": " + exc.Message);
            }
            Console.WriteLine("Server started successfully.");
            is_running = true;
        }

        public void listen()
        {
            while (is_running)
            {
                TcpClient client = null;
                NetworkStream nstream = null;

                try
                {
                    client = listener.AcceptTcpClient();
                    nstream = client.GetStream();
                    ClientHandler client_handler = new ClientHandler(nstream, client);
                    Thread client_thread = new Thread(client_handler.handle_client);
                    client_thread.Start();
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                    nstream.Close();
                }
            }
        }
    }

    class ClientHandler
    {
        private const int BUFFER_SIZE = 1024; //Nicht fix

        private NetworkStream nstream;
        private TcpClient client;
        private String clientID;
        static int connected_client = 0;
        public ClientHandler(NetworkStream nstream_in, TcpClient client_in)
        {
            nstream = nstream_in;
            client = client_in;
            connected_client++;
            clientID = "Client_0" + connected_client.ToString();
            Console.WriteLine(clientID + " connected successfully.");
        }
        
        public void handle_client()
        {
            Request request = new Request(nstream);
            Console.WriteLine(request.toString());
            nstream.Close();
            client.Close();
            Console.WriteLine(clientID + " disconnected.");
        }
    }
}
