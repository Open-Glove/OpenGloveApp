using System.Collections.Generic;
using System.Threading.Tasks;
using OpenGloveApp.Models;
using Xamarin.Forms;

namespace OpenGloveApp.OpenGloveAPI
{
    /// <summary>
    /// Represents  a communication instance between the API and the glove. 
    /// Provide methods for send and receive data through serial port
    /// </summary>
    public interface ICommunication
    {

        List<BluetoothDevices> GetAllPairedDevices();
        void OpenDeviceConnection(string bluetoothDeviceName);
        void CloseDeviceConnection();
        void Write(string message);
        string ReadLine();
    }
}

/// <summary>
        /// Returns an list with all paired BluetoothDevices (Name and MAC direction)
        /// </summary>
        /// <returns>An list with the paired BluetoothDevices</returns>
/// 
///         /// <summary>
        /// Open a Connection with the Bluetooth Device
        /// </summary>
        /// <param name="bluetoothDevice"> The device to connect </param>
