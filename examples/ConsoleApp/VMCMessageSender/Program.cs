using System;
using System.Net;
using System.Reflection;

[assembly: AssemblyVersion("1.0.*")]
namespace VMCMessageSender
{
    class Program
    {
        static ushort defaultPort = 39540;

        static void Main()
        {
            Console.WriteLine($"///////////////////////////////////////////");
            Console.WriteLine($"///   VMCTransportBridge sample apps    ///");
            Console.WriteLine($"///         VMC Message Sender          ///");
            Console.WriteLine($"///////////////////////////////////////////");

            var assemblyName = Assembly.GetExecutingAssembly().GetName();
            var version = assemblyName.Version;

            Console.WriteLine($"--------------------------------------------------");
            Console.WriteLine($"Version: {version}");
            Console.WriteLine($"--------------------------------------------------");

            var args = System.Environment.GetCommandLineArgs();

            if (args.Length == 1)
            {
                Console.WriteLine($"");
                Console.WriteLine($"Usage:");
                Console.WriteLine($"  VMCMessageSender.exe --port <port number>");
                Console.WriteLine($"");
                return;
            }

            var port = defaultPort;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--port")
                {
                    if (UInt16.TryParse(args[i + 1], out var result))
                    {
                        port = result;
                    }
                }
            }

            Console.WriteLine($"--------------------------------------------------");
            Console.WriteLine($"OscPort: {port}");
            Console.WriteLine($"--------------------------------------------------");

            var destinationAddress = IPAddress.Loopback.ToString();
            new Startup(destinationAddress, port);
        }
    }
}