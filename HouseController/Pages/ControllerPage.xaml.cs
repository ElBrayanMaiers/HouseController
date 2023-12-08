using System.Diagnostics;

namespace HouseController.Pages;

using Networking;

[QueryProperty(nameof(Ip), "Ip")]
public partial class ControllerPage : ContentPage
{
    int count = 0;
    private int status;
    private readonly Color Red, Green;

    private ESPSocket _socket;
    private readonly int Port = 2500;

    public string Ip
    {
        set => _socket = new ESPSocket(value, Port);
    }
    public ControllerPage()
    {
        InitializeComponent();
        Task.Run(InitializeEsp);
    }
    private async void InitializeEsp()
    {
        await _socket.StartConnectionAsync();
        List<ESPSocket.ESPInitialData> DevicesStatus = await _socket.GetDevicesData();
        //We set the interface for every device configurated in the ESP
        foreach (ESPSocket.ESPInitialData Device in DevicesStatus)
        {
            Debug.WriteLine(Device.name);
        }
    }
    private void OnCounterClicked(object sender, EventArgs e)
    {
        count++;

    }
}