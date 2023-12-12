using System.ComponentModel;
using HouseController;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Networking;
using Microsoft.Maui.Controls;

namespace CusComponents;

public partial class Card : ContentView
{
    private static readonly BindableDataDevice DeviceData = new();
    private static readonly BindableProperty _deviceNameProperty = BindableProperty.Create(nameof(DeviceName), typeof(string), typeof(Card), propertyChanged:OnDeviceNamePropertyChanged);
    private static readonly BindableProperty _deviceStatusProperty = BindableProperty.Create(nameof(DeviceStatus), typeof(string), typeof(Card), propertyChanged:OnDeviceStatusChanged);

    private static readonly BindableProperty _deviceTimesProperty = BindableProperty.Create(nameof(DeviceTimes), typeof(List<string>), typeof(Card), propertyChanged:OnDeviceTimesChanged);
    private static readonly BindableProperty _deviceTimeStatusProperty = BindableProperty.Create(nameof(DeviceTimeStatus), typeof(List<string>), typeof(Card), propertyChanged:OnDeviceTimeStatusChanged);
    public string DeviceName
    {
        get=>(string)GetValue(_deviceNameProperty);
        set=>SetValue(_deviceNameProperty,value);}
    public string DeviceStatus 
    { get=>(string)GetValue(_deviceStatusProperty); set=>SetValue(_deviceStatusProperty, value); }
    public List<string> DeviceTimes { get=>(List<string>)GetValue(_deviceTimesProperty); set=>SetValue(_deviceTimesProperty, value); }
    public List<string> DeviceTimeStatus { get => (List<string>)GetValue(_deviceTimeStatusProperty); set => SetValue(_deviceTimeStatusProperty, value); }

    private static void OnDeviceNamePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        DeviceData.Name = (string)newValue;
    }
    private static void OnDeviceStatusChanged(BindableObject bindable, object oldValue, object newValue)
    {
        DeviceData.Status = (string)newValue;
    }
    private static void OnDeviceTimesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        DeviceData.Times = (List<string>)newValue;
    }
    private static void OnDeviceTimeStatusChanged(BindableObject bindable, object oldValue, object newValue)
    {
        DeviceData.TimeStatus = (List<string>)newValue;
    }
    public Card()
	{
        InitializeComponent();
        VerticalLayout.BindingContext = DeviceData;
    }

    private class BindableDataDevice : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _name;
        private string _status;
        private List<string> _times;
        private List<string> _timeStatus;

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }
        public List<string> Times
        {
            get => _times;
            set
            {
                _times = value;
                OnPropertyChanged("Times");
            }
        }
        public List<string> TimeStatus
        {
            get => _timeStatus;
            set
            {
                _timeStatus = value;
                OnPropertyChanged("TimeStatus");
            }
        }
    }

    private void OnStatusButtonClicked(object sender, EventArgs e)
    {
        
    }
}