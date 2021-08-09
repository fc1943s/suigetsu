namespace Suigetsu.Core.Desktop.Net
{
    public class ServerSocket : BaseSocket
    {
        public ServerSocket() : base(SocketType.Server) {}

        public new void Listen(SocketListenerParameters socketListenerParameters) => base.Listen(socketListenerParameters);
    }
}
