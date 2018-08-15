using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenGloveApp.Models;
using OpenGloveApp.OpenGloveAPI;
using OpenGloveApp.Server;
using Xamarin.Forms;

namespace OpenGloveApp.Pages
{
    public partial class DevicesPage : ContentPage
    {
        public DevicesPage()
        {
            InitializeComponent();

            listViewBoundedDevices.IsPullToRefreshEnabled = true;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            LoadDeviceListAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        public async Task<bool> LoadDeviceListAsync()
        {
            bool loaded = false;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    listViewBoundedDevices.ItemsSource = new List<BluetoothDevices>() { new BluetoothDevices() { Name = "ForTest-iOS-Emulator1", Address = "aa:d9:61:de:39:b2" } }; //For show in emulator and devices
                    await DisplayAlert("No Implemented Exception", "OpenGloveApp.Pages.DevicesPage.cs: OpenGlove dont support iOS device bluetooth management, need implement Communication.cs", "Ok");
                    loaded = true;
                    break;

                case Device.Android:
                    var devices = Home.OpenGlove.GetAllPairedDevices();
                    if (devices != null)
                        if(devices.Count > 0)
                            listViewBoundedDevices.ItemsSource = devices;
                    else
                        listViewBoundedDevices.ItemsSource = new List<BluetoothDevices>() { new BluetoothDevices() { Name = "ForTest-Android-Emulator1", Address = "aa:d9:61:de:39:b2" } }; //For show in emulator
                    break;
                default:
                    loaded = false;
                    break;
            }
            return loaded;
        }

        void Handle_Activated(object sender, System.EventArgs e)
        {
            DisplayAlert("Settings Activated", "Your pressed: " + ((ToolbarItem)sender).Text, "OK");
        }

        async void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            var device = e.Item as BluetoothDevices;
            bool connect = await DisplayAlert("Try Connecting", $" Device: {device.Name} \n MAC Address: {device.Address}", "Connect", "Cancel");

            //Blocking call
            if (connect)
            {
                //TODO call the config asociate to this OpenGlove device if exist in this Page
                OpenGloveConfiguration openGloveConfiguration = new OpenGloveConfiguration(); //default config
                OpenGlove openGlove = new OpenGlove(device.Name, openGloveConfiguration);

                if(!OpenGloveServer.OpenGloveByDeviceName.ContainsKey(device.Name))
                    OpenGloveServer.OpenGloveByDeviceName.Add(device.Name, openGlove);
                
                OpenGloveServer.OpenGloveByDeviceName[device.Name].OpenDeviceConnection(device); //Blocking call
            }
        }

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            listViewBoundedDevices.SelectedItem = null;
        }

        async void Handle_RefreshingAsync(object sender, System.EventArgs e)
        {
            await LoadDeviceListAsync();
            listViewBoundedDevices.IsRefreshing = false;
        }
    }
}
