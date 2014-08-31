using System;
using System.Threading;
using Microsoft.Owin.Hosting;

namespace HubConnectionManager.SignalR.Server
{
    class Program
    {
        private const string URL = "http://localhost:6790";
        private static IDisposable WebAppStart { get; set; }

        private static void Main(string[] args)
        {
            try
            {
                WebAppStart = WebApp.Start<Startup>(URL);
                Console.WriteLine("Server running on {0}", URL);
                Console.ReadKey(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled exception occcurred! " + ex.Message);
            }
            finally
            {
                Console.Write("Closing...");
                Thread.Sleep(1000);
            }
        }
    }
}
