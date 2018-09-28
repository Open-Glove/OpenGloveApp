using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace OpenGloveApp.Pages
{
    public partial class FlexorsPage : ContentPage
    {
        public FlexorsPage()
        {
            InitializeComponent();
            ConfigureToolbarItemsByPlatform();
        }

        public void ConfigureToolbarItemsByPlatform()
        {
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    ToolbarItemSave.Order = ToolbarItemOrder.Primary;
                    ToolbarItemSave.Priority = 0;
                    ToolbarItemTest.Order = ToolbarItemOrder.Secondary;
                    ToolbarItemTest.Priority = 1;
                    ToolbarItemCalibrate.Order = ToolbarItemOrder.Secondary;
                    ToolbarItemCalibrate.Priority = 2;
                    break;
                case Device.iOS:
                    ToolbarItemSave.Order = ToolbarItemOrder.Primary;
                    ToolbarItemSave.Priority = 0;
                    ToolbarItemTest.Order = ToolbarItemOrder.Primary;
                    ToolbarItemTest.Priority = 1;
                    ToolbarItemCalibrate.Order = ToolbarItemOrder.Primary;
                    ToolbarItemCalibrate.Priority = 2;
                    break;
                default:
                    break;
            }
        }

        void Handle_Activated(object sender, System.EventArgs e)
        {
            DisplayAlert("Settings Activated", "Your pressed: " + ((ToolbarItem)sender).Text, "OK");
        }
    }
}
