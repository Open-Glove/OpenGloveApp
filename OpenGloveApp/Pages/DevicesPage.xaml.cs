using System;
using System.Collections.Generic;
using OpenGloveApp.Models;
using Xamarin.Forms;

namespace OpenGloveApp.Pages
{
    public partial class DevicesPage : ContentPage
    {
        public DevicesPage()
        {
            InitializeComponent();

            switch(Device.RuntimePlatform)
            {
                case Device.iOS:
                    DisplayAlert("No Implemented Exception","OpenGlove dont support iOS device bluetooth management, need implement Communication.cs","Ok");
                    break;
                case Device.Android:
                    var devices = Home.OpenGlove.GetAllPairedDevices();
                    if (devices != null)
                        listViewBoundedDevices.ItemsSource = devices;
                    else
                        listViewBoundedDevices.ItemsSource = new List<BluetoothDevices>() { new BluetoothDevices(){Name = "ForTest-Android-Emulator1", Address = "aa:d9:61:de:39:b2"}}; //For show in emulator
                    break;
                default:
                    break;
            }



        }

        async void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            var device = e.Item as BluetoothDevices;
            bool connect = await DisplayAlert("Try Connecting", $" Device: {device.Name} \n MAC Address: {device.Address}", "Connect", "Cancel");

            //Blocking call
            if (connect)
            {
                Home.OpenGlove.OpenDeviceConnection(device); //Blocking call
            }
        }

        void Handle_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            listViewBoundedDevices.SelectedItem = null;
        }
    }
}
