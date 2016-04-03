using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NUnit.Framework;
using Suigetsu.Core.Cryptography;
using Suigetsu.Core.Desktop.Net;
using Suigetsu.Core.Extensions;
using Suigetsu.Core.IO;
using Suigetsu.Core.Logging;

namespace Suigetsu.Core.Desktop.Tests.Net
{
    [TestFixture]
    public class BaseSocketTests : AssertionHelper
    {
        private static readonly Logger Logger = LoggingManager.GetCurrentClassLogger();

        [Test, Ignore("Slow"), ExcludeFromCodeCoverage]
        public async Task BigDataTest()
        {
            var port = new Random().Next(30000, 40000);

            using(var serverSocket = new ServerSocket())
            using(var clientSocket = new ClientSocket())
            {
                //serverSocket.LogPreview = true;
                //clientSocket.LogPreview = true;

                var bigData = Enumerable.Repeat<byte>(65, 500000).ToArray();

                foreach(var v in new[] { true, false })
                {
                    serverSocket.Listen
                        (new SocketListenerParameters
                        {
                            Port = port,
                            DataSizeHeader = v,
                            OnReceiveData = (data, sendBack) => sendBack(data) 
                        });

                    clientSocket.Connect("localhost", port);

                    var result = await clientSocket.Send(bigData, v, true);
                    Expect(result, EqualTo(bigData));
                }
            }
        }

        [Test, Ignore("Slow"), ExcludeFromCodeCoverage, SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        public async Task FileTest()
        {
            using(var tmp1 = new TempFile())
            using(var tmp2 = new TempFile())
            {
                tmp1.Handle.Seek(10L * 1024 * 1024, SeekOrigin.Begin);
                tmp1.Handle.WriteByte(1);
                tmp1.CloseHandle();

                using(var serverSocket = new ServerSocket())
                using(var clientSocket = new ClientSocket())
                {
                    //serverSocket.LogPreview = true;
                    //clientSocket.LogPreview = true;

                    serverSocket.Listen
                        (new SocketListenerParameters
                        {
                            Port = 10000,
                            DataSizeHeader = false,
                            OnReceivePartialData = (data, clearData) =>
                            {
                                using(var stream = new FileStream(tmp2.FilePath, FileMode.Append))
                                {
                                    stream.Write(data, 0, data.Length);
                                    clearData();
                                }
                            }
                        });

                    var buffer = File.ReadAllBytes(tmp1.FilePath);

                    clientSocket.Connect("localhost", 10000);

                    await clientSocket.Send(buffer, false, false);

                    while(new FileInfo(tmp2.FilePath).Length != new FileInfo(tmp1.FilePath).Length) {}
                }

                Expect(Md5.FromFile(tmp1.FilePath), EqualTo(Md5.FromFile(tmp2.FilePath)));
            }
        }

        [Test, SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        public async Task GeneralTest()
        {
            var port = new Random().Next(30000, 40000);

            using(var serverSocket = new ServerSocket())
            using(var clientSocket1 = new ClientSocket())
            using(var clientSocket2 = new ClientSocket())
            using(var clientSocket3 = new ClientSocket())
                /*{
            CreateSocket = () =>
            {
                var clientMock = new Mock<SocketWrapper>
                {
                    CallBase = true
                };
                var intfClientMock = clientMock.As<ISocket>();
                intfClientMock.Setup(x => x.Receive(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), 0)).Throws(new SocketException(10060));
                return intfClientMock.Object;
            }
        })*/
            {
                serverSocket.LogPreview = true;
                clientSocket1.LogPreview = true;
                clientSocket2.LogPreview = true;
                clientSocket3.LogPreview = true;

                Expect(() => clientSocket1.Delimiter = new byte[0], Throws.InvalidOperationException);

                Expect(async () => await clientSocket1.Send(new byte[0], false, false), Throws.InstanceOf<Exception>());

                Expect(() => clientSocket1.Connect(string.Empty, port + 1), Throws.ArgumentException);

                Expect(() => clientSocket1.Connect("localhost", 0), Throws.ArgumentException);
                Expect(clientSocket1.Connect("localhost", port + 1), False);

                var i = 0;
                foreach(var v in new[] { false, true, false, true, false })
                {
                    i++;
                    Logger.Trace("CURRENT INDEX: {0}", i);

                    clientSocket1.Timeout++;

                    if(i == 3)
                    {
                        serverSocket.Delimiter = Encoding.ASCII.GetBytes("!*!");
                        clientSocket1.Delimiter = Encoding.ASCII.GetBytes("!*!");
                        clientSocket2.Delimiter = Encoding.ASCII.GetBytes("!*!");
                    }

                    if(i == 5)
                    {
                        serverSocket.ForceEmptyDelimiter();
                        clientSocket1.ForceEmptyDelimiter();
                        clientSocket2.ForceEmptyDelimiter();
                    }

                    serverSocket.Listen
                        (new SocketListenerParameters
                        {
                            Port = port,
                            DataSizeHeader = v,
                            OnReceiveData = (data, sendBack) =>
                            {
                                if(data.Length < 100)
                                {
                                    if(Encoding.Default.GetString(data).In("S1_NORESPONSETEXT", "S2_NORESPONSETEXT"))
                                    {
                                        sendBack(null);
                                        return;
                                    }

                                    if(Encoding.Default.GetString(data) == "RETURNEMPTY")
                                    {
                                        sendBack(new byte[0]);
                                        return;
                                    }
                                }

                                sendBack(data);
                            },
                            OnReceivePartialData = (data, clearData) =>
                            {
                                if(Encoding.Default.GetString(data) == "CLEARDATA")
                                {
                                    clearData();
                                }
                            }
                        });

                    Expect(clientSocket1.Connect("localhost", port));

                    Expect(clientSocket2.Connect("localhost", port));

                    await clientSocket1.Send(Encoding.Default.GetBytes("S1_NORESPONSETEXT"), v, false);
                    await clientSocket2.Send(Encoding.Default.GetBytes("S2_NORESPONSETEXT"), v, false);

                    var result = await clientSocket1.Send(Encoding.Default.GetBytes("S1_DATATEXT"), v, true);
                    Expect(Encoding.Default.GetString(result), EqualTo("S1_DATATEXT"));

                    if(!v && i != 5)
                    {
                        var currentDelimiter = clientSocket1.Delimiter;
                        clientSocket1.ForceEmptyDelimiter();

                        await clientSocket1.Send(Encoding.Default.GetBytes("HA"), false, false);

                        //Expect(Encoding.Default.GetString(result), EqualTo(Encoding.Default.GetString(currentDelimiter)));

                        clientSocket1.Delimiter = currentDelimiter;

                        result = await clientSocket1.Send(Encoding.Default.GetBytes("LF"), false, true);
                        Expect(Encoding.Default.GetString(result), EqualTo("HALF"));
                    }

                    if(i == 4)
                    {
                        clientSocket3.Connect("localhost", port);

                        //TIMEOUT
                        //result = await clientSocket3.Send(Encoding.Default.GetBytes("TIMEOUT"), v, true);
                        //Expect(Encoding.Default.GetString(result), Empty);
                    }

                    result = await clientSocket1.Send(Encoding.Default.GetBytes("S1_TEXTAFTERHALF!!!"), v, true);
                    Expect(Encoding.Default.GetString(result), EqualTo("S1_TEXTAFTERHALF!!!"));

                    result = await clientSocket1.Send(Encoding.Default.GetBytes("CLEARDATA"), v, i != 5);
                    if(i != 5)
                    {
                        Expect(Encoding.Default.GetString(result), EqualTo(""));
                    }

                    result = await clientSocket2.Send(Encoding.Default.GetBytes("RETURNEMPTY"), v, i != 5);
                    Expect(Encoding.Default.GetString(result), Empty);

                    result = await clientSocket2.Send(Encoding.Default.GetBytes("S2_DATATEXT"), v, true);
                    Expect(Encoding.Default.GetString(result), EqualTo("S2_DATATEXT"));
                }
            }
        }
    }
}
