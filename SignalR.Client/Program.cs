using System;
using System.Threading;
using Microsoft.AspNet.SignalR.Client;

namespace HubConnectionManager.SignalR.Client
{
    class Program
    {
        private static IHubConnectionManager _manager;
        
        static void Main(string[] args)
        {
            string CLIENT_HUB = "ClientHub";
            string WORKING_URL = "http://localhost:6790";
            
            _manager = HubConnectionManager.GetHubConnectionManager(WORKING_URL);

            Console.WriteLine("Attmepting to connect to SignalR endpoint...");
            
            IHubProxy hubProxy = _manager.CreateHubProxy(CLIENT_HUB);
            SetupNotifications(_manager);
            _manager.Initialize().ContinueWith(task => Console.WriteLine(task.IsFaulted ? "There was an error opening the connection." : "Connected.")).Wait();
           
            while (true)
            {
                if (_manager.State == ConnectionState.Connected)
                {
                    try
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(hubProxy.Invoke<string>("Greetings", "User").Result);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Connected via " + _manager.ConnectionType.Name);
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
