using System;
using System.Diagnostics;
using OpenGloveApp.Server;
using Xamarin.Forms;

namespace OpenGloveApp.Pages
{
    public partial class ServerPage : ContentPage
    {
        public static OpenGloveServer OpenGloveServer;

        public ServerPage()
        {
            InitializeComponent();
        }

        void Handle_Activated(object sender, System.EventArgs e)
        {
            DisplayAlert("Settings Activated", "Your pressed: " + ((ToolbarItem)sender).Text, "OK");
        }

        void Switcher_Toogled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                TryStartServer();
            }
            else
            {
                TryStopServer();
            }    
        }

        public void TryStartServer()
        {
            bool instancedServer = false;
            try
            {
                OpenGloveServer = new OpenGloveServer(entryEndPointServer.Text);
                instancedServer = true;
            }
            catch (FormatException formatException)
            {
                instancedServer = false;
                Debug.WriteLine("FormatException: " + formatException.Message);
                DisplayAlert("Error", "Example server End Point: \n ws://127.0.0.1:7070", "OK");
            }

            if (instancedServer)
            {
                OpenGloveServer.Start();
                labelServerStatus.TextColor = Color.FromHex(AppConstants.Colors.ColorPrimary);
                labelServerStatus.Text = "ON";
            }
            else
            {
                switchServer.IsToggled = false; // failed to run server
            }
        }

        public void TryStopServer()
        {

            bool instancedServer = true;
            try
            {
                OpenGloveServer.Stop();
                instancedServer = false;
            }
            catch (ObjectDisposedException dE)
            {
                instancedServer = true;
                Debug.WriteLine("ObjectDisposedException: " + dE.Message);
                DisplayAlert("Error", "Failed to Dispose the Server", "OK");
            }

            if (instancedServer)
            {
                labelServerStatus.TextColor = Color.FromHex(AppConstants.Colors.ColorPrimary);
                labelServerStatus.Text = "ON";
                switchServer.IsToggled = true; // failed to dispose server
            }
            else
            {
                labelServerStatus.TextColor = Color.FromHex(AppConstants.Colors.ColorAccent);
                labelServerStatus.Text = "OFF";
            }
        }
    }
}
