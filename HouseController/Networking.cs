using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace Networking
{
    /// <summary>
    /// All the neccesary methods for make the connections and receive the data from the ESP8266
    /// </summary>
    public static class Networking
    {
        /// <summary>
        /// Creates a new socket with the neccesary params for connect to the ESP8266
        /// </summary>
        /// <param name="iPEndPoint"></param>
        /// <returns>Returns a new socket ready to connect with the ESP8266</returns>
        public static Socket CreateSocket(IPEndPoint iPEndPoint)
        {
            return new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }
        /// <summary>
        /// Creates a new IpEndPoint with the params
        /// </summary>
        /// <param name="Ip">Ip for the IpEndPoint</param>
        /// <param name="Port">Port for the IpEndPoint</param>
        /// <returns>returns a new IpEndPoint</returns>
        public static IPEndPoint CreateEndPoint(string Ip, int Port)
        {
            return new IPEndPoint(IPAddress.Parse(Ip), Port);
        }
        /// <summary>
        /// Returns true if the connection was succesfully established
        /// </summary>
        /// <param name="iPEndPoint">EndPoint containing the ip and the port</param>
        /// <param name="socket">Socket to make the connection with</param>
        public static void StartConnection(IPEndPoint iPEndPoint, Socket socket)
        {
            socket.Connect(iPEndPoint);
            var testa = System.Text.Encoding.UTF8.GetBytes("Prueba");
            socket.Send(testa);
            socket.Disconnect(true);
        }
    }
}
