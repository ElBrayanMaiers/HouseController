namespace HouseController
{
    using Networking;
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private int status;
        private readonly Color Red, Green;

        private readonly ESPSocket socket;
        private readonly string Ip = "192.168.0.105";
        private readonly int Port = 85;
        public MainPage()
        {
            socket = new ESPSocket(Ip, Port);
            InitializeComponent();
            InitializeESP();
        }
        private async void InitializeESP()
        {
            List<ESPSocket.ESPInitialData> DevicesStatus = await socket.StartConnection();
            //We set the interface for every device configurated in the ESP
            foreach (ESPSocket.ESPInitialData Device in DevicesStatus)
            {
                
            }
        }
        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

        }
    }
}