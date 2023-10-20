using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using Newtonsoft.Json;
using ExtensionMethods;

namespace Networking
{
    /// <summary>
    /// All the neccesary methods for make the connections and receive the data from the ESP8266
    /// </summary>
    public class ESPSocket
    {
        private Socket socket;
        private IPEndPoint iPEndPoint;
        public ESPSocket(string ip, int port)
        {
            iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            socket = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public class ESPMessage
        {
            public string Device { get; set; }
            public string Action { get; set; }
            public string Message { get; set; }
            public ESPMessage(string Device, string Action, string Message)
            {
                this.Device = Device;
                this.Action = Action;
                this.Message = Message;
            }
        }

        /// <summary>
        /// Create a connection with the ip and check if the data is sent correctly
        /// </summary>
        /// <returns>Returns true if the connection was succesfully established</returns>
        public bool StartConnection()
        {
            socket.Connect(iPEndPoint);
            byte[] message = "Prueba".EncodeMessage();
            socket.Send(message);
            byte[] receivedMessage = new byte[4];
            socket.Receive(receivedMessage);
            //We check if the message we sent is the same that arrived to the esp8266
            if(message == receivedMessage)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sends data to the socket and returns if the data arrived succesfully
        /// </summary>
        /// <param name="Data">Data to send</param>
        /// <returns>Returns  the data if was  sent correctly or null if the data wasnt send correctly</returns>
        public string SendData(string Data)
        {
            byte[] dataBytes = Data.EncodeMessage();
            socket.Send(dataBytes);
            byte[] receivedData = new byte[32];
            socket.Receive(receivedData);
            
            if(dataBytes == receivedData)
            {
                return receivedData.DecodeMessage();
            }
            return  null;
        }

        public JsonObject GetData()
        {
            byte[] data = new byte[128];
            socket.Receive(data);

            return JsonConvert.DeserializeObject(data.DecodeMessage()) as JsonObject;
        }
    }
}

namespace ExtensionMethods
{
    public static class MyExtensions
    {
        /// <summary>
        /// Converts a string to bytes for send a string with the socket
        /// </summary>
        /// <param name="message">Message to convert</param>
        /// <returns>The Message in bytes</returns>
        public static byte[] EncodeMessage(this string message)
        {
            return Encoding.UTF8.GetBytes(message);
        }

        /// <summary>
        /// Converts bytes to a string to read a data received from the socket
        /// </summary>
        /// <param name="message">Data in bytes</param>
        /// <returns>The message in String</returns>
        public static string DecodeMessage(this byte[] message)
        {
            return Encoding.UTF8.GetString(message);
        }
    }
}
