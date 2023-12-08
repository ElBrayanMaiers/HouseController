using Microsoft.Maui.Controls;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using Networking;
using System.Net.Sockets;
using ExtensionMethods;
using System.Text.RegularExpressions;
using Microsoft.Maui;

namespace HouseController.Pages;


//TODO: Implement Refreshing CollectionView and keep refreshing the ips
//TODO: Improve UI
//TODO: Implement saving ips
public partial class ConnectPage : ContentPage
{
    readonly int _port = 2500;
    readonly List<string> _addresses = new();
    public List<string> Testa = new List<string>();
    public ConnectPage()
    {
        InitializeComponent();
        string IpBase = "192.168.0.";
        for (int i = 0; i < 255; i++)
        {
            _addresses.Add($"{IpBase}{i}");
        }

        Buta.Clicked += (e, a) =>
        {
            Task.Run(ScanForEsp);
        };
    }

    private void IpButton_Clicked(object sender, EventArgs e)
    {
        var ipButton = (Button)sender;
        MainThread.InvokeOnMainThreadAsync(()=>
        {
            Shell.Current.GoToAsync($"controllerPage?Ip={ipButton.Text}");
        });
    }

    private async Task ScanForEsp()
    {
        List<string> allAvailableAddresses = await GetAllAvailableAddresses();
        List<Task<string>> allEspAddresses = new List<Task<string>>(); 
        foreach (string address in allAvailableAddresses)
        {
            allEspAddresses.Add(CheckEsp(address, _port));
        }
        await Task.WhenAll(allEspAddresses);
        foreach(var address in allEspAddresses)
        {
            if(address.Result == null)
            {
                continue;
            }

            try
            {
                Dispatcher.Dispatch(() =>
                {
                    string a = address.Result.ToString();
                    Testa.Add(a);
                    SearchedList.ItemsSource = Testa;
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }

    /// <summary>
    /// Get all Addresses that are reachable on the local network
    /// </summary>
    /// <returns></returns>
    private async Task<List<string>> GetAllAvailableAddresses()
	{
        List<string> addressesList = new List<string>();
        List<Task<PingReply>> availableAddresses = new List<Task<PingReply>>();
        foreach (string address in _addresses) 
        {
            availableAddresses.Add(CheckAddress(address));
        }
        await Task.WhenAll(availableAddresses.ToArray());
        //Get all available devices in the network
        foreach (var testa  in availableAddresses)
        {
            //Return the ip's that answered to the Ping
            if (testa.Result.Status == IPStatus.Success)
            {
                addressesList.Add(testa.Result.Address.ToString());
            }
        }
        return addressesList;
    }

    private Task<PingReply> CheckAddress(string ip)
    {
        return new Ping().SendPingAsync(ip);
    }

    /// <summary>
    /// Check if the device in a ip is an ESP HouseController Device
    /// </summary>
    /// <param name="ip">Ip to check</param>
    /// <returns>Ip if its an EspDevice, null if its not</returns>
    private async Task<string> CheckEsp(string ip, int port)
    {
        ESPSocket socket = new ESPSocket(ip, port);
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
        catch
        {
            // ignored
        }
        finally { socket.CloseConnection();}
        return null;
    }
}