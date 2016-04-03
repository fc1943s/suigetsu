using System;
using System.Text;
using System.Threading;
using NLog;
using Suigetsu.Core.Logging;

namespace Suigetsu.Core.Desktop.Net
{
    public class RemoteInvokeServer
    {
        private const string RemoteInvokeCmd = "RemoteInvoke";
        private static readonly Logger Logger = LoggingManager.GetCurrentClassLogger();
        private readonly Action _action;
        private readonly int _port;
        private ServerSocket _server;

        public RemoteInvokeServer(int port, Action action)
        {
            _port = port;
            _action = action;
        }

        public void Start()
        {
            _server = _server ?? new ServerSocket();

            _server.Listen
                (new SocketListenerParameters
                {
                    Port = _port,
                    DataSizeHeader = false,
                    OnReceiveData = (data, sendBack) =>
                    {
                        var dataText = Encoding.UTF8.GetString(data);
                        if(dataText == RemoteInvokeCmd)
                        {
                            Logger.Trace("Remote Invoke received.");
                            try
                            {
                                new Thread(() => _action()).Start();
                            }
                            catch(Exception e)
                            {
                                Logger.Error(e);
                            }
                            Logger.Trace("End remote invoke.");
                        }
                        else
                        {
                            Logger.Trace("Invalid Remote Invoke command received: {0}.", dataText);
                        }
                    }
                });
        }

        public void Stop()
        {
            _server?.Dispose();
        }
    }
}
