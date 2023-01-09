#if ENABLE_MONO || ENABLE_IL2CPP
#define UNITY_ENGINE
#endif

using System;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Realtime.Extension;

namespace VMCTransportBridge.Transports.PhotonRealtime
{
    public sealed class PhotonRealtimeConnectParameters
    {
        public AppSettings AppSettings;
    }
    
    public sealed class PhotonRealtimeJoinParameters
    {
        public string RoomName;
        public RoomOptions RoomOptions;
        public TypedLobby TypedLobby;
        public string[] ExpectedUsers;
    }

    public partial class PhotonRealtimeTransport : ITransport, IOnEventCallback
    {
        public static readonly byte VMCTransportBridgeEventCode = 128;
        
        public event Action<byte[]> OnReceiveMessage;

        public bool IsConnected => _connected && _joined;
        public int ClientId => _photonRealtimeClient.NetworkingClient.LocalPlayer.ActorNumber;
        
        // public PhotonRealtimeClient PhotonRealtimeClient => _photonRealtimeClient;
        public string PunVersion => _photonRealtimeClient.PunVersion;
        public AppSettings AppSettings => _connectParameters.AppSettings;
        
        private readonly PhotonRealtimeClient _photonRealtimeClient;
        private readonly PhotonRealtimeConnectParameters _connectParameters;
        private readonly PhotonRealtimeJoinParameters _joinParameters;
        private readonly RaiseEventOptions _raiseEventOptions;
        
        private bool _connected;
        private bool _joined;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="punVersion"></param>
        /// <param name="client"></param>
        /// <param name="protocol"></param>
        public PhotonRealtimeTransport
        (
            string punVersion,
            PhotonRealtimeConnectParameters connectParameters,
            PhotonRealtimeJoinParameters joinParameters,
            ConnectionProtocol protocol = ConnectionProtocol.Udp,
            ReceiverGroup receiverGroup = ReceiverGroup.Others,
            bool isBackgroundThread = false
        )
        {
            _connectParameters = connectParameters;
            _joinParameters = joinParameters;

            _raiseEventOptions = new RaiseEventOptions
            {
                Receivers = receiverGroup,
                CachingOption = EventCaching.DoNotCache,
            };

            _photonRealtimeClient = new PhotonRealtimeClient(punVersion, protocol, isBackgroundThread);
            _photonRealtimeClient.NetworkingClient.AddCallbackTarget(this);
        }
        
        /// <summary>
        /// DisposeAsync
        /// </summary>
        /// <returns></returns>
        public async Task DisposeAsync()
        {
            await DisconnectAsyncCore();
            _photonRealtimeClient?.Dispose();
            _photonRealtimeClient?.NetworkingClient.RemoveCallbackTarget(this);
            Log("[PhotonRealtimeTransport] Disposed");
        }
        
        /// <summary>
        /// ConnectAsync
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ConnectAsync(string roomId = "")
        {
            if (!string.IsNullOrEmpty(roomId))
            {
                _joinParameters.RoomName = roomId;
            }
            await ConnectAsyncCore(_connectParameters);
            await JoinAsync(_joinParameters);
            return IsConnected;
        }
        
        /// <summary>
        /// DisconnectAsync
        /// </summary>
        /// <returns></returns>
        public async Task DisconnectAsync()
        {
            await DisconnectAsyncCore();
        }
        
        /// <summary>
        /// SendAsync
        /// </summary>
        /// <param name="serializedMessage"></param>
        /// <returns></returns>
        public async Task SendAsync(ArraySegment<byte> serializedMessage)
        {
            _photonRealtimeClient.RaiseEvent(VMCTransportBridgeEventCode, serializedMessage.Array, _raiseEventOptions, SendOptions.SendReliable);
        }
        
        /// <summary>
        /// Event handler
        /// </summary>
        /// <param name="eventData"></param>
        void IOnEventCallback.OnEvent(EventData eventData)
        {           
            if (eventData.Code == VMCTransportBridgeEventCode)
            {
                // Log($"[PhotonRealtimeTransport] OnEvent - Sender:{eventData.Sender}, EventCode:{eventData.Code}");
                var serializedMessage = (byte[])eventData.CustomData;
                OnReceiveMessage?.Invoke(serializedMessage);
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
            => System.Console.WriteLine(message);
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
            => System.Console.WriteLine($"[WARNING] {message}");
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
            => System.Console.WriteLine($"[ERROR] {message}");
#endif
    }
}