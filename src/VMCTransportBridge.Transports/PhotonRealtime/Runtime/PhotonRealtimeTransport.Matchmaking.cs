using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Photon.Realtime;

namespace VMCTransportBridge.Transports.PhotonRealtime
{
    public partial class PhotonRealtimeTransport : IMatchmakingCallbacks
    {
        public event Action OnJoinedRoom;
        public event Action<(short ReturnCode, string message)> OnJoinRoomFailed;
        public event Action OnLeftRoom;
        
        private TaskCompletionSource<bool> _onJoinedRoom = new TaskCompletionSource<bool>();
        private TaskCompletionSource<bool> _onJoinRoomFailed = new TaskCompletionSource<bool>();
        private TaskCompletionSource<bool> _onLeftRoom = new TaskCompletionSource<bool>();
        
        /// <summary>
        /// JoinAsync
        /// </summary>
        /// <param name="joinParameters"></param>
        /// <returns></returns>
        public async Task<bool> JoinAsync(PhotonRealtimeJoinParameters joinParameters)
        {
            _onJoinedRoom = new TaskCompletionSource<bool>();
            _onJoinRoomFailed = new TaskCompletionSource<bool>();

            var task = Task.WhenAny(_onJoinedRoom.Task, _onJoinRoomFailed.Task);
            _photonRealtimeClient.JoinOrCreateRoom(joinParameters.RoomName, joinParameters.RoomOptions, joinParameters.TypedLobby, joinParameters.ExpectedUsers);
            await task;

            return _joined;
        }
        
        /// <summary>
        /// LeaveAsync
        /// </summary>
        /// <returns></returns>
        public async Task LeaveAsync()
        {
            _onLeftRoom = new TaskCompletionSource<bool>();
            _photonRealtimeClient.LeaveRoom();
            await _onLeftRoom.Task;
        }
        
#region Callbacks

        void IMatchmakingCallbacks.OnJoinedRoom()
        {
            Log($"[PhotonRealtimeTransport] OnJoinedRoom");

            OnJoinedRoom?.Invoke();

            _joined = true;
            _onJoinedRoom?.TrySetResult(true);
        }

        void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
        {
            Log("[PhotonRealtimeTransport] OnJoinRoomFailed");

            OnJoinRoomFailed?.Invoke((returnCode, message));

            _joined = false;
            _onJoinRoomFailed?.TrySetResult(true);
        }

        void IMatchmakingCallbacks.OnLeftRoom()
        {
            Log("[PhotonRealtimeTransport] OnLeftRoom");

            OnLeftRoom?.Invoke();

            _joined = false;
            _onLeftRoom?.TrySetResult(true);
        }

        void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList)
        {
        }

        void IMatchmakingCallbacks.OnCreatedRoom()
        {
        }

        void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
        {
        }

        void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
        {
        }

#endregion
    }
}