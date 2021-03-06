﻿using System;
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

    /// <summary>
    /// Starts the Server and listening
    /// </summary>
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

    /// <summary>
    /// The Server itself
    /// </summary>
    class Server
    {
        private int port = 8080; //8080 is default
        TcpListener listener = null;
        private bool is_running = false;

        /// <summary>
        /// The Server Constructor
        /// </summary>
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

        /// <summary>
        /// Listens for Clients connecting and dispatches the Threads
        /// </summary>
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

    /// <summary>
    /// The Client Handler
    /// </summary>
    class ClientHandler
    {
        private const int BUFFER_SIZE = 1024; //Nicht fix

        private NetworkStream nstream;
        private TcpClient client;
        static int connected_client = 0;

        /// <summary>
        /// Constructor for the Handler
        /// </summary>
        public ClientHandler(NetworkStream nstream_in, TcpClient client_in)
        {
            nstream = nstream_in;
            client = client_in;
            connected_client++;
            Console.WriteLine("Connected. [{0}]", connected_client);
        }

        /// <summary>
        /// Picks a Plugin for the Client's request
        /// </summary>
        public void handle_client()
        {
            Request request = new Request(nstream);
            PluginManager pluginmanager = new PluginManager();
            Console.WriteLine(request.toString());
            IPlugin plugin = pluginmanager.GetBestPlugin(request);
            if(plugin!=null)
            {
                var response = plugin.Handle(request);
                response.Send(nstream);
            }
            nstream.Close();
            client.Close();
            Console.WriteLine("Disconnected. [{0}]",connected_client);
            connected_client--;
        }
    }
}
