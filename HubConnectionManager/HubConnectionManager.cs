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

        public IHubProxy CreateHubProxy(string hubName)
        {
            if (string.IsNullOrEmpty(hubName))
            {
                throw new ArgumentNullException("hubName");
            }

            return _hubConnection.CreateHubProxy(hubName);
        }

        public async Task Initialize()
        {
            _hubConnection.Received += OnReceived;
            _hubConnection.Closed += OnClosed;
            _hubConnection.Reconnecting += OnReconnecting;
            _hubConnection.Reconnected += OnReconnected;
            _hubConnection.ConnectionSlow += OnConnectionSlow;
            _hubConnection.Error += OnError;
            _hubConnection.StateChanged += OnStateChanged;

            await _hubConnection.Start();
        }

        public void Stop()
        {
            _hubConnection.Received -= OnReceived;
            _hubConnection.Closed -= OnClosed;
            _hubConnection.Reconnecting -= OnReconnecting;
            _hubConnection.Reconnected -= OnReconnected;
            _hubConnection.ConnectionSlow -= OnConnectionSlow;
            _hubConnection.Error -= OnError;
            _hubConnection.StateChanged -= OnStateChanged;

            _hubConnection.Stop();
        }

        private void OnReceived(string data)
        {
            if (Received != null)
            {
                Received(data);
            }
        }

        private async void OnClosed()
        {
            if (Closed != null)
            {
                Closed();
            }
            await RetryConnection();
        }

        private async Task RetryConnection()
        {
            await TaskEx.Delay(RetryPeriod);
            try
            {
                await _hubConnection.Start();
            }
            catch (Exception ex)
            {
                //NOTE: Unable to connect...again.
            }
        }

        private void OnReconnecting()
        {
            if (Reconnecting != null)
            {
                Reconnecting();
            }
        }

        private void OnReconnected()
        {
            if (Reconnected != null)
            {
                Reconnected();
            }
        }

        private void OnConnectionSlow()
        {
            if (ConnectionSlow != null)
            {
                ConnectionSlow();
            }
        }

        private void OnError(Exception error)
        {
            if (Error != null)
            {
                Error(error);
            }
        }

        private void OnStateChanged(StateChange stateChange)
        {
            if (StateChanged != null)
            {
                StateChanged(stateChange);
            }
        }
    }
}