using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using Newtonsoft.Json;
using ExtensionMethods;
using System.Diagnostics;

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
        /// Create a connection with the server
        /// </summary>
        public Task StartConnectionAsync()
        {
            return socket.ConnectAsync(iPEndPoint);
        }

        public void CloseConnection()
        {
            socket.Close();
        }

        /// <summary>
        /// Sends data to the socket and returns if the data arrived succesfully
        /// </summary>
        /// <param name="Data">Data to send</param>
        /// <returns>Returns  the data if was  sent correctly or null if the data wasnt send correctly</returns>
        public async Task<string> SendDataAsync(string Data, int byteSize, bool waitForAnswer = true)
        {
            byte[] dataBytes = Data.EncodeMessage();
            await socket.SendAsync(dataBytes);
            if (waitForAnswer)
            {
                Task.Delay(300).Wait();
                var receivedData = new byte[byteSize];
                var buffer = await socket.ReceiveAsync(receivedData);
                return receivedData.DecodeMessage(buffer);
            }
            return null;
        }

        /// <summary>
        /// Get the information of all devices connected on the ESP
        /// </summary>
        /// <returns>ESPInitial array object containing the data of the devices from the JSON received from the ESP</returns>
        public async Task<List<ESPInitialData>> GetDevicesData()
        {
            var jsonString = await SendDataAsync("InDt", 2048);
            var jsonObject = JsonConvert.DeserializeObject<List<ESPInitialData>>(jsonString);
            return jsonObject;
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
        public static string DecodeMessage(this byte[] message, int buffer)
        {
            //Get the string from the first element until the length of the data less 1 cause of the intial element 0
            //We delete possible whitespaces with Trim method
            return Encoding.UTF8.GetString(message, 0, buffer).Trim();
        }
    }
}
