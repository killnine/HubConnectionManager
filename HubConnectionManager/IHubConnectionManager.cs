using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;

namespace HubConnectionManager
{
    public interface IHubConnectionManager : IDisposable
    {
        int RetryPeriod { get; set; }
        ConnectionState State { get; }
        IClientTransport ConnectionType { get; }
        event Action<Exception> Error;
        event Action<string> Received;
        event Action Closed;
        event Action Reconnecting;
        event Action Reconnected;
        event Action ConnectionSlow;
        event Action<StateChange> StateChanged;
        IHubProxy CreateHubProxy(string hubName);
        Task Initialize();
        void Stop();
    }
}