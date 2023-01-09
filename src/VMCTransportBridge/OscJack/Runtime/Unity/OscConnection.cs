// OSC Jack - Open Sound Control plugin for Unity
// https://github.com/keijiro/OscJack

#if UNITY_2021_3_OR_NEWER // Version 2.1.0
using UnityEngine;

namespace OscJack
{
    public enum OscConnectionType { Udp }

    [CreateAssetMenu(fileName = "OscConnection",
                     menuName = "ScriptableObjects/OSC Jack/Connection")]
    public sealed class OscConnection : ScriptableObject
    {
        public OscConnectionType type = OscConnectionType.Udp;
        public string host = "127.0.0.1";
        public int port = 8000;
    }
}
#endif