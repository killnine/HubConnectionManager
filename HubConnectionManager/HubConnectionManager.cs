using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace HubConnectionManager
{
    public class HubConnectionManager : IHubConnectionManager
    {
        private readonly Timer _retryTimer;
        private readonly HubConnection _hubConnection;

        public event Action<Exception> Error;
        public event Action<string> Received;
        public event Action Closed;
        public event Action Reconnecting;
        public event Action Reconnected;
        public event Action ConnectionSlow;
        public event Action<StateChange> StateChanged;

        private int _retryPeriod = 30000;
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

        private HubConnectionManager(string url)
        {
            _retryTimer = new Timer(async state => await RetryConnection(), null, Timeout.Infinite, RetryPeriod);
            _hubConnection = new HubConnection(url);
        }

        public static IHubConnectionManager GetHubConnectionmanager(string url)
        {
            IHubConnectionManager connectionManager = new HubConnectionManager(url);
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
            _hubConnection.Closed += () =>
            {
                if (Closed != null)
                {
                    Closed();
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
            _hubConnection.StateChanged += OnStateChanged;

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

        private void OnStateChanged(StateChange stateChange)
        {
            try
            {
                if (stateChange.NewState == ConnectionState.Disconnected)
                {
                    _retryTimer.Change(RetryPeriod, RetryPeriod);
                }
                else
                {
                    _retryTimer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }
            finally
            {
                //Bubble event up to higher-level subscribers;
                if (StateChanged != null)
                {
                    StateChanged(stateChange);
                }
            }
        }

        private async Task RetryConnection()
        {
            if (_hubConnection != null && _hubConnection.State == ConnectionState.Disconnected)
            {
                await _hubConnection.Start();
            }
        }
    }
}