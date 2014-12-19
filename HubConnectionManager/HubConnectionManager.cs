using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;

namespace HubConnectionManager
{
    public class HubConnectionManager : IHubConnectionManager
    {
        private readonly HubConnection _hubConnection;
        private int _retryPeriod = 10000;

        public event Action<Exception> Error;
        public event Action<string> Received;
        public event Action Closed;
        public event Action Reconnecting;
        public event Action Reconnected;
        public event Action ConnectionSlow;
        public event Action<StateChange> StateChanged;

        public int RetryPeriod
        {
            get { return _retryPeriod; }
            set
            {
                //Sir, you don't want a negative retry period
                if (RetryPeriod <= 0)
                {
                    return;
                }

                _retryPeriod = value;
            }
        }

        public ConnectionState State
        {
            get { return _hubConnection.State; }
        }

        public IClientTransport ConnectionType
        {
            get { return _hubConnection.Transport; }
        }

        private HubConnectionManager(string url)
        {
            _hubConnection = new HubConnection(url);
        }

        private HubConnectionManager(HubConnection hubConnection)
        {
            _hubConnection = hubConnection;
        }

        public static IHubConnectionManager GetHubConnectionManager(string url)
        {
            IHubConnectionManager connectionManager = new HubConnectionManager(url);
            return connectionManager;
        }

        public static IHubConnectionManager GetHubConnectionManager(HubConnection hubConnection)
        {
            IHubConnectionManager connectionManager = new HubConnectionManager(hubConnection);
            return connectionManager;
        }

        public async Task Initialize()
        {
            _hubConnection.Received += s =>
            {
                if (Received != null)
                {
                    Received(s);
                }
            };
            _hubConnection.Closed += async () =>
            {
                if (Closed != null)
                {
                    Closed();
                }

                await TaskEx.Delay(RetryPeriod);
                try
                {
                    await _hubConnection.Start();
                }
                catch (Exception ex)
                {
                    //NOTE: Unable to connect...again.
                }
            };
            _hubConnection.Reconnecting += () =>
            {
                if (Reconnecting != null)
                {
                    Reconnecting();
                }
            };
            _hubConnection.Reconnected += () =>
            {
                if (Reconnected != null)
                {
                    Reconnected();
                }
            };
            _hubConnection.ConnectionSlow += () =>
            {
                if (ConnectionSlow != null)
                {
                    ConnectionSlow();
                }
            };
            _hubConnection.Error += e =>
            {
                if (Error != null)
                {
                    Error(e);
                }
            };
            _hubConnection.StateChanged += e =>
            {
                if (StateChanged != null)
                {
                    StateChanged(e);
                }
            };
            
            await _hubConnection.Start();
        }

        public IHubProxy CreateHubProxy(string hubName)
        {
            if (string.IsNullOrEmpty(hubName))
            {
                throw new ArgumentNullException("hubName");
            }
            
            return _hubConnection.CreateHubProxy(hubName);
        }
    }
}