using System;
using System.Runtime.Remoting.Proxies;
using System.Threading;
using System.Threading.Tasks;
using HubConnectionManager;
using Microsoft.AspNet.SignalR.Client;

namespace SignalR.Client
{
    class Program
    {
        private static HubConnectionManager.IHubConnectionManager _manager;
        private static HubConnection _connection;

        static void Main(string[] args)
        {
            string CLIENT_HUB = "ClientHub";
            string WORKING_URL = "http://localhost:6790";
            string BROKEN_URL = "http://invalid:6790";
            

            Console.WriteLine("Choose connection method:");
            Console.WriteLine("1) Attempt successful connect");
            Console.WriteLine("2) Attempt unsuccessful connect (connect to non-existent host).");

            var keyPress = Console.ReadKey();
            switch (keyPress.Key)
            {
                case ConsoleKey.D1:
                    _manager = HubConnectionManager.HubConnectionManager.GetHubConnectionmanager(WORKING_URL);
                    _connection = new HubConnection(WORKING_URL);
                    break;
                case ConsoleKey.D2:
                    _manager = HubConnectionManager.HubConnectionManager.GetHubConnectionmanager(BROKEN_URL);
                    _connection = new HubConnection(BROKEN_URL);
                    break;
                default:
                    Console.WriteLine("Invalid Selection! Press any key");
                    break;
            }

            Console.WriteLine("Attmepting to connect to SignalR endpoint...");
            
            IHubProxy hubProxy = _manager.CreateHubProxy(CLIENT_HUB);
            SetupNotifications(_manager);
            _manager.Initialize().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Console.WriteLine("There was an error opening the connection.");
                }
                else
                {
                    Console.WriteLine("Connected.");
                }
            }).Wait();
           
            while (true)
            {
                if (_manager.State == ConnectionState.Connected)
                {
                    try
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(hubProxy.Invoke<string>("Greetings", "User").Result);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    catch
                    { }
                }

                //Chill
                Thread.Sleep(5000);
            }
        }

        private static void SetupNotifications(IHubConnectionManager manager)
        {
            manager.StateChanged += change => Console.WriteLine("State changed to:" + change.NewState);
        }
    }
}
