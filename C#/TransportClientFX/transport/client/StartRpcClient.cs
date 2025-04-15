using System;
using System.Configuration;
using System.Net.Mime;
using System.Windows.Forms;
using proiect.view;
using TransportServices.transport.services;
using TransportClient.transport.client;
using TransportNetworking.transport.network.rpcprotocol;


namespace TransportClient.transport.client
{
    public class StartRpcClient
    {
        private static int defaultChatPort = 55555;
        private static string defaultServer = "localhost";

        public void Start()
        {
            Console.WriteLine("In start");

            string serverIP = ConfigurationManager.AppSettings["TransportServer.Host"] ?? defaultServer;
            int serverPort = defaultChatPort;

            try
            {
                serverPort = int.Parse(ConfigurationManager.AppSettings["TransportServer.Port"]);
            }
            catch (FormatException ex)
            {
                Console.Error.WriteLine("Wrong port number " + ex.Message);
                Console.WriteLine("Using default port: " + defaultChatPort);
            }
            Console.WriteLine("Using server IP " + serverIP);
            Console.WriteLine("Using server port " + serverPort);

            ITransportServices server = new TransportServicesRpcProxy(serverIP, serverPort);
            //ITransportServices server = new TransportServicesJsonProxy(serverIP, serverPort);
            
            Login loginForm = new Login();
            loginForm.SetServer(server);

            Angajat mainForm = new Angajat(server);

            loginForm.SetMainForm(mainForm);
            Application.Run(loginForm);
        }
    }
}