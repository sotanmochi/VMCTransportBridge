#if ENABLE_MONO || ENABLE_IL2CPP
#define UNITY_ENGINE
#endif

using System;
using System.Threading;
using ExitGames.Client.Photon;

namespace Photon.Realtime.Extension
{
    public sealed class PhotonRealtimeClient : IDisposable
    {
        public Action OnUpdate { get; set; }

        public string PunVersion => _punVersion;
        public LoadBalancingClient NetworkingClient => _networkingClient;

        private readonly LoadBalancingClient _networkingClient;
        private readonly string _punVersion;

        private readonly Thread _updateThread;
        private readonly CancellationTokenSource _cts;
        private readonly int _sleepTimeMilliseconds = 10;

        // Networking / Timing related settings
        internal int _intervalDispatch = 10; // Interval between DispatchIncomingCommands() calls
        internal int _lastDispatch = Environment.TickCount;
        internal int _intervalSend = 50; // Interval between SendOutgoingCommands() calls
        internal int _lastSend = Environment.TickCount;

        // Update control variables for UI
        internal int _lastUiUpdate = Environment.TickCount;
        private int _intervalUiUpdate = 1000; // The UI update interval. this demo has a low update interval, cause we update also on event and status change

        public PhotonRealtimeClient
        (
            string punVersion, 
            ConnectionProtocol protocol = ConnectionProtocol.Udp, 
            bool isBackgroundThread = false, 
            int sleepTimeMilliseconds = 10, 
            int intervalDispatch = 10, 
            int intervalSend = 50, 
            int intervalUiUpdate = 1000
        )
        {
            _punVersion = punVersion;

            _sleepTimeMilliseconds = sleepTimeMilliseconds;
            _intervalDispatch = intervalDispatch;
            _intervalSend = intervalSend;
            _intervalUiUpdate = intervalUiUpdate;

            _cts = new CancellationTokenSource();

            _networkingClient = new LoadBalancingClient(protocol);

            _updateThread = new Thread(UpdateLoop);
            _updateThread.IsBackground = isBackgroundThread;
            _updateThread.Start();

            Log($"[PhotonRealtimeClient] Thread ID of the main thread: {Thread.CurrentThread.ManagedThreadId}");
            Log($"[PhotonRealtimeClient] Thread ID of the update thread: {_updateThread.ManagedThreadId}");
            Log($"[PhotonRealtimeClient] The update thread is background: {_updateThread.IsBackground}");
        }

        public void Dispose()
        {
            _cts.Cancel();
            Log("[PhotonRealtimeClient] Disposed");
        }

        public bool ConnectUsingSettings(AppSettings appSettings)
        {
            if (_networkingClient.LoadBalancingPeer.PeerState != PeerStateValue.Disconnected)
            {
                LogWarning("[PhotonRealtimeClient] ConnectUsingSettings() failed. Can only connect while in state 'Disconnected'. Current state: " + _networkingClient.LoadBalancingPeer.PeerState);
                return false;
            }

            _networkingClient.LoadBalancingPeer.TransportProtocol = appSettings.Protocol;
            _networkingClient.ExpectedProtocol = null;
            _networkingClient.EnableProtocolFallback = appSettings.EnableProtocolFallback;
            _networkingClient.AuthMode = appSettings.AuthMode;

            _networkingClient.AppId = appSettings.AppIdRealtime;
            _networkingClient.AppVersion = string.Format("{0}_{1}", appSettings.AppVersion, _punVersion);

            _networkingClient.EnableLobbyStatistics = appSettings.EnableLobbyStatistics;
            _networkingClient.ProxyServerAddress = appSettings.ProxyServer;

            if (appSettings.IsMasterServerAddress)
            {
                if (_networkingClient.AuthValues == null)
                {
                    _networkingClient.AuthValues = new AuthenticationValues(Guid.NewGuid().ToString());
                }
                else if (string.IsNullOrEmpty(_networkingClient.AuthValues.UserId))
                {
                    _networkingClient.AuthValues.UserId = Guid.NewGuid().ToString();
                }

                var masterServerAddress = appSettings.Server;
                var port = appSettings.Port;

                _networkingClient.IsUsingNameServer = false;
                _networkingClient.MasterServerAddress = (port == 0) ? masterServerAddress : masterServerAddress + ":" + port;

                Log($"[PhotonRealtimeClient] ConnectToMasterServer");
                return _networkingClient.ConnectToMasterServer();
            }

            _networkingClient.NameServerPortInAppSettings = appSettings.Port;
            if (!appSettings.IsDefaultNameServer)
            {
                _networkingClient.NameServerHost = appSettings.Server;
            }

            // ConnectToBestCloudServer
            if (appSettings.IsBestRegion)
            {
                Log($"[PhotonRealtimeClient] ConnectToNameServer");
                return _networkingClient.ConnectToNameServer();
            }

            // ConnectToRegion
            var region = appSettings.FixedRegion;
            if (!string.IsNullOrEmpty(region))
            {
                Log($"[PhotonRealtimeClient] ConnectToRegionMaster");
                return _networkingClient.ConnectToRegionMaster(region);
            }

            return false;
        }

