using System;
using System.Threading.Tasks;
using UnityEngine;

namespace TransportClient.Unity
{
    public class TransportClientPresenter : IDisposable
    {
        private readonly TransportClientUIView _view;
        private readonly TransportClient _transportClient;
        
        public TransportClientPresenter(TransportClientUIView view, TransportClient transportClient)
        {
            _transportClient = transportClient;
            _view = view;
            _view.OnRaisedConnectEvent += ConnectEventHandler;
            _view.OnRaisedDisconnectEvent += DisconnectEventHandler;
            _view.OnRaisedSendMessageEvent += SendMessageEventHandler;
        }
        
        public void Dispose()
        {
            _view.OnRaisedConnectEvent -= ConnectEventHandler;
            _view.OnRaisedDisconnectEvent -= DisconnectEventHandler;
            _view.OnRaisedSendMessageEvent -= SendMessageEventHandler;
        }
        
        private void ConnectEventHandler()
        {
            Task.Run(async() => 
            {
                var connected = await _transportClient.ConnectAsync();
                Debug.Log($"ConnectAsync: {connected}");
            });
        }
        
        private void DisconnectEventHandler()
        {
            Task.Run(async() => await _transportClient.DisconnectAsync());
        }
        
        private void SendMessageEventHandler(string message)
        {
            _transportClient.SendMessage(message);
        }
    }
}