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
        
        public static string SendReceive(string message)
        {
            // Sending
            var toSendLen = Encoding.ASCII.GetByteCount(message);
            var toSendBytes = Encoding.ASCII.GetBytes(message);
            var toSendLenBytes = BitConverter.GetBytes(toSendLen);
            _clientSocket.Send(toSendLenBytes);
            _clientSocket.Send(toSendBytes);

            return Receive();
        }

        public static string Receive()
        {
            // Receiving
            var bytesReceived = new byte[256];
            int bytes;
            var received = string.Empty;
            do
            {
                bytes = _clientSocket.Receive(bytesReceived, bytesReceived.Length, 0);
                received += Encoding.ASCII.GetString(bytesReceived, 0, bytes);
            }
            while (bytes > 0);
            return received;
        }

        private bool ProcessExists(int id)
        {
            return Process.GetProcesses().Any(x => x.Id == id);
        }
    }
}
