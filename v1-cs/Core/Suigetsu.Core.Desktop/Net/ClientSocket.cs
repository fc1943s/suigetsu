using System.Threading.Tasks;

namespace Suigetsu.Core.Desktop.Net
{
    public class ClientSocket : BaseSocket
    {
        public ClientSocket() : base(SocketType.Client) {}

        public new bool Connect(string host, int port) => base.Connect(host, port);

        public new async Task<byte[]> Send(byte[] data, bool dataSizeHeader, bool expectResponse)
            => await base.Send(data, dataSizeHeader, expectResponse);
    }
}
