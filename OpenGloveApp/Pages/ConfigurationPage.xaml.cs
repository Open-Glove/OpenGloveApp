using System;
using System.Collections.Generic;
using OpenGloveApp.Extensions;
using OpenGloveApp.Models;
using Xamarin.Forms;

namespace OpenGloveApp.Pages
{
    public partial class ConfigurationPage : ContentPage
    {
        public static List<string> OpenGloveConfigurations = new List<string> {"OpenGloveIZQ.xml", "OpenGloveDER.xml", "Helmet.xml", "Jacket"};
        public static OpenGloveConfiguration OpenGloveConfiguration = new OpenGloveConfiguration();

        public ConfigurationPage()
        {
            InitializeComponent();

            pickerCurrentConfiguration.ItemsSource = OpenGloveConfigurations;

            BuildGridMenuItems();
            ConfigureToolbarItemsByPlatform();
        }

        public void BuildGridMenuItems()
        {
            Color gridBackGroundColor = Color.Transparent; // changue this color for design the grid layout
            Color itemBackGroundColor = Color.Transparent; // changue this color for design the grid layout

            var grid = new Grid()
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                RowSpacing = 20,
                ColumnSpacing = 20,
                Padding = new Thickness(20, 20, 20, 20),
                BackgroundColor = gridBackGroundColor,
            };

            var board = new ImageCircleTouchable(this)
            {
                Source = "board",
                Text = "Boards",
                BackgroundColor = itemBackGroundColor,
            };

            var actuators = new ImageCircleTouchable(this)
            {
                Source = "actuator_mapping_back",
                Text = "Actuators",
                BackgroundColor = itemBackGroundColor,
            };

            var flexors = new ImageCircleTouchable(this)
            {
                Source = "flexor_mapping",
                Text = "Flexors",
                BackgroundColor = itemBackGroundColor,
            };

            var IMU = new ImageCircleTouchable(this)
            {
                Source = "IMU_sensor",
                Text = "IMU",
                BackgroundColor = itemBackGroundColor,
            };

            grid.Children.Add(board, 0, 0);
            grid.Children.Add(actuators, 1, 0);
            grid.Children.Add(flexors, 2, 0);
            grid.Children.Add(IMU, 0, 1);

            Menu.Children.Add(grid);
        }

        public void ConfigureToolbarItemsByPlatform()
        {
            switch(Device.RuntimePlatform)
            {
                case Device.Android:
                    ToolbarItemSave.Order = ToolbarItemOrder.Primary;
                    ToolbarItemSave.Priority = 0;
                    ToolbarItemHelp.Order = ToolbarItemOrder.Primary;
                    ToolbarItemHelp.Priority = 1;
                    break;
                case Device.iOS:
                    ToolbarItemSave.Order = ToolbarItemOrder.Primary;
                    ToolbarItemSave.Priority = 0;
                    ToolbarItemHelp.Order = ToolbarItemOrder.Primary;
                    ToolbarItemHelp.Priority = 1;
                    break;
                default:
                    break;
            }
        }

        void Handle_Activated(object sender, System.EventArgs e)
        {
            DisplayAlert("Settings Activated", "Your pressed: " + ((ToolbarItem)sender).Text, "OK");
        }

        // Method to subscribe to OpenGloveApp.Extensions.ImageCircleTouchable for ItemClicked
        public void OnMenuItemClicked(object source, EventArgs e)
        {
            var menuItemText = ((ImageCircleTouchable)source).Text;

            switch (menuItemText)
            {
                case "Boards":
                    Navigation.PushAsync(new BoardsPage()
                    { 
                        Padding = AppConstants.AppConstants.ChildrenPagePadding,
                        Title = (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS) ?  "Boards": "Boards Configuration",
                    });
                    break;
                case "Actuators":
                    Navigation.PushAsync(new ActuatorsPage()
                    {
                        Padding = AppConstants.AppConstants.ChildrenPagePadding,
                        Title = (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS) ? "Actuators" : "Actuators Configuration",
                    });
                    break;
                case "Flexors":
                    Navigation.PushAsync(new FlexorsPage()
                    {
                        Padding = AppConstants.AppConstants.ChildrenPagePadding,
                        Title = (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS) ? "Flexors" : "Flexors Configuration",
                    });
                    break;
                case "IMU":
                    Navigation.PushAsync(new IMUPage()
                    { 
                        Padding = AppConstants.AppConstants.ChildrenPagePadding,
                        Title = (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS) ? "IMU" : "IMU Configuration",
                    });
                    break;
                default:
                    break;
            }
        }

        public async void CreateNewConfiguration(object sender, System.EventArgs e)
        {
            await Animations.AnimationTouch((View)sender);
            await DisplayAlert("New configuration", "hi", "OK");
        }
    }
}
