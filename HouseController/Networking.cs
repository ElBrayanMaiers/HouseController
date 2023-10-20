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
            //We create an endpoint with the params ip and port and we create the socket in TCP mode
            iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            socket = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        //Class for the JSON object of the data that is received to get the actual status of the ESP
        public class ESPInitialData
        {
            public string name { get; set; }
            public int status { get; set; }
            public string[] times { get; set; }
            public string[] timesStatus { get; set; }
        }

        /// <summary>
        /// Create a connection with the ip
        /// </summary>
        /// <returns>Returns a status of the devices</returns>
        public async Task<List<ESPSocket.ESPInitialData>> StartConnection()
        {
            socket.Connect(iPEndPoint);
            //Delay to be sure that all the ESP status data is received
            await Task.Delay(500);
            return GetStatusJSON();
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
            byte[] receivedData = new byte[500];
            socket.Receive(receivedData);
            
            if(dataBytes == receivedData)
            {
                return receivedData.DecodeMessage();
            }
            return  null;
        }
        /// <summary>
        /// Get the actual status of the devices
        /// </summary>
        /// <returns>ESPInitial array object containing the status data of devices from the JSON received from the ESP</returns>
        public List<ESPInitialData> GetStatusJSON()
        {
            byte[] data = new byte[500];
            socket.Receive(data);
            return JsonConvert.DeserializeObject<List<ESPInitialData>>(data.DecodeMessage());
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
