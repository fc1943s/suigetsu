using System.Net.Sockets;

namespace Suigetsu.Core.Desktop.Net
{
    public class SocketWrapper : Socket, ISocket
    {
        public SocketWrapper() : base(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) {}
    }
}
