using System;
using UnityEngine;
using UnityEngine.UI;

namespace TransportClient.Unity
{
    public class TransportClientUIView : MonoBehaviour
    {
        [SerializeField] private Button connectionButton;
        [SerializeField] private InputField messageInputField;
        [SerializeField] private Button sendMessageButton;
        
        public event Action OnRaisedConnectEvent;
        public event Action OnRaisedDisconnectEvent;
        public event Action<string> OnRaisedSendMessageEvent;
        
        void Awake()
        {
            connectionButton.onClick.AddListener(OnClickConnectButton);
            sendMessageButton.onClick.AddListener(OnClickSendMessageButton);
        }
        
        private void OnClickConnectButton()
        {
            OnRaisedConnectEvent?.Invoke();
        }
        
        private void OnClickSendMessageButton()
        {
            OnRaisedSendMessageEvent?.Invoke(messageInputField.text);
        }
    }
}