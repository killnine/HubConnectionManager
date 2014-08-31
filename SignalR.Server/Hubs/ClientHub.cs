using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace HubConnectionManager.SignalR.Server.Hubs
{
    [HubName("ClientHub")]
    public class ClientHub : Hub
    {
        public override Task OnConnected()
        {
            Console.WriteLine("User connected: " + Context.ConnectionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Console.WriteLine("User disconnected: " + Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }

        public string Greetings(string name)
        {
            return string.IsNullOrEmpty(name) ? "Hello world!" : string.Format("Hello {0}!", name);
        }
    }
}
