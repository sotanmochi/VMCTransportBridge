using UnityEngine;
using MessagePack;
using MessagePack.Resolvers;
using VMCTransportBridge.Serialization;

namespace TransportClient.Unity
{
    /// <summary>
    /// Entry point
    /// </summary>
    public class Startup : MonoBehaviour
    {
        [SerializeField] string serverAddress = "http://localhost:50051";
        [SerializeField] TransportClientUIView view;

        static bool serializerRegistered = false;

        TransportClientPresenter _presenter;
        TransportClient _client;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            if (!serializerRegistered)
            {
                StaticCompositeResolver.Instance.Register
                (
                    GrpcTransportGeneratedResolver.Instance,
                    GeneratedResolver.Instance,
                    StandardResolver.Instance
                );

                var option = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);

                MessagePackSerializer.DefaultOptions = option;
                serializerRegistered = true;
            }
        }

        void Awake()
        {
            var messageSerializer = new MessagePackMessageSerializer(MessagePackSerializer.DefaultOptions);
            _client = new TransportClient(messageSerializer, serverAddress);
            _presenter = new TransportClientPresenter(view, _client);
        }
        
        void OnDestroy()
        {
            _presenter.Dispose();
            _client.Dispose();
        }
    }
}