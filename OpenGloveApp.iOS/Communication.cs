﻿using System;
using System.Collections.Generic;
using OpenGloveApp.Droid;
using OpenGloveApp.Models;
using OpenGloveApp.OpenGloveAPI;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Communication))]
namespace OpenGloveApp.Droid
{
    public class Communication : ICommunication
    {
        public void ClosePort()
        {
            throw new NotImplementedException();
        }

        public List<BluetoothDevices> GetAllPairedDevices()
        {
            throw new NotImplementedException();
        }

        public string[] GetPortNames()
        {
            throw new NotImplementedException();
        }

        public void OpenDeviceConnection(ContentPage contentPage, BluetoothDevices bluetoothDevice)
        {
            throw new NotImplementedException();
        }

        public void OpenPort(string portName, int baudRate)
        {
            throw new NotImplementedException();
        }

        public string ReadLine()
        {
            throw new NotImplementedException();
        }

        public void Write(string data)
        {
            throw new NotImplementedException();
        }
    }
}
