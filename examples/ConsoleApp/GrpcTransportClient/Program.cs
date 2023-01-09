using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using VMCTransportBridge.Serialization;

namespace GrpcTransportClient;

public class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine($"///////////////////////////////////////////");
        Console.WriteLine($"///   VMCTransportBridge sample apps    ///");
        Console.WriteLine($"///        Grpc Transport Client        ///");
        Console.WriteLine($"///////////////////////////////////////////");
        
        // The port number must match the port of the gRPC server.
        using var channel = GrpcChannel.ForAddress("http://localhost:50051");
        // using var channel = GrpcChannel.ForAddress("https://localhost:50052");
        
        Console.WriteLine($"-------------------------------------------");
        
        var messageSerializer = new MessagePackMessageSerializer();
        var startup = new Startup(channel, messageSerializer);
        await startup.StartAsync();
        startup.Dispose();
        
        Console.WriteLine($"//////////////////////////////////////////////");
        Console.WriteLine($"///      End of Grpc Transport Client      ///");
        Console.WriteLine($"//////////////////////////////////////////////");
    }
}