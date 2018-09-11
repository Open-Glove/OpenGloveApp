using System;
using System.Collections.Generic;
using OpenGloveApp.Droid;
using OpenGloveApp.Models;
using OpenGloveApp.OpenGloveAPI;

[assembly: Xamarin.Forms.Dependency(typeof(Communication))]
namespace OpenGloveApp.Droid
{
    public class Communication : ICommunication
    {
        public List<BluetoothDevices> GetAllPairedDevices()
        {
            throw new NotImplementedException();
        }

        public void CloseDeviceConnection()
        {
            throw new NotImplementedException();
        }

        public void OpenDeviceConnection(string bluetoothDeviceName)
        {
            throw new NotImplementedException();
        }

        public void Write(string message)
        {
            throw new NotImplementedException();
        }

        public string ReadLine()
        {
            throw new NotImplementedException();
        }
    }
}
