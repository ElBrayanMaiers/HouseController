using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using Networking;
using System.Net.Sockets;
using ExtensionMethods;
using System.Text.RegularExpressions;
using Microsoft.Maui;
using System.Collections.Specialized;

namespace HouseController.Pages;


//TODO: Implement Refreshing CollectionView and keep refreshing the ips
//TODO: Improve UI
//TODO: Implement saving ips
public partial class ConnectPage : ContentPage
{
    readonly int _port = 2500;
    readonly List<string> _addresses = new();
    private readonly ObservableCollection<string> _espIpList = new();

    public ConnectPage()
    {
        InitializeComponent();
        string IpBase = "192.168.0.";
        //Create a list with all the possible local Ips
        for (int i = 0; i < 255; i++)
        {
            _addresses.Add($"{IpBase}{i}");
        }
        SearchedList.ItemsSource = _espIpList;
        Task.Run(ScanForEsp);
    }

    private void IpButton_Clicked(object sender, EventArgs e)
    {
        var ipButton = (Button)sender;
        MainThread.InvokeOnMainThreadAsync(()=>
        {
            Shell.Current.GoToAsync($"controllerPage?Ip={ipButton.Text}");
        });
    }

    /// <summary>
    /// Get all Ips that are Esp
    /// </summary>
    /// <returns></returns>
    private async Task ScanForEsp()
    {
        //Get all alive addresses in local connection
        var allAvailableAddresses = await GetAllAvailableAddresses();
        List<Task<string>> allEspAddresses = new();

        //Add alive addresses to a list of async checks to check if the ips are from an Esp device
        foreach (var address in allAvailableAddresses)
        {
            allEspAddresses.Add(CheckEsp(address, _port));
        }
        await Task.WhenAll(allEspAddresses);

        //Check the ones that answers to the message and add it to a list
        foreach (var address in allEspAddresses)
        {
            if (address.Result != null)
            {
                Dispatcher.Dispatch(() => { _espIpList.Add(address.Result.ToString()); });
            }
        }
    }

    /// <summary>
    /// Check if the device in a ip is an ESP HouseController Device
    /// </summary>
    /// <param name="ip">Ip to check</param>
    /// <param name="port">Port to check</param>
    /// <returns>Ip if it's an EspDevice, null if it's not</returns>
    private static async Task<string> CheckEsp(string ip, int port)
    {
        ESPSocket socket = new(ip, port);
        try
        {
            await socket.StartConnectionAsync();
            string data = await socket.SendDataAsync("isESP", 1024);
            //Check if the device is an ESP
            if (data.Equals("HouseController"))
            {
                return ip;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error al conectarse a la ip:" + ex.Message);
        }
        finally { socket.CloseConnection(); }

        return null;
    }
    /// <summary>
    /// Get all Addresses that are reachable on the local network
    /// </summary>
    /// <returns>List of addresses that are alive on the local network</returns>
    private async Task<List<string>> GetAllAvailableAddresses()
	{
        List<string> addressesList = new();
        List<Task<PingReply>> availableAddresses = new();
        foreach (var address in _addresses) 
        {
            availableAddresses.Add(CheckAddress(address));
        }
        await Task.WhenAll(availableAddresses.ToArray());
        //Get all available devices in the network
        foreach (var address  in availableAddresses)
        {
            if (address.Result.Status == IPStatus.Success)
            {
                addressesList.Add(address.Result.Address.ToString());
            }
        }
        return addressesList;
    }

    private static Task<PingReply> CheckAddress(string ip)
    {
        return new Ping().SendPingAsync(ip);
    }
}