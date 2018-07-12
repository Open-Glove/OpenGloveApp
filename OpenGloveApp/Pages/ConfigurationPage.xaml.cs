﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenGloveApp.Extensions;
using Xamarin.Forms;

namespace OpenGloveApp.Pages
{
    public partial class ConfigurationPage : ContentPage
    {
        public ConfigurationPage()
        {
            InitializeComponent();

            Color gridBackGroundColor = Color.Transparent; // changue this color for design the layout
            Color itemBackGroundColor = Color.Transparent; // changue this color for design the layout

            var grid = new Grid()
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                RowSpacing = 20,
                ColumnSpacing = 20,
                Padding = new Thickness(20, 20 , 20, 20),
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

            //for subscribe OnMenuClicked to ImageCircleTouchables
            grid.Children.Add(board, 0, 0);
            grid.Children.Add(actuators, 1, 0);
            grid.Children.Add(flexors, 2, 0);
            grid.Children.Add(IMU, 0, 1);

            Menu.Children.Add(grid);

        }

        // Method to subscribe to CircleImageTouchable ItemClicked
        public void OnMenuItemClicked(object source, EventArgs e)
        {
            var menuItemText = ((ImageCircleTouchable)source).Text;

            switch (menuItemText)
            {
                case "Boards":
                    Navigation.PushAsync(new DevicesPage());
                    break;
                case "Actuators":
                    Navigation.PushAsync(new ActuatorsPage());
                    break;
                case "Flexors":
                    Navigation.PushAsync(new FlexorsPage());
                    break;
                case "IMU":
                    Navigation.PushAsync(new IMUPage());
                    break;
                default:
                    break;
            }
        }
    }
}
