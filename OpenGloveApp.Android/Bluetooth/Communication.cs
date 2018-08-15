using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Android.Bluetooth;
using OpenGloveApp.CustomEventArgs;
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
        private Hashtable mBoundedDevices = new Hashtable();
        private static ConnectedThread mBluetoothManagement;

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

        public void OpenDeviceConnection(BluetoothDevices bluetoothDevice)
        {
            if (mBluetoothAdapter != null)
            {
                if (mBluetoothAdapter.IsEnabled)
                {
                    mDevice = mBoundedDevices[bluetoothDevice.Name] as BluetoothDevice;
                    ConnectThread connectThread = new ConnectThread(mDevice);
                    connectThread.Start();
                }
            }
        }

        public void OpenDeviceConnection(ContentPage contentPage, BluetoothDevices bluetoothDevice)
        {
            if (mBluetoothAdapter != null)
            {
                if (mBluetoothAdapter.IsEnabled)
                {
                    mDevice = mBoundedDevices[bluetoothDevice.Name] as BluetoothDevice;
                    ConnectThread connectThread = new ConnectThread(contentPage, mDevice);
                    connectThread.Start();
                }
            }
        }

        public List<BluetoothDevices> GetAllPairedDevices()
        {
            if (mBluetoothAdapter == null) return null;

            mBoundedDevices.Clear();
            mBoundedDevicesModel.Clear();


            var devices = mBluetoothAdapter.BondedDevices;

            if (devices != null && devices.Count > 0)
            {
                foreach (BluetoothDevice device in devices)
                {
                    mBoundedDevices.Add(device.Name, device);
                    mBoundedDevicesModel.Add(new BluetoothDevices
                    {
                        Name = device.Name,
                        Address = device.Address
                    });
                }
            }
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
            mBluetoothManagement.Write(message);
        }

        public string ReadLine()
        {
            return mBluetoothManagement.ReadLine();
        }

        public void ClosePort()
        {
            throw new NotImplementedException();
        }

        public class ConnectThread : Java.Lang.Thread
        {
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
                    if (mmContentPage != null)
                        mBluetoothManagement = new ConnectedThread(mmContentPage, mmSocket);
                    else
                        mBluetoothManagement = new ConnectedThread(mmDevice, mmSocket);
                    mBluetoothManagement.Start();
                }
                catch (Java.IO.IOException e)
                {
                    mmSocket.Close();
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
            private List<int> mFlexorPins = new List<int> { 17 }; //TODO get this from OpenGloveApp
            public int mEvaluation = 0; //OpenGloveAppPage.FLEXOR_EVALUATION; //OpenGloveAppPage.MOTOR_EVALUATION;

            public ConnectedThread(BluetoothDevice device, BluetoothSocket bluetoothSocket)
            {
                mmDevice = device;
                mmDeviceName = device.Name;
                mmSocket = bluetoothSocket;
                try
                {
                    mmInputStreamReader = new StreamReader(mmSocket.InputStream);
                    mmOutputStream = mmSocket.OutputStream;
                    this.BluetoothDataReceived += Server.OpenGloveServer.OnBluetoothMessage; // The WebSocket Server subscribe to this instance of ConnectedThread (Bluetooth Device)
                    //Server.OpenGloveServer.WebSocketMessageReceived += this.OnWebSocketServerMessage; // The Thread subscribe to WebSocket Server

                }
                catch (System.IO.IOException e)
                {
                    mmInputStreamReader.Close();
                    mmOutputStream.Close();
                    Debug.WriteLine(e.Message);
                }
            }

            public ConnectedThread(ContentPage contentPage, BluetoothSocket bluetoothSocket)
            {
                mmSocket = bluetoothSocket;
                try
                {
                    mmInputStreamReader = new StreamReader(mmSocket.InputStream);
                    mmOutputStream = mmSocket.OutputStream;

                    //subscribe UI thread on this especific ConnectedThread for get data
                    this.BluetoothDataReceived += ((OpenGloveAppPage)contentPage).OnBluetoothMessage; //UI thread subscribe to this instance of ConnectedThread
                    //this.BluetoothDataReceived += Server.OpenGloveServer.OnBluetoothMessage; // The WebSocket Server subscribe to this instance of ConnectedThread (Bluetooth Device)
                    ((OpenGloveAppPage)contentPage).BluetoothMessageSended += this.OnBluetoothMessageSended; //This thread subscribe to UI thread for get commands

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

            // TODO delete this handler!!! is deprecated
            // Handle event from WebSocket Server data
            public void OnWebSocketServerMessage(object source, WebSocketEventArgs e)
            {
                // Only accept message to Bluetooth Device managed for this ConnectedThread.
                // TODO: Improve this with specific listeners for each connected thread(openglove devices connected).
                if (e.DeviceName == mmDeviceName)
                    Debug.WriteLine("Communication.Android: Delete This method");
            }



            //Handle event from UI thread
            public void OnBluetoothMessageSended(object source, BluetoothEventArgs e)
            {
                int index = 3; //0
                int count = 1; //2
                switch (e.What)
                {
                    case OpenGloveAppPage.INITIALIZE_MOTORS:
                        {
                            Debug.WriteLine($"INITIALIZE_MOTORS: Initializing motors (thread: {this.Id})");
                            string message = mMessageGenerator.InitializeMotor(((List<int>)e.Pins).GetRange(0, 2).ToArray());
                            this.Write(message);
                            break;
                        }
                    case OpenGloveAppPage.ACTIVATE_MOTORS:
                        {   
                            Debug.WriteLine($"ACTIVATE_MOTORS: Activating motors (thread: {this.Id})");
                            Debug.WriteLine($"ValuesON: {((List<string>)e.ValuesON).Count}");
                            Debug.WriteLine($"ValuesON.range(0,2): { ((List<string>)e.ValuesON).GetRange(index, count).ToArray() }");
                            Debug.WriteLine($"Pins.range(0,2).Count: { ((List<int>)e.Pins).GetRange(index, count).Count }");
                            string message = mMessageGenerator.ActivateMotor(((List<int>)e.Pins).GetRange(index, count).ToArray(), ((List<string>)e.ValuesON).GetRange(index, count).ToArray());
                            this.Write(message);
                            break;
                        }
                    case OpenGloveAppPage.DISABLE_MOTORS:
                        {
                            Debug.WriteLine($"DISABLE_MOTORS: Disable motors (thread: {this.Id})");
                            string message = mMessageGenerator.ActivateMotor(((List<int>)e.Pins).GetRange(index, count).ToArray(), ((List<string>)e.ValuesOFF).GetRange(index, count).ToArray());
                            this.Write(message);
                            break;
                        }
                    case OpenGloveAppPage.FLEXOR_READ:
                        {
                            //Debug.WriteLine($"FLEXOR_READ: FOR READ PINS (thread: {this.Id})");
                            //mFlexorPins = e.FlexorPins as List<int>;
                            break;
                        }
                    case OpenGloveAppPage.FLEXOR_EVALUATION:
                        {
                            mEvaluation = OpenGloveAppPage.FLEXOR_EVALUATION;
                            break;
                        }
                    case OpenGloveAppPage.MOTOR_EVALUATION:
                        {
                            mEvaluation = OpenGloveAppPage.MOTOR_EVALUATION;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }

            override
            public void Run()
            {

                switch (mEvaluation)
                {
                    case OpenGloveAppPage.FLEXOR_EVALUATION:
                        FlexorTest(1000, 1, "latency-test", "flexor1XamarinNexus.csv");
                        break;
                    case OpenGloveAppPage.MOTOR_EVALUATION:
                        MotorTest(1000, 5, "latency-test", "motor5XamarinNexus.csv");
                        break;
                    default:
                        FlexorCapture();
                        break;
                }
            }

            private void FlexorCapture()
            {
                // Keep listening to the InputStream whit a StreamReader until an exception occurs
                string line;

                OpenGloveServer.OpenGloveByDeviceName[mmDeviceName].InitializeActuators();
                OpenGloveServer.OpenGloveByDeviceName[mmDeviceName].InitializeFlexors();

                while (true)
                {
                    try
                    {
                        //line = AnalogRead(mFlexorPins[0]);
                        line = ReadLine();

                        if (line != null)
                        {
                            //Raise the event to WebSocket Server, that need stay subscriber to this publisher thread
                            OnBluetoothDataReceived(this.Id, this.mmDeviceName, line);
                        }
                        else
                        {
                            Debug.WriteLine($"BluetoothSocket is Disconnected, Trying Connect");
                            mmSocket.Connect();
                            OpenGloveServer.OpenGloveByDeviceName[mmDeviceName].InitializeActuators();
                            OpenGloveServer.OpenGloveByDeviceName[mmDeviceName].InitializeFlexors();
                        }
                    }
                    catch (Java.IO.IOException e)
                    {
                        Debug.WriteLine($"CONNECTED THREAD {this.Id}: {e.Message}");
                    }
                }
            }

            private void FlexorTest(int samples, int flexors, String folderName, String fileName)
            {
                // Keep listening to the InputStream whit a StreamReader until an exception occurs
                string line;
                int counter = 0;
                List<long> latencies = new List<long>();
                IO.CSV csvWriter = new IO.CSV(folderName, fileName);

                Debug.WriteLine(csvWriter.ToString());

                Stopwatch stopWatch = new Stopwatch(); // for capture elapsed time
                TimeSpan ts;

                while (true)
                {
                    try
                    {
                        Debug.WriteLine("Counter: " + counter);
                        stopWatch = new Stopwatch();
                        stopWatch.Start();
                        line = AnalogRead(mFlexorPins[0]);
                        stopWatch.Stop();
                        ts = stopWatch.Elapsed;

                        if (counter < samples)
                        {
                            latencies.Add(ts.Ticks * 100); // nanoseconds https://msdn.microsoft.com/en-us/library/system.datetime.ticks(v=vs.110).aspx
                            if ((counter + 1) % 100 == 0) Debug.WriteLine("Counter: " + counter);
                        }
                        else
                        {
                            break;
                        }
                        counter++;

                        if (line != null)
                        {

                            //Raise the event to UI thread, that need stay subscriber to this publisher thread
                            //Send the current thread id and send Message
                            OnBluetoothDataReceived(this.Id, this.mmDeviceName, line);
                        }
                        else
                        {
                            Debug.WriteLine($"BluetoothSocket is Disconnected");
                            mmSocket.Connect();
                        }
                    }
                    catch (Java.IO.IOException e)
                    {
                        Debug.WriteLine($"CONNECTED THREAD {this.Id}: {e.Message}");
                    }
                }

                csvWriter.Write(latencies, "latencies-ns");
                Debug.WriteLine(csvWriter.ToString());
            }

            private void MotorTest(int samples, int motors, string folderName, string fileName)
            {
                string message;
                int counter = 0;
                List<long> latencies = new List<long>();
                List<int> pins = new List<int>(OpenGloveAppPage.mPins.GetRange(0, motors * 2));
                List<string> valuesON = new List<string>(OpenGloveAppPage.mValuesON.GetRange(0, motors * 2));
                List<string> valuesOFF = new List<string>(OpenGloveAppPage.mValuesOFF.GetRange(0, motors * 2));
                IO.CSV csvWriter = new IO.CSV(folderName, fileName);

                Debug.WriteLine(csvWriter.ToString());

                Stopwatch stopWatch = new Stopwatch(); // for capture elapsed time
                TimeSpan ts;

                while (true)
                {
                    if (counter < samples)
                    {
                        try
                        {
                            stopWatch = new Stopwatch();
                            stopWatch.Start();

                            message = mMessageGenerator.ActivateMotor(pins, valuesON);
                            this.Write(message); // Activate the motors

                            message = mMessageGenerator.ActivateMotor(pins, valuesOFF);
                            this.Write(message); // Disable the motors

                            stopWatch.Stop();
                            ts = stopWatch.Elapsed;

                            latencies.Add(ts.Ticks * 100);
                            if ((counter + 1) % 100 == 0) Debug.WriteLine("Counter: " + counter);
                        }
                        catch (Java.IO.IOException e)
                        {
                            e.PrintStackTrace();
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                    counter++;
                }

                csvWriter.Write(latencies, "latencies-ns");
                Debug.WriteLine(csvWriter.ToString());
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
                }
                catch (Java.IO.IOException e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }
    }
}
