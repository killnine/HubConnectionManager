using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace HubConnectionManager
{
    public interface IHubConnectionManager
    {
        int RetryPeriod { get; set; }
        ConnectionState State { get; }
        event Action<Exception> Error;
        event Action<string> Received;
        event Action Closed;
        event Action Reconnecting;
        event Action Reconnected;
        event Action ConnectionSlow;
        event Action<StateChange> StateChanged;
        Task Initialize();
        IHubProxy CreateHubProxy(string hubName);
    }
}