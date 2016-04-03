using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Suigetsu.Core.Configuration;
using Suigetsu.Core.Extensions;
using Suigetsu.Core.Logging;

namespace Suigetsu.Core.Desktop.Net
{
    public class SocketListenerParameters
    {
        public delegate void OnReceiveDataHandler(byte[] data, Action<byte[]> sendBack);

        public delegate void OnReceivePartialDataHandler(byte[] data, Action clearData);

        public bool DataSizeHeader;
        public OnReceiveDataHandler OnReceiveData;
        public OnReceivePartialDataHandler OnReceivePartialData;
        public int Port;
    }

    public abstract class BaseSocket : IDisposable
    {
        private const int DefaultBufferSize = 65535;
        public static readonly byte[] DefaultSocketDelimiter = Encoding.ASCII.GetBytes("^");
        private static readonly Logger Logger = LoggingManager.GetCurrentClassLogger();
        private readonly SocketType _socketType;
        private byte[] _delimiter;
        private string _host;
        private int _port;
        private ISocket _socket;
        private int _timeout;

        protected BaseSocket(SocketType socketType)
        {
            _socketType = socketType;
            LogPreview = false;

            Delimiter = DefaultSocketDelimiter;
            Timeout = Settings.Get<Settings>().SocketTimeout;

            CreateSocket = CreateDefaultSocket;
        }

        public bool LogPreview { private get; set; }

        public byte[] Delimiter
        {
            get
            {
                return _delimiter;
            }
            set
            {
                if(value.IsEmpty())
                {
                    throw new InvalidOperationException("Socket delimiter can't be empty.");
                }

                _delimiter = value;
            }
        }

        public int Timeout
        {
            get
            {
                return _timeout;
            }
            set
            {
                _timeout = value;

                if(_socket == null)
                {
                    return;
                }

                _socket.SendTimeout = value;
                _socket.ReceiveTimeout = value;
            }
        }

        private Func<ISocket> CreateSocket { get; }

        public void Dispose()
        {
            Dispose(true);
        }

        private ISocket CreateDefaultSocket()
        {
            return new SocketWrapper
            {
                SendTimeout = Timeout,
                ReceiveTimeout = Timeout
            };
        }

        public void ForceEmptyDelimiter()
        {
            _delimiter = new byte[0];
        }

        protected bool Connect(string host, int port)
        {
            if(host == string.Empty)
            {
                throw new ArgumentException("Invalid host.");
            }

            if(port <= 0)
            {
                throw new ArgumentException("Invalid port.");
            }

            _host = host;
            _port = port;

            if(_socket == null || _socket.Connected)
            {
                CloseSocket();
                _socket = CreateSocket();
            }

            if(_socket != null)
            {
                Logger.Trace
                    ("({0}:{1}) Connecting on {2}:{3}. Timeout: {4}.", _socket.GetHashCode(), _socketType, _host, _port, Timeout);

                var result = _socket.BeginConnect(host, port, null, null);

                if(!_socket.Connected)
                {
                    result.AsyncWaitHandle.WaitOne(Timeout + 100, true);
                }

                if(_socket.Connected)
                {
                    Thread.Sleep(100);
                    return true;
                }

                Logger.Trace("({0}:{1}) Failed to connect.", _socket.GetHashCode(), _socketType);

                CloseSocket();
                _socket = null;
            }

            return false;
        }

        protected async Task<byte[]> Send(byte[] data, bool dataSizeHeader, bool expectResponse)
        {
            if(_socket == null || !_socket.Connected)
            {
                throw new Exception("Socket not connected.");
            }

            if(!dataSizeHeader)
            {
                data = data.Concat(Delimiter).ToArray();
            }

            var total = 0;
            var size = data.Length;
            var dataLeft = size;

            if(dataSizeHeader)
            {
                var dataSize = BitConverter.GetBytes(size);
                Logger.Trace("({0}:{1}) Sending dataSize: {2}.", _socket.GetHashCode(), _socketType, size);
                await Task.Factory.FromAsync(_socket.BeginSend(dataSize, 0, dataSize.Length, 0, null, null), _socket.EndSend);
            }

            while(total < size)
            {
                var preview = new byte[0];
                if(LogPreview)
                {
                    preview = new byte[data.Length > 100 ? 100 : data.Length];
                    Buffer.BlockCopy(data, total, preview, 0, preview.Length);
                }

                Logger.Trace
                    ("({0}:{1}) Socket synchronous send: {2}({4}). dataSizeHeader: {3}.",
                     _socket.GetHashCode(),
                     _socketType,
                     Encoding.Default.GetString(preview),
                     dataSizeHeader,
                     preview.Length);

                var sent =
                    await
                    Task.Factory.FromAsync
                        (_socket.BeginSend
                             (data,
                              total,
                              /*dataLeft > DefaultBufferSize ? DefaultBufferSize :*/
                              dataLeft,
                              SocketFlags.None,
                              null,
                              null),
                         _socket.EndSend);

                //var sent = _socket.Send(data, total, /*dataLeft > 1024 ? 1024 : dataLeft*/dataLeft, SocketFlags.None);

                Logger.Trace("Sent: {0}.", sent);

                total += sent;
                dataLeft -= sent;
            }

            if(expectResponse)
            {
                return await AsyncReceive
                                 (_socket as Socket,
                                  new SocketListenerParameters
                                  {
                                      DataSizeHeader = dataSizeHeader
                                  });
            }

            return new byte[0];
        }

        private async Task<byte[]> AsyncReceive
            (Socket socket, SocketListenerParameters socketListenerParameters, bool recursive = false)
        {
            if(socket == null || !socket.Connected)
            {
                throw new Exception("Invalid socket object. Unable to AsyncReceive.");
            }

            var bufferSize = DefaultBufferSize;
            var currentPartialDataLength = 0;
            var partialData = new byte[0];
            var dataPending = false;

            Func<bool, Task<byte[]>> receive = null;

            receive = async waitForDataSizeHeader =>
            {
                dataPending = false;
                byte[] buffer;
                if(waitForDataSizeHeader)
                {
                    buffer = new byte[4];
                    var bytesRead = 0;

                    try
                    {
                        var beginReceive = socket.BeginReceive(buffer, 0, 4, 0, null, null);
                        if(beginReceive == null)
                        {
                            Logger.Error("BeginReceive = null.");
                        }
                        else
                        {
                            bytesRead = await Task.Factory.FromAsync(beginReceive, socket.EndReceive);
                        }
                    }
                    catch(SocketException e)
                    {
                        Logger.Error("({0}:{1}) AsyncReceive: {2}.", socket.GetHashCode(), _socketType, e.Message);
                    }
                    if(bytesRead == 4)
                    {
                        var size = BitConverter.ToInt32(buffer, 0);

                        if(size == 0)
                        {
                            return new byte[0];
                        }

                        Logger.Trace
                            ("({0}:{1}) AsyncReceive: The socket will read {2} bytes.", socket.GetHashCode(), _socketType, size);

                        bufferSize = size;

                        return await receive(false);
                    }

                    if(bytesRead == 0)
                    {
                        Logger.Trace("({0}:{1}) #1 Received zero. Closing socket.", socket.GetHashCode(), _socketType);
                        socket.Close();
                    }
                    else
                    {
                        Logger.Error
                            ("({0}:{1}) Socket received the wrong amount of dataSizeHeader bytes: {2}.",
                             socket.GetHashCode(),
                             _socketType,
                             bytesRead);
                    }
                }
                else
                {
                    buffer = new byte[DefaultBufferSize];

                    var bytesRead = 0;
                    try
                    {
                        var beginReceive = socket.BeginReceive(buffer, 0, DefaultBufferSize, 0, null, null);
                        if(beginReceive == null)
                        {
                            Logger.Error("BeginReceive = null.");
                        }
                        else
                        {
                            bytesRead = await Task.Factory.FromAsync(beginReceive, socket.EndReceive);
                        }
                    }
                    catch(SocketException e)
                    {
                        Logger.Error("({0}:{1}) AsyncReceive: {2}.", socket.GetHashCode(), _socketType, e.Message);
                    }

                    if(bytesRead > 0)
                    {
                        var preview = new byte[0];
                        if(LogPreview)
                        {
                            preview = new byte[bytesRead > 100 ? 100 : bytesRead];
                            Buffer.BlockCopy(buffer, 0, preview, 0, preview.Length);
                        }

                        Logger.Trace
                            ("({0}:{1}) Socket received text: {2}. bytesRead: {3}. Total expected: {4}. Delimiter: {5}. dataSizeHeader: {6}.",
                             socket.GetHashCode(),
                             _socketType,
                             Encoding.Default.GetString(preview),
                             bytesRead,
                             bufferSize,
                             Encoding.ASCII.GetString(Delimiter),
                             socketListenerParameters.DataSizeHeader);

                        var newBlock = new byte[bytesRead];
                        Buffer.BlockCopy(buffer, 0, newBlock, 0, bytesRead);

                        if(!partialData.IsEmpty())
                        {
                            var newContent = new byte[partialData.Length + bytesRead];
                            Buffer.BlockCopy(partialData, 0, newContent, 0, partialData.Length);
                            Buffer.BlockCopy(newBlock, 0, newContent, partialData.Length, bytesRead);
                            partialData = newContent;
                            currentPartialDataLength = partialData.Length;
                        }
                        else
                        {
                            partialData = newBlock;
                            currentPartialDataLength += partialData.Length;
                        }

                        if(socketListenerParameters.DataSizeHeader && currentPartialDataLength < bufferSize)
                        {
                            Logger.Trace
                                ("({0}:{1}) The socket are still waiting {2} bytes of data. Total: {3}",
                                 socket.GetHashCode(),
                                 _socketType,
                                 bufferSize - currentPartialDataLength,
                                 bufferSize);
                            socketListenerParameters.OnReceivePartialData?.Invoke(partialData, () => partialData = new byte[0]);
                            dataPending = true;
                            return null;
                        }

                        var receivedData = new byte[0];

                        if(!socketListenerParameters.DataSizeHeader)
                        {
                            var dataComplete = Delimiter.IsEmpty();
                            var contentCopy = partialData;
                            var splittedData = partialData.Split
                                (Delimiter,
                                 i =>
                                 {
                                     if(i == contentCopy.Length - Delimiter.Length)
                                     {
                                         dataComplete = true;
                                     }
                                 });

                            for(var i = 0; i < splittedData.Count; i++)
                            {
                                if(splittedData.Count == 1 && splittedData[i].Length == partialData.Length
                                   && partialData.Length >= bufferSize)
                                {
                                    Logger.Trace
                                        ("({0}:{1}) Waiting for more data. Current size: {2}.",
                                         socket.GetHashCode(),
                                         _socketType,
                                         currentPartialDataLength);
                                    socketListenerParameters.OnReceivePartialData?.Invoke
                                        (partialData, () => partialData = new byte[0]);
                                    dataPending = true;
                                    return null;
                                }

                                if(i == splittedData.Count - 1)
                                {
                                    if(dataComplete)
                                    {
                                        partialData = new byte[0];
                                        currentPartialDataLength = 0;
                                    }
                                    else
                                    {
                                        receivedData = null;
                                        partialData = splittedData[i];
                                        break;
                                    }
                                }

                                receivedData = splittedData[i];
                            }
                        }
                        else
                        {
                            receivedData = partialData;

                            partialData = new byte[0];
                            currentPartialDataLength = 0;
                        }

                        byte[] sendData = null;

                        preview = new byte[0];
                        if(LogPreview)
                        {
                            preview = new byte[bytesRead > 100 ? 100 : bytesRead];
                            Buffer.BlockCopy(buffer, 0, preview, 0, preview.Length);
                        }
                        Logger.Trace
                            ("({0}:{1}) Post processed server data length: {2}. dataSizeHeader: {3}. preview: {4}",
                             socket.GetHashCode(),
                             _socketType,
                             receivedData?.Length ?? -1,
                             socketListenerParameters.DataSizeHeader,
                             Encoding.Default.GetString(preview));

                        if(socketListenerParameters.OnReceivePartialData != null && receivedData != null)
                        {
                            socketListenerParameters.OnReceivePartialData(receivedData, () => receivedData = new byte[0]);
                        }

                        if(socketListenerParameters.OnReceiveData != null && receivedData != null)
                        {
                            socketListenerParameters.OnReceiveData
                                (receivedData, changedSendData => { sendData = changedSendData; });
                        }

                        receivedData = receivedData ?? new byte[0];

                        if(sendData == null)
                        {
                            return receivedData;
                        }

                        if(!socketListenerParameters.DataSizeHeader)
                        {
                            sendData = sendData.Concat(Delimiter).ToArray();
                        }
                        else
                        {
                            var dataSize = BitConverter.GetBytes(sendData.Length);
                            Logger.Trace("({0}:{1}) Sending dataSize: {2}.", socket.GetHashCode(), _socketType, sendData.Length);

                            var beginSend = socket.BeginSend(dataSize, 0, 4, 0, null, null);
                            if(beginSend == null)
                            {
                                Logger.Error("BeginSend = null.");
                            }
                            else
                            {
                                await Task.Factory.FromAsync(beginSend, socket.EndSend);
                            }
                        }

                        var preview2 = new byte[0];
                        if(LogPreview)
                        {
                            preview2 = new byte[sendData.Length > 100 ? 100 : sendData.Length];
                            Buffer.BlockCopy(sendData, 0, preview2, 0, preview2.Length);
                        }
                        Logger.Trace
                            ("({0}:{1}) Sending {2} bytes: {3}. dataSizeHeader: {4}.",
                             socket.GetHashCode(),
                             _socketType,
                             sendData.Length,
                             Encoding.Default.GetString(preview2),
                             socketListenerParameters.DataSizeHeader);

                        var bytesSent =
                            await
                            Task.Factory.FromAsync(socket.BeginSend(sendData, 0, sendData.Length, 0, null, null), socket.EndSend);

                        Logger.Trace("({0}:{1}) Sent {2} bytes.", socket.GetHashCode(), _socketType, bytesSent);

                        return receivedData;
                    }

                    Logger.Trace("({0}:{1}) #2 Received zero. Closing socket.", socket.GetHashCode(), _socketType);
                    socket.Close();
                }

                return new byte[0];
            };
            var data = new byte[0];
            while(socket.Connected)
            {
                data = await receive(!dataPending && socketListenerParameters.DataSizeHeader);

                if(!recursive && !dataPending)
                {
                    break;
                }
            }

            return data;
        }

        protected void Listen(SocketListenerParameters socketListenerParameters)
        {
            if(_socket == null || _socket.IsBound)
            {
                CloseSocket();
                _socket = CreateSocket();
            }

            if(_socket == null)
            {
                return;
            }

            var ipEndPoint = new IPEndPoint(IPAddress.Any, socketListenerParameters.Port);

            _socket.Bind(ipEndPoint);
            _socket.Listen(100);

            new Thread
                (() =>
                {
                    Logger.Trace("Listening on port {0}.", socketListenerParameters.Port);

                    Action accept = null;
                    accept = async () =>
                    {
                        try
                        {
                            var clientSocket = await Task.Factory.FromAsync(_socket.BeginAccept, _socket.EndAccept, true);

                            accept();
                            Logger.Trace
                                ("({0}:{1}) Server accepted connection of {2}. Target: {3}",
                                 _socket.GetHashCode(),
                                 _socketType,
                                 clientSocket.RemoteEndPoint,
                                 clientSocket.GetHashCode());

                            await AsyncReceive(clientSocket, socketListenerParameters, true);
                            Logger.Trace("After AsyncReceive.");
                        }
                        catch(ObjectDisposedException)
                        {
                            Logger.Error("({0}:{1}) Socket disposed while listening.", _socket.GetHashCode(), _socketType);
                        }
                    };
                    accept();
                })
            {
                IsBackground = true
            }.Start();
        }

        private void CloseSocket()
        {
            if(_socket == null)
            {
                return;
            }

            Logger.Trace("({0}:{1}) Closing socket.", _socket.GetHashCode(), _socketType);
            if(_socket.Connected)
            {
                _socket.Shutdown(SocketShutdown.Both);
            }
            _socket.Close();
        }

        [SuppressMessage("ReSharper", "VirtualMemberNeverOverriden.Global")]
        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                CloseSocket();
            }
        }

        protected enum SocketType
        {
            Client,
            Server
        }
    }
}
