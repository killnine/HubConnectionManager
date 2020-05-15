using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using System;
using System.Threading.Tasks;

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
            var connectionManager = new HubConnectionManager(url);
            return connectionManager;
        }

        public static IHubConnectionManager GetHubConnectionManager(HubConnection hubConnection)
        {
            var connectionManager = new HubConnectionManager(hubConnection);
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
            Received?.Invoke(data);
        }

        private async void OnClosed()
        {
            Closed?.Invoke();
            await RetryConnection();
        }

        private async Task RetryConnection()
        {
            await Task.Delay(RetryPeriod);
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
            Reconnecting?.Invoke();
        }

        private void OnReconnected()
        {
            Reconnected?.Invoke();
        }

        private void OnConnectionSlow()
        {
            ConnectionSlow?.Invoke();
        }

        private void OnError(Exception error)
        {
            Error?.Invoke(error);
        }

        private void OnStateChanged(StateChange stateChange)
        {
            StateChanged?.Invoke(stateChange);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}