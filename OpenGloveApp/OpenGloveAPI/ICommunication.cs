using System.Collections.Generic;
using OpenGloveApp.Models;
using Xamarin.Forms;
//using System.Text.RegularExpressions; // use in Communication for windows

namespace OpenGloveApp.OpenGloveAPI
{
    /// <summary>
    /// Represents  a communication instance between the API and the glove. 
    /// Provide methods for send and receive data through serial port
    /// </summary>
    public interface ICommunication
    {
        /// <summary>
        /// Returns an list with all paired BluetoothDevices (Name and MAC direction)
        /// </summary>
        /// <returns>An list with the paired BluetoothDevices</returns>
        List<BluetoothDevices> GetAllPairedDevices();
        /// <summary>
        /// Open a Connection with 
        /// </summary>
        /// <param name="contentPage"> The Page of xamarin.forms, the thread UI subscribe to Connected Thread</param>
        /// <param name="bluetoothDevice"> The device to connect </param>
        void OpenDeviceConnection(ContentPage contentPage, BluetoothDevices bluetoothDevice);
        /// <summary>
        /// Returns an array with all active serial ports names
        /// </summary>
        /// <returns>An array with the names of all active serial ports</returns>
        string[] GetPortNames();
        /// <summary>
        /// Open a new connection with the specified port and baud rate
        /// </summary>
        ///<param name = "portName" >Name of the serial port to open a communication</param>
        /// <param name="baudRate">Data rate in bits per second. Use one of these values: 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400, 57600, or 115200</param>
        void OpenPort(string portName, int baudRate);
        /// <summary>
        /// Send the string to the serial port
        /// </summary>
        /// <param name="data">String data to send</param>
        void Write(string data);
        /// <summary>
        /// Read the input buffet until a next line character
        /// </summary>
        /// <returns>A string without the next line character</returns>
        string ReadLine();
        /// <summary>
        /// Close the serial communication
        /// </summary>
        void ClosePort();
    }
}
