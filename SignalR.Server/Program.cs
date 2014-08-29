using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;

namespace SignalR.Server
{
    class Program
    {
        private const string URL = "http://localhost:6790";
        private static IDisposable WebAppStart { get; set; }

        private bool IsRunningAsElevatedUser
        {
            get
            {
                bool isElevated = false;
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                if (identity != null)
                {
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
                }

                return isElevated;
            }
        }

        private static void Main(string[] args)
        {
            try
            {
                WebAppStart = WebApp.Start<Startup>(URL);
                Console.WriteLine("Server running on {0}", URL);
                Console.ReadKey(true);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Unhandled exception occcurred! " + ex.Message);   
            }
        }
    }
}
