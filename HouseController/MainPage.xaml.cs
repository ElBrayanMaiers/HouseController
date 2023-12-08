namespace HouseController
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            MainThread.BeginInvokeOnMainThread(() => { Navigation.PushAsync(new Pages.ConnectPage());});
        }
    }
}