using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace OpenGloveApp.Pages
{
    public partial class BoardsPage : ContentPage
    {
        public BoardsPage()
        {
            InitializeComponent();
        }

        void Handle_Activated(object sender, System.EventArgs e)
        {
            DisplayAlert("Settings Activated", "Your pressed: " + ((ToolbarItem)sender).Text, "OK");
        }
    }
}
