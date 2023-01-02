using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LandOfSignals_Signalbox.Socket
{
    public class SocketHandler
    {
        private static System.Net.Sockets.Socket _clientSocket;
        public static string test;

        public SocketHandler()
        {
            var serverAddress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4343);
            var thread = new Thread(async () =>
            {
                _clientSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                while (true)
                {
                    var task = _clientSocket.ConnectAsync(serverAddress);
                    if (await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(10))) == task)
                    {
                        while (_clientSocket.Connected)
                        {
                            var message = Receive();
                            test = message;
                        }
                    }
                    else Environment.Exit(0);
                }
            });
            thread.Start();
        }

        public static void Send(string message)
        {
            // Sending
            var toSendLen = Encoding.ASCII.GetByteCount(message);
            var toSendBytes = Encoding.ASCII.GetBytes(message);
            var toSendLenBytes = BitConverter.GetBytes(toSendLen);
            _clientSocket.Send(toSendLenBytes);
            _clientSocket.Send(toSendBytes);
        }

        public static string Receive()
        {
            // Receiving
            var rcvLenBytes = new byte[4];
            _clientSocket.Receive(rcvLenBytes);
            var rcvLen = BitConverter.ToInt32(rcvLenBytes, 0);
            var rcvBytes = new byte[rcvLen];
            _clientSocket.Receive(rcvBytes);
            return Encoding.ASCII.GetString(rcvBytes);
        }

        private bool ProcessExists(int id)
        {
            return Process.GetProcesses().Any(x => x.Id == id);
        }
    }
}
