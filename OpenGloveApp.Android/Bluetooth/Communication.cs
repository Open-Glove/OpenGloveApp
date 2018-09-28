using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Android.Bluetooth;
using OpenGloveApp.Droid.Bluetooth;
using OpenGloveApp.Models;
using OpenGloveApp.OpenGloveAPI;
using OpenGloveApp.Server;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Communication))]
namespace OpenGloveApp.Droid.Bluetooth
{
    public class Communication : ICommunication
    {
        // Universally Unique Identifier
        private const string mUUID = "1e966f42-52a8-45db-9735-5db0e21b881d";
        private BluetoothAdapter mBluetoothAdapter;
        private BluetoothDevice mDevice;
        private List<BluetoothDevices> mBoundedDevicesModel;
        private Dictionary<string, BluetoothDevice> mBoundedDeviceByDeviceName = new Dictionary<string, BluetoothDevice>();
        private static ConnectedThread mBluetoothManagementThread;

        public Communication()
        {
            mBoundedDevicesModel = new List<BluetoothDevices>();
            mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;
        }

        public Java.Util.UUID GetUUID()
        {
            return Java.Util.UUID.FromString(mUUID);
        }

        private void Close(IDisposable aConnectedObject)
        {
            if (aConnectedObject == null) return;
            try
            {
                aConnectedObject.Dispose();
            }
            catch (Exception e)
            {
                throw e;
            }
            aConnectedObject = null;
        }

        public void OpenDeviceConnection(string bluetoothDeviceName)
        {
            if (mBluetoothAdapter != null)
            {
                if (mBluetoothAdapter.IsEnabled)
                {
                    UpdatePairedDevices();

                    if(mBoundedDeviceByDeviceName.ContainsKey(bluetoothDeviceName))
                    {
                        mDevice = mBoundedDeviceByDeviceName[bluetoothDeviceName];
                        ConnectThread connectThread = new ConnectThread(mDevice);
                        connectThread.Start();
                    }
                }
            }
        }

        public void CloseDeviceConnection()
        {
            if(mBluetoothManagementThread != null)
                    mBluetoothManagementThread.Close();
        }

        public void UpdatePairedDevices()
        {
            if (mBluetoothAdapter == null) return;

            mBoundedDeviceByDeviceName.Clear();
            mBoundedDevicesModel.Clear();

            var devices = mBluetoothAdapter.BondedDevices;

            if (devices != null && devices.Count > 0)
            {
                foreach (BluetoothDevice device in devices)
                {
                    mBoundedDeviceByDeviceName.Add(device.Name, device);
                    mBoundedDevicesModel.Add(new BluetoothDevices
                    {
                        Name = device.Name,
                        Address = device.Address
                    });
                }
            }
        }

        public List<BluetoothDevices> GetAllPairedDevices()
        {
            UpdatePairedDevices();
            return mBoundedDevicesModel;
        }

        public string[] GetPortNames()
        {
            throw new NotImplementedException();
        }

        public void OpenPort(string portName, int baudRate)
        {
            throw new NotImplementedException();
        }

        public void Write(string message)
        {
            if (mBluetoothManagementThread != null)
                if (mBluetoothManagementThread.IsAlive)
                    mBluetoothManagementThread.Write(message);
        }

        public string ReadLine()
        {
            if (mBluetoothManagementThread != null)
                return mBluetoothManagementThread.ReadLine();
            else
                return null;
        }

        public void ClosePort()
        {
            throw new NotImplementedException();
        }

        public class ConnectThread : Java.Lang.Thread
        {
            // Event for send data from Bluetooth Socket to subcribers
            public event EventHandler<BluetoothEventArgs> BluetoothDataReceived;
            private BluetoothSocket mmSocket;
            private BluetoothDevice mmDevice;
            private BluetoothAdapter mmBluetoothAdapter;
            private ContentPage mmContentPage;

            public ConnectThread(BluetoothDevice device)
            {
                BluetoothSocket auxSocket = null;
                mmDevice = device;

                try
                {
                    Java.Util.UUID uuid = Java.Util.UUID.FromString(mUUID);
                    auxSocket = (BluetoothSocket)mmDevice.Class.GetMethod("createRfcommSocket", new Java.Lang.Class[] { Java.Lang.Integer.Type }).Invoke(mmDevice, 1);
                    Debug.WriteLine("BluetoothSocket: CREATED");
                    Debug.WriteLine($"Name: {device.Name}, Address: {device.Address}");
                }
                catch (Java.IO.IOException e)
                {
                    Debug.WriteLine("BluetoothSocket: NOT CREATED");
                    e.PrintStackTrace();
                }
                mmSocket = auxSocket;
            }

            public ConnectThread(ContentPage contentPage, BluetoothDevice device)
            {
                BluetoothSocket auxSocket = null;
                mmDevice = device;
                mmContentPage = contentPage;

                try
                {
                    Java.Util.UUID uuid = Java.Util.UUID.FromString(mUUID);
                    auxSocket = (BluetoothSocket)mmDevice.Class.GetMethod("createRfcommSocket", new Java.Lang.Class[] { Java.Lang.Integer.Type }).Invoke(mmDevice, 1);
                    Debug.WriteLine("BluetoothSocket: CREATED");
                    Debug.WriteLine($"Name: {device.Name}, Address: {device.Address}");
                }
                catch (Java.IO.IOException e)
                {
                    Debug.WriteLine("BluetoothSocket: NOT CREATED");
                    e.PrintStackTrace();
                }
                mmSocket = auxSocket;
            }

            // Method for raise the event: Bluetooth Socket (handled by thread) to WebSocket Server
            protected virtual void OnBluetoothDataReceived(long threadId, string deviceName, string message)
            {
                BluetoothDataReceived?.Invoke(this, new BluetoothEventArgs()
                { ThreadId = threadId, DeviceName = deviceName, Message = message });
            }

            override
            public void Run()
            {
                // Cancel discovery because it will slow down the connection
                mmBluetoothAdapter = BluetoothAdapter.DefaultAdapter;
                mmBluetoothAdapter.CancelDiscovery();

                try
                {
                    // Connect the device through the socket. This will block
                    // until it succeeds or throws an exception
                    mmSocket.Connect();
                    Debug.WriteLine("BluetoothSocket: CONNECTED");
                    // Do work to manage the connection (in a separate thread)
                    // TODO manageConnectedSocket(mmSocket);
                    mBluetoothManagementThread = new ConnectedThread(mmDevice, mmSocket);
                    mBluetoothManagementThread.Start();
                    OnBluetoothDataReceived(this.Id, this.mmDevice.Name, "b," + mmSocket.IsConnected.ToString()); // b,True Or b,False for BluetoothDeviceConnection state
                }
                catch (Java.IO.IOException e)
                {
                    mmSocket.Close();
                    OnBluetoothDataReceived(this.Id, this.mmDevice.Name, "b," + "False"); // b,True Or b,False for BluetoothDeviceConnection state
                    Debug.WriteLine("BluetoothSocket: NOT CONNECTED");
                    e.PrintStackTrace();
                }
            }
        }


        public class ConnectedThread : Java.Lang.Thread
        {
            // Event for send data from Bluetooth Socket to subcribers
            public event EventHandler<BluetoothEventArgs> BluetoothDataReceived;

            private string mmDeviceName;
            private BluetoothDevice mmDevice;
            private BluetoothSocket mmSocket;
            private StreamReader mmInputStreamReader;
            private Stream mmOutputStream;
            private MessageGenerator mMessageGenerator = new MessageGenerator();
            private List<int> mFlexorPins = new List<int> { 17 }; //TODO delete this
            private bool TurnON = true;
            public bool mIsBluetoothDeviceConnected = false;

            public ConnectedThread(BluetoothDevice device, BluetoothSocket bluetoothSocket)
            {
                mmDevice = device;
                mmDeviceName = device.Name;
                mmSocket = bluetoothSocket;
                mIsBluetoothDeviceConnected = mmSocket.IsConnected;

                try
                {
                    mmInputStreamReader = new StreamReader(mmSocket.InputStream);
                    mmOutputStream = mmSocket.OutputStream;
                    this.BluetoothDataReceived += OpenGloveServer.OnBluetoothMessage; // The WebSocket Server subscribe to this instance of ConnectedThread (Bluetooth Device)
                }
                catch (System.IO.IOException e)
                {
                    mmInputStreamReader.Close();
                    mmOutputStream.Close();
                    Debug.WriteLine(e.Message);
                }
            }

            // Method for raise the event: Bluetooth Socket (handled by thread) to WebSocket Server
            protected virtual void OnBluetoothDataReceived(long threadId, string deviceName, string message)
            {
                BluetoothDataReceived?.Invoke(this, new BluetoothEventArgs()
                { ThreadId = threadId, DeviceName = deviceName, Message = message });
            }

            override
            public void Run()
            {
                MainRutine(); //You can add other Rutines for Management BluetoothDeviceConnection
            }

            private void MainRutine()
            {
                // Keep listening to the InputStream whit a StreamReader until an exception occurs
                string line;

                OnBluetoothDataReceived(this.Id, this.mmDeviceName, "b," + mmSocket.IsConnected.ToString()); // b,True Or b,False for BluetoothDeviceConnection state
                OpenGloveServer.OpenGloveByDeviceName[mmDeviceName].IsConnected = mmSocket.IsConnected;
                OpenGloveServer.OpenGloveByDeviceName[mmDeviceName].InitializeOpenGloveConfigurationOnDevice();

                while (TurnON)
                {
                    this.mIsBluetoothDeviceConnected = mmSocket.IsConnected;

                    try
                    {
                        line = ReadLine();

                        if (line != null)
                        {
                            //Debug.WriteLine($"from Communication.ConnectedThread {this.Id}, line from BluetoothDevice: {line}");
                            //Raise the event to WebSocket Server, that need stay subscriber to this publisher thread
                            OnBluetoothDataReceived(this.Id, this.mmDeviceName, line);
                        }
                        else
                        {
                            Debug.WriteLine($"BluetoothSocket is Disconnected, Trying Connect");
                            OnBluetoothDataReceived(this.Id, this.mmDeviceName, "b," + "False");
                            OpenGloveServer.OpenGloveByDeviceName[mmDeviceName].IsConnected = mmSocket.IsConnected;
                            mmSocket.Connect();

                            OnBluetoothDataReceived(this.Id, this.mmDeviceName, "b," + mmSocket.IsConnected.ToString());
                            OpenGloveServer.OpenGloveByDeviceName[mmDeviceName].IsConnected = mmSocket.IsConnected;
                            OpenGloveServer.OpenGloveByDeviceName[mmDeviceName].InitializeOpenGloveConfigurationOnDevice();
                        }
                    }
                    catch (Java.IO.IOException e)
                    {
                        Debug.WriteLine($"CONNECTED THREAD {this.Id}: {e.Message}");
                    }
                }
            }

            public string AnalogRead(int pin)
            {
                string message = mMessageGenerator.AnalogRead(pin);
                this.Write(message);
                try
                {
                    return mmInputStreamReader.ReadLine();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    return null;
                }
            }

            public string ReadLine()
            {
                try
                {
                    return mmInputStreamReader.ReadLine();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    return null;
                }
            }

            public void Write(string message)
            {
                try
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(message);
                    mmOutputStream.Write(bytes, 0, bytes.Length);
                }
                catch (Java.IO.IOException e)
                {
                    Debug.WriteLine(e.Message);
                }
            }

            public void Close()
            {
                try
                {
                    mmSocket.Close();
                    TurnON = false;
                    OnBluetoothDataReceived(this.Id, this.mmDeviceName, "b," + "False");
                    OpenGloveServer.OpenGloveByDeviceName[mmDeviceName].IsConnected = false;
                }
                catch (Java.IO.IOException e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }
    }
}
