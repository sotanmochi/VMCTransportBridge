// This code is available for OscJack version 2.1.0.
// https://github.com/sotanmochi/OscJack/tree/dotnet
// https://github.com/sotanmochi/OscJack/releases/tag/2.1.0

using System.Net.Sockets;

namespace OscJack.Extensions
{
    public static partial class OscClientExtensions
    {
        public static void Send(this OscClient client, string address, string element1, float element2)
        {            
            client.Encoder.Clear();
            client.Encoder.Append(address);
            client.Encoder.Append(",sf");
            client.Encoder.Append(element1);
            client.Encoder.Append(element2);
            client.Socket.Send(client.Encoder.Buffer, client.Encoder.Length, SocketFlags.None);
        }

        public static void Send(this OscClient client, string address, float element1, int element2)
        {            
            client.Encoder.Clear();
            client.Encoder.Append(address);
            client.Encoder.Append(",fi");
            client.Encoder.Append(element1);
            client.Encoder.Append(element2);
            client.Socket.Send(client.Encoder.Buffer, client.Encoder.Length, SocketFlags.None);
        }

        public static void Send(this OscClient client, string address, float element1, string element2)
        {            
            client.Encoder.Clear();
            client.Encoder.Append(address);
            client.Encoder.Append(",fs");
            client.Encoder.Append(element1);
            client.Encoder.Append(element2);
            client.Socket.Send(client.Encoder.Buffer, client.Encoder.Length, SocketFlags.None);
        }

        public static void Send(this OscClient client, string address, string element1, string element2)
        {            
            client.Encoder.Clear();
            client.Encoder.Append(address);
            client.Encoder.Append(",ss");
            client.Encoder.Append(element1);
            client.Encoder.Append(element2);
            client.Socket.Send(client.Encoder.Buffer, client.Encoder.Length, SocketFlags.None);
        }

        public static void Send(this OscClient client, string address, string element1, string element2, string element3)
        {            
            client.Encoder.Clear();
            client.Encoder.Append(address);
            client.Encoder.Append(",sss");
            client.Encoder.Append(element1);
            client.Encoder.Append(element2);
            client.Encoder.Append(element3);
            client.Socket.Send(client.Encoder.Buffer, client.Encoder.Length, SocketFlags.None);
        }

        public static void Send(this OscClient client, string address, string element1, string element2, int element3)
        {            
            client.Encoder.Clear();
            client.Encoder.Append(address);
            client.Encoder.Append(",ssi");
            client.Encoder.Append(element1);
            client.Encoder.Append(element2);
            client.Encoder.Append(element3);
            client.Socket.Send(client.Encoder.Buffer, client.Encoder.Length, SocketFlags.None);
        }

        public static void Send(this OscClient client, string address, string element1, float element2, int element3)
        {            
            client.Encoder.Clear();
            client.Encoder.Append(address);
            client.Encoder.Append(",sfi");
            client.Encoder.Append(element1);
            client.Encoder.Append(element2);
            client.Encoder.Append(element3);
            client.Socket.Send(client.Encoder.Buffer, client.Encoder.Length, SocketFlags.None);
        }

        public static void Send(this OscClient client, string address, int element1, string element2, int element3)
        {            
            client.Encoder.Clear();
            client.Encoder.Append(address);
            client.Encoder.Append(",isi");
            client.Encoder.Append(element1);
            client.Encoder.Append(element2);
            client.Encoder.Append(element3);
            client.Socket.Send(client.Encoder.Buffer, client.Encoder.Length, SocketFlags.None);
        }

        public static void Send(this OscClient client, string address, string element1, string element2, string element3, string element4)
        {            
            client.Encoder.Clear();
            client.Encoder.Append(address);
            client.Encoder.Append(",ssss");
            client.Encoder.Append(element1);
            client.Encoder.Append(element2);
            client.Encoder.Append(element3);
            client.Encoder.Append(element4);
            client.Socket.Send(client.Encoder.Buffer, client.Encoder.Length, SocketFlags.None);
        }

        public static void Send(this OscClient client, string address, string element1, string element2, string element3, int element4)
        {            
            client.Encoder.Clear();
            client.Encoder.Append(address);
            client.Encoder.Append(",sssi");
            client.Encoder.Append(element1);
            client.Encoder.Append(element2);
            client.Encoder.Append(element3);
            client.Encoder.Append(element4);
            client.Socket.Send(client.Encoder.Buffer, client.Encoder.Length, SocketFlags.None);
        }

        public static void Send(this OscClient client, string address, int element1, string element2, int element3, int element4)
        {            
            client.Encoder.Clear();
            client.Encoder.Append(address);
            client.Encoder.Append(",isii");
            client.Encoder.Append(element1);
            client.Encoder.Append(element2);
            client.Encoder.Append(element3);
            client.Encoder.Append(element4);
            client.Socket.Send(client.Encoder.Buffer, client.Encoder.Length, SocketFlags.None);
        }

        public static void Send(this OscClient client, string address, int element1, int element2, int element3, int element4, int element5)
        {            
            client.Encoder.Clear();
            client.Encoder.Append(address);
            client.Encoder.Append(",iiiii");
            client.Encoder.Append(element1);
            client.Encoder.Append(element2);
            client.Encoder.Append(element3);
            client.Encoder.Append(element4);
            client.Encoder.Append(element5);
            client.Socket.Send(client.Encoder.Buffer, client.Encoder.Length, SocketFlags.None);
        }

        public static void Send(this OscClient client, string address, string element1, 
                                float element2, float element3, float element4, float element5, float element6, float element7, float element8)
        {            
            client.Encoder.Clear();
            client.Encoder.Append(address);
            client.Encoder.Append(",sfffffff");
            client.Encoder.Append(element1);
            client.Encoder.Append(element2);
            client.Encoder.Append(element3);
            client.Encoder.Append(element4);
            client.Encoder.Append(element5);
            client.Encoder.Append(element6);
            client.Encoder.Append(element7);
            client.Encoder.Append(element8);
            client.Socket.Send(client.Encoder.Buffer, client.Encoder.Length, SocketFlags.None);
        }

        public static void Send(this OscClient client, string address, string element1, 
                                float element2, float element3, float element4, float element5, float element6, float element7, float element8, float element9)
        {            
            client.Encoder.Clear();
            client.Encoder.Append(address);
            client.Encoder.Append(",sffffffff");
            client.Encoder.Append(element1);
            client.Encoder.Append(element2);
            client.Encoder.Append(element3);
            client.Encoder.Append(element4);
            client.Encoder.Append(element5);
            client.Encoder.Append(element6);
            client.Encoder.Append(element7);
            client.Encoder.Append(element8);
            client.Encoder.Append(element9);
            client.Socket.Send(client.Encoder.Buffer, client.Encoder.Length, SocketFlags.None);
        }

        public static void Send(this OscClient client, string address, int element1, string element2,
                                int element3, int element4, int element5, float element6, float element7, float element8)
        {            
            client.Encoder.Clear();
            client.Encoder.Append(address);
            client.Encoder.Append(",isiiifff");
            client.Encoder.Append(element1);
            client.Encoder.Append(element2);
            client.Encoder.Append(element3);
            client.Encoder.Append(element4);
            client.Encoder.Append(element5);
            client.Encoder.Append(element6);
            client.Encoder.Append(element7);
            client.Encoder.Append(element8);
            client.Socket.Send(client.Encoder.Buffer, client.Encoder.Length, SocketFlags.None);
        }

        public static void Send(this OscClient client, string address, int element1, string element2,
                                int element3, int element4, int element5, float element6, float element7, float element8, int element9)
        {            
            client.Encoder.Clear();
            client.Encoder.Append(address);
            client.Encoder.Append(",isiiifffi");
            client.Encoder.Append(element1);
            client.Encoder.Append(element2);
            client.Encoder.Append(element3);
            client.Encoder.Append(element4);
            client.Encoder.Append(element5);
            client.Encoder.Append(element6);
            client.Encoder.Append(element7);
            client.Encoder.Append(element8);
            client.Encoder.Append(element9);
            client.Socket.Send(client.Encoder.Buffer, client.Encoder.Length, SocketFlags.None);
        }

        public static void Send(this OscClient client, string address, string element1, 
                                float element2, float element3, float element4, float element5, float element6, float element7, float element8, float element9, int element10)
        {            
            client.Encoder.Clear();
            client.Encoder.Append(address);
            client.Encoder.Append(",sffffffffi");
            client.Encoder.Append(element1);
            client.Encoder.Append(element2);
            client.Encoder.Append(element3);
            client.Encoder.Append(element4);
            client.Encoder.Append(element5);
            client.Encoder.Append(element6);
            client.Encoder.Append(element7);
            client.Encoder.Append(element8);
            client.Encoder.Append(element9);
            client.Encoder.Append(element10);
            client.Socket.Send(client.Encoder.Buffer, client.Encoder.Length, SocketFlags.None);
        }

        public static void Send(this OscClient client, string address, string element1, 
                                float element2, float element3, float element4, float element5, float element6, float element7, float element8, 
                                float element9, float element10, float element11, float element12)
        {            
            client.Encoder.Clear();
            client.Encoder.Append(address);
            client.Encoder.Append(",sfffffffffff");
            client.Encoder.Append(element1);
            client.Encoder.Append(element2);
            client.Encoder.Append(element3);
            client.Encoder.Append(element4);
            client.Encoder.Append(element5);
            client.Encoder.Append(element6);
            client.Encoder.Append(element7);
            client.Encoder.Append(element8);
            client.Encoder.Append(element9);
            client.Encoder.Append(element10);
            client.Encoder.Append(element11);
            client.Encoder.Append(element12);
            client.Socket.Send(client.Encoder.Buffer, client.Encoder.Length, SocketFlags.None);
        }

        public static void Send(this OscClient client, string address, string element1, 
                                float element2, float element3, float element4, float element5, float element6, float element7, float element8, 
                                float element9, float element10, float element11, float element12, int element13)
        {
            client.Encoder.Clear();
            client.Encoder.Append(address);
            client.Encoder.Append(",sfffffffffffi");
            client.Encoder.Append(element1);
            client.Encoder.Append(element2);
            client.Encoder.Append(element3);
            client.Encoder.Append(element4);
            client.Encoder.Append(element5);
            client.Encoder.Append(element6);
            client.Encoder.Append(element7);
            client.Encoder.Append(element8);
            client.Encoder.Append(element9);
            client.Encoder.Append(element10);
            client.Encoder.Append(element11);
            client.Encoder.Append(element12);
            client.Encoder.Append(element13);
            client.Socket.Send(client.Encoder.Buffer, client.Encoder.Length, SocketFlags.None);
        }

        public static void Send(this OscClient client, string address, string element1, 
                                float element2, float element3, float element4, float element5, float element6, float element7, float element8, 
                                float element9, float element10, float element11, float element12, float element13, float element14)
        {            
            client.Encoder.Clear();
            client.Encoder.Append(address);
            client.Encoder.Append(",sfffffffffffff");
            client.Encoder.Append(element1);
            client.Encoder.Append(element2);
            client.Encoder.Append(element3);
            client.Encoder.Append(element4);
            client.Encoder.Append(element5);
            client.Encoder.Append(element6);
            client.Encoder.Append(element7);
            client.Encoder.Append(element8);
            client.Encoder.Append(element9);
            client.Encoder.Append(element10);
            client.Encoder.Append(element11);
            client.Encoder.Append(element12);
            client.Encoder.Append(element13);
            client.Encoder.Append(element14);
            client.Socket.Send(client.Encoder.Buffer, client.Encoder.Length, SocketFlags.None);
        }

        public static void Send(this OscClient client, string address, string element1, 
                                float element2, float element3, float element4, float element5, float element6, float element7, float element8, 
                                float element9, float element10, float element11, float element12, float element13, float element14, int element15)
        {            
            client.Encoder.Clear();
            client.Encoder.Append(address);
            client.Encoder.Append(",sfffffffffffffi");
            client.Encoder.Append(element1);
            client.Encoder.Append(element2);
            client.Encoder.Append(element3);
            client.Encoder.Append(element4);
            client.Encoder.Append(element5);
            client.Encoder.Append(element6);
            client.Encoder.Append(element7);
            client.Encoder.Append(element8);
            client.Encoder.Append(element9);
            client.Encoder.Append(element10);
            client.Encoder.Append(element11);
            client.Encoder.Append(element12);
            client.Encoder.Append(element13);
            client.Encoder.Append(element14);
            client.Encoder.Append(element15);
            client.Socket.Send(client.Encoder.Buffer, client.Encoder.Length, SocketFlags.None);
        }
    }
}