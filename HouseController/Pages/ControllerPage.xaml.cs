using System.Diagnostics;

namespace HouseController.Pages;

using Networking;

[QueryProperty(nameof(Ip), "Ip")]
public partial class ControllerPage : ContentPage
{
    private ESPSocket _socket;
    private readonly int Port = 2500;

    public string Ip
    {
        set => _socket = new ESPSocket(value, Port);
    }
    public ControllerPage()
    {
        InitializeComponent();
        asdf.DeviceTimeStatus = new List<string>(){ "1", "asdasd","ggggg"};
        asdf.DeviceTimes = new List<string>() { "51", "ggggggg", "maraca" };
        Task.Run(InitializeEsp);
    }

    private async void InitializeEsp()
    {
        await _socket.StartConnectionAsync();
        List<ESPSocket.EspInitialData> DevicesStatus = await _socket.GetDevicesData();
        //We set the interface for every device configurated in the ESP
        foreach (ESPSocket.EspInitialData Device in DevicesStatus)
        {
            Debug.WriteLine(Device.Name);
        }
    }
}