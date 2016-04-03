using System;
using System.Net;
using System.Net.Sockets;

namespace Suigetsu.Core.Desktop.Net
{
    public interface ISocket
    {
        int SendTimeout { set; }
        int ReceiveTimeout { set; }
        bool Connected { get; }
        bool IsBound { get; }
        IAsyncResult BeginAccept(AsyncCallback callback, object state);
        void Bind(EndPoint localEp);
        void Listen(int i);
        void Shutdown(SocketShutdown both);
        void Close();
        IAsyncResult BeginConnect(string host, int port, AsyncCallback requestCallback, object state);
        Socket EndAccept(IAsyncResult asyncResult);
        int EndSend(IAsyncResult asyncResult);

        IAsyncResult BeginSend
            (byte[] buffer, int offset, int size, SocketFlags socketFlags, AsyncCallback callback, object state);
    }
}
