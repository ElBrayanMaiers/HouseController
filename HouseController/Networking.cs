using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using Newtonsoft.Json;

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
        /// Converts a string to bytes for send a string with the socket
        /// </summary>
        /// <param name="message">Message to convert</param>
        /// <returns>The Message in bytes</returns>
        private static byte[] CreateMessage(string message)
        {
            return Encoding.UTF8.GetBytes(message);
        }

        private static string DecodeMessage(byte[] message)
        {
            return Encoding.UTF8.GetString(message);
        }
        /// <summary>
        /// Returns true if the connection was succesfully established
        /// </summary>
        /// <param name="iPEndPoint">EndPoint containing the ip and the port</param>
        /// <param name="socket">Socket to make the connection with</param>
        public static bool StartConnection(IPEndPoint iPEndPoint, Socket socket)
        {
            socket.Connect(iPEndPoint);
            byte[] testMessage = CreateMessage("Prueba");
            socket.Send(testMessage);
            byte[] receivedMessage = new byte[4];
            socket.Receive(receivedMessage);
            //We check if the message we sent is the same that arrived to the esp8266
            if(testMessage == receivedMessage)
            {
                return true;
            }
            return false;
        }

        public static JsonObject GetData(Socket socket)
        {
            byte[] data = new byte[128];
            socket.Receive(data);

            return JsonConvert.DeserializeObject(DecodeMessage(data)) as JsonObject;
        }
    }
}