        public void Disconnect()
        {
            _networkingClient.Disconnect();
        }

        public bool JoinOrCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby, string[] expectedUsers = null)
        {
            if (_networkingClient.Server != ServerConnection.MasterServer || !_networkingClient.IsConnectedAndReady)
            {
                LogError("[PhotonRealtimeClient] JoinOrCreateRoom failed. Client is on " + _networkingClient.Server + " (must be Master Server for matchmaking)" + (_networkingClient.IsConnectedAndReady ? " and ready" : "but not ready for operations (State: " + _networkingClient.State + ")") + ". Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
                return false;
            }
            if (string.IsNullOrEmpty(roomName))
            {
                LogError("[PhotonRealtimeClient] JoinOrCreateRoom failed. A roomname is required. If you don't know one, how will you join?");
                return false;
            }

            typedLobby = typedLobby ?? ((_networkingClient.InLobby) ? _networkingClient.CurrentLobby : null);  // use given lobby, or active lobby (if any active) or none

            EnterRoomParams opParams = new EnterRoomParams();
            opParams.RoomName = roomName;
            opParams.RoomOptions = roomOptions;
            opParams.Lobby = typedLobby;
            opParams.PlayerProperties = _networkingClient.LocalPlayer.CustomProperties;
            opParams.ExpectedUsers = expectedUsers;

            return _networkingClient.OpJoinOrCreateRoom(opParams);
        }

        public void LeaveRoom()
        {
            var becomeInactive = true;
            var currentRoom = _networkingClient.CurrentRoom;
            if (currentRoom is null)
            {
                LogWarning("[PhotonRealtimeClient] NetworkingClient.CurrentRoom is null. You don't have to call LeaveRoom() when you're not in one. State: " + _networkingClient.State);
            }
            else
            {
                becomeInactive = becomeInactive && currentRoom.PlayerTtl != 0; // in a room with playerTTL == 0, the operation "leave" will never turn a client inactive
            }
            _networkingClient.OpLeaveRoom(becomeInactive);
        }

        public bool RaiseEvent(byte eventCode, object eventContent, RaiseEventOptions raiseEventOptions, SendOptions sendOptions)
        {
            var inRoom = (_networkingClient.State == ClientState.Joined);

            if (!inRoom || eventCode >= 200)
            {
                LogWarning($"[PhotonRealtimeClient] RaiseEvent({eventCode}) failed. Your event is not being sent! Check if your are in a Room and the eventCode must be less than 200 (0..199).");
                return false;
            }

            return _networkingClient.OpRaiseEvent(eventCode, eventContent, raiseEventOptions, sendOptions);
        }

        private void UpdateLoop()
        {
            while (!_cts.IsCancellationRequested)
            {
                Update();
                Thread.Sleep(_sleepTimeMilliseconds);
            }
        }

        private void Update()
        {
            if (Environment.TickCount - _lastDispatch > _intervalDispatch)
            {
                _lastDispatch = Environment.TickCount;
                _networkingClient.LoadBalancingPeer.DispatchIncomingCommands();
            }

            if (Environment.TickCount - _lastSend > _intervalSend)
            {
                _lastSend = Environment.TickCount;
                _networkingClient.LoadBalancingPeer.SendOutgoingCommands(); // will send pending, outgoing commands
            }

            // Update call for windows phone UI-Thread
            if (Environment.TickCount - _lastUiUpdate > _intervalUiUpdate)
            {
                _lastUiUpdate = Environment.TickCount;
                OnUpdate?.Invoke();
            }
        }

        /// <summary>
        /// Logs a message to the console 
        /// only when DEVELOPMENT_BUILD or UNITY_EDITOR is defined.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [
            System.Diagnostics.Conditional("DEVELOPMENT_BUILD"), 
            System.Diagnostics.Conditional("UNITY_EDITOR"),
        ]
        private void Log(object message)
#if UNITY_ENGINE
            => UnityEngine.Debug.Log(message);
#else
            => Console.WriteLine(message);
#endif

        /// <summary>
        /// Logs a warning message to the console 
        /// only when DEVELOPMENT_BUILD or UNITY_EDITOR is defined.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [
            System.Diagnostics.Conditional("DEVELOPMENT_BUILD"), 
            System.Diagnostics.Conditional("UNITY_EDITOR"),
        ]
        private void LogWarning(object message)
#if UNITY_ENGINE
            => UnityEngine.Debug.LogWarning(message);
#else
            => Console.WriteLine($"[WARNING]{message}");
#endif

        /// <summary>
        /// Logs an error message to the console 
        /// only when DEVELOPMENT_BUILD or UNITY_EDITOR is defined.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [
            System.Diagnostics.Conditional("DEVELOPMENT_BUILD"), 
            System.Diagnostics.Conditional("UNITY_EDITOR"),
        ]
        private void LogError(object message)
#if UNITY_ENGINE
            => UnityEngine.Debug.LogError(message);
#else
            => Console.WriteLine($"[ERROR]{message}");
#endif
    }
}