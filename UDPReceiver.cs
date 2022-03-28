using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AlbertSavesThePlanets2UDPReceiverAndSignalRSender
{
    internal class UDPReceiver
    {
        public UdpClient socket;
        internal void Create()
        {
            if (socket == null)
                socket = new UdpClient(8080);
        }

        internal string DecipherMessage(IAsyncResult result)
        {
            socket = result.AsyncState as UdpClient; // UdpClient
            IPEndPoint source = new IPEndPoint(0, 0);
            byte[] message = socket.EndReceive(result, ref source);
            return Encoding.ASCII.GetString(message);
        }
    }
}
