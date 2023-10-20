using HouseController;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Networking;

namespace CusComponent;

public partial class Card : ContentView
{
    private int status;
    private readonly Color Red, Green;
    public static readonly BindableProperty DeviceNameProperty = BindableProperty.Create(nameof(DeviceName), typeof(string), typeof(Card), string.Empty);
    public string DeviceName {  get; set; }
    public Card()
	{
        InitializeComponent();
        if(Application.Current.Resources.TryGetValue("Green", out var green))
        {
            Green = (Color)green;
        }
        if (Application.Current.Resources.TryGetValue("Red", out var red))
        {
            Red = (Color)red;
        }
    }
    private void OnStatusButtonClicked(object sender, EventArgs e)
    {
        
    }

    private int curtimerow = 0;
    private int curtimecolumn = 0;
    private void OnTimeAdded(object sender, EventArgs e)
    {
        //We set the position in the grid of the times that we add
        if(curtimerow == 3)
        {
            curtimerow = 0;
            curtimecolumn += 1;
        }
        else
        {
            curtimerow += 1;
        }
    }

    private void AddTime(object sender, EventArgs e)
    {
        Label test = new Label();
        test.Text = "asd";
        BoxView status = new BoxView();
        status.WidthRequest = 16;
        status.HeightRequest = 16;
        status.VerticalOptions = LayoutOptions.Center;
        HorizontalStackLayout layout = new HorizontalStackLayout();
        layout.SetValue(Grid.RowProperty, curtimerow);
        layout.SetValue(Grid.ColumnProperty, curtimecolumn);

        //Add the status color and the time for the times that are configured to turn on and off
        layout.Children.Add(status);
        layout.Children.Add(test);
    }
}