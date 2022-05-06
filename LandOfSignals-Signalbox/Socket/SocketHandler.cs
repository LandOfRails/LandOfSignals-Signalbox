using System.Diagnostics;
using System.Net;
using System.Net.Mime;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using Microsoft.AspNetCore.Mvc.Formatters;
using Timer = System.Timers.Timer;

namespace LandOfSignals_Signalbox.Socket
{
    public class SocketHandler
    {
        private static System.Net.Sockets.Socket _clientSocket;
        private readonly int _pid;

        public SocketHandler()
        {

            var serverAddress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4343);

            _clientSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clientSocket.Connect(serverAddress);
            _pid = Convert.ToInt32(Receive());

            new Thread(() =>
            {
                while (ProcessExists(_pid)) Thread.Sleep(5000);
                Environment.Exit(0);
            });
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

        public static string SendReceive(string message)
        {
            Send(message);
            return Receive();
        }

        private bool ProcessExists(int id)
        {
            return Process.GetProcesses().Any(x => x.Id == id);
        }
    }
}
