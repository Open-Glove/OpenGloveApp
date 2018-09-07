using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Fleck;
using OpenGloveApp.AppConstants;
using OpenGloveApp.Models;
using OpenGloveApp.OpenGloveAPI;

namespace OpenGloveApp.Server
{
    public class OpenGloveServer: IServer
    {
        private string Url { get; set; }
        private WebSocketServer Server; // sample "ws://127.0.0.1:7070"
        private List<IWebSocketConnection> AllSockets = new List<IWebSocketConnection>();
        private static Dictionary<string, IWebSocketConnection> webSocketByDeviceName = new Dictionary<string, IWebSocketConnection>();
        public static Dictionary<string, OpenGlove> OpenGloveByDeviceName = new Dictionary<string, OpenGlove>();

        public static Dictionary<string, OpenGloveConfiguration> ConfigurationByName = new Dictionary<string, OpenGloveConfiguration>();
        //public static Dictionary<string, OpenGloveConfiguration> ConfigurationByDeviceName = new Dictionary<string, OpenGloveConfiguration>();

        //For Actuators Activation for Latency Time Test
        private Stopwatch stopwatch;
        private TimeSpan timeSpan;

        public OpenGloveServer(string url)
        {
            this.Url = url;
        }

        public void StartWebsockerServer()
        {
            FleckLog.Level = LogLevel.Debug;
            Server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Debug.WriteLine("Open WebSocket!");
                    AllSockets.Add(socket);
                };
                socket.OnClose = () =>
                { 
                    Debug.WriteLine("Close WebSocket!");
                    try
                    {
                        if (webSocketByDeviceName.ContainsValue(socket) || AllSockets.Contains(socket))
                        {
                            var item = webSocketByDeviceName.First(entry => entry.Value == socket);
                            webSocketByDeviceName.Remove(item.Key);
                            AllSockets.Remove(socket);
                        }
                    }
                    catch
                    {
                        Debug.WriteLine("WebSocketError: try quit webSocket from list of websocket connected");
                    }

                };
                socket.OnMessage = message =>
                {
                    HandleMessage(socket, message);
                };
            });
        }

        public void CloseAllSockets()
        {
            AllSockets.ToList().ForEach(s => s.Close());
        }

        public void Start()
        {
            this.Server = new WebSocketServer(Url);
            ConfigureServer();
            StartWebsockerServer();
        }

        public void Stop()
        {
            CloseAllSockets();
            Server.Dispose(); //this method does not allow more incoming connections, and disconnect the server. So it is necessary to disconnect all sockets first to cut off all communication
            AllSockets.Clear();
        }

        public void ConfigureServer()
        {
            Server.RestartAfterListenError = true;
        }

        /* WebSocket format message:  ACTION;DEVICE;REGIONS;VALUES;EXTRA_VALUES
         * sample:      Activate;OpenGloveIZQ;0,1,2,3;100,HIGH,LOW,255
         * Action:      one of this Actions in enum OpenGloveActions: StartCaptureData = 0, StopCaptureData = 1, ...
         * DeviceName:  Name of bluetooth device to apply the command
         * Regions:     a list of regions to activate (int)
         * Values (Intensities, Pins, single value): [Intensities: 0 to 255, HIGH and LOW] [Pins: number of the pin on board] [single value how: True, False, integer]
        */
        // Handle Message from WebSocket Client
        private void HandleMessage(IWebSocketConnection socket, string message)
        {
            Debug.WriteLine(message);
            string[] words;             // ACTION;DEVICE;REGIONS;VALUES;EXTRA_VALUES
            int CountMessageSplit = 5;

            if (message != null)
            {
                try
                {
                    words = message.Split(';');
                    int count = words.Length;

                    int action = -1;
                    string deviceName = null;
                    string regions = null;
                    string values = null; // intensities or pins
                    string extraValues = null; // for add actuators ACTION;DEVICE;REGIONS;POSITIVE_PINS;NEGATIVE_PINS

                    if(count == CountMessageSplit)
                    {
                        action = Int32.Parse(words[0]);
                        deviceName = words[1];
                        regions = words[2];
                        values = words[3];
                        extraValues = words[4];

                        SwitchOpenGloveActions(socket, message, action, deviceName, regions, values, extraValues);
                    }
                }
                catch
                {
                    Debug.WriteLine("WebSocketServer ERROR: BAD FORMAT in HandleMessage");
                }
            }

        }

        // For add news actions on OpenGloveServer
        public void SwitchOpenGloveActions(IWebSocketConnection socket, string message, int what, string deviceName, string regions, string values, string extraValues)
        {
            int Region = -1;
            List<int> Regions = null;
            List<string> Intensities = null;
            int Pin = -1;
            int ExtraPin;
            List<int> Pins = null;
            List<int> ExtraPins = null;
            string Value = null;
            int IntegerValue = -1;

            try
            {
                switch (what)
                {
                    case (int)OpenGloveActions.StartOpenGlove:
                        InitializeOpenGloveConfigurationOnDevice(deviceName);
                        break;

                    case (int)OpenGloveActions.StopOpenGlove:
                        OpenGloveByDeviceName[deviceName].TurnOffAllOpenGloveComponents();
                        break;

                    case (int)OpenGloveActions.AddOpenGloveDeviceToServer:
                        Value = values;
                        AddOpenGloveDeviceToServer(deviceName, Value);
                        break;

                    case (int)OpenGloveActions.RemoveOpenGloveDeviceFromServer:
                        RemoveOpenGloveDeviceFromServer(deviceName);
                        break;

                    case (int)OpenGloveActions.SaveOpenGloveConfiguration:
                        Value = values;
                        SaveOpenGloveConfiguration(deviceName, Value);
                        break;

                    case (int)OpenGloveActions.ConnectToBluetoothDevice:
                        TryConnectToBluetoothDevice(socket, deviceName);
                        break;

                    case (int)OpenGloveActions.DisconnectFromBluetoothDevice:
                        TryDisconnectFromBluetoothDevice(socket, deviceName);
                        break;

                    case (int)OpenGloveActions.StartCaptureDataFromServer:
                        StartCaptureDataFromServer(socket, deviceName);
                        break;

                    case (int)OpenGloveActions.StopCaptureDataFromServer:
                        StopCaptureDataFromServer(deviceName);
                        break;

                    case (int)OpenGloveActions.AddActuator:
                        Region = regions.Split(',').Select(int.Parse).ToList()[0];
                        Pin = values.Split(',').Select(int.Parse).ToList()[0];
                        ExtraPin = Pin = values.Split(',').Select(int.Parse).ToList()[0];
                        OpenGloveByDeviceName[deviceName].AddActuator(Region, Pin, ExtraPin);
                        break;

                    case (int)OpenGloveActions.AddActuators:
                        Regions = regions.Split(',').Select(int.Parse).ToList();
                        Pins = values.Split(',').Select(int.Parse).ToList();
                        ExtraPins = extraValues.Split(',').Select(int.Parse).ToList();
                        OpenGloveByDeviceName[deviceName].AddActuators(Regions, Pins, ExtraPins);
                        break;

                    case (int)OpenGloveActions.RemoveActuator:
                        Region = regions.Split(',').Select(int.Parse).ToList()[0];
                        OpenGloveByDeviceName[deviceName].RemoveActuator(Region);
                        break;

                    case (int)OpenGloveActions.RemoveActuators:
                        Regions = regions.Split(',').Select(int.Parse).ToList();
                        OpenGloveByDeviceName[deviceName].RemoveActuators(Regions);
                        break;

                    case (int)OpenGloveActions.ActivateActuators:
                        Regions = regions.Split(',').Select(int.Parse).ToList();
                        Intensities = values.Split(',').ToList();
                        OpenGloveByDeviceName[deviceName].ActivateActuators(Regions, Intensities);
                        break;

                    case (int)OpenGloveActions.ActivateActuatorsTimeTest:
                        //Send elapsed time on OpenGlove Configuration Application
                        stopwatch = new Stopwatch();
                        stopwatch.Start();

                        Regions = regions.Split(',').Select(int.Parse).ToList();
                        Intensities = values.Split(',').ToList();
                        OpenGloveByDeviceName[deviceName].ActivateActuatorsTimeTest(Regions, Intensities);

                        stopwatch.Stop();
                        timeSpan = stopwatch.Elapsed;
                        webSocketByDeviceName[deviceName].Send("ns," + timeSpan.Ticks * 100); // nanoseconds https://msdn.microsoft.com/en-us/library/system.datetime.ticks(v=vs.110).aspx
                        break;

                    case (int)OpenGloveActions.TurnOnActuators:
                        OpenGloveByDeviceName[deviceName].TurnOnActuators();
                        break;

                    case (int)OpenGloveActions.TurnOffActuators:
                        OpenGloveByDeviceName[deviceName].TurnOffActuators();
                        break;

                    case (int)OpenGloveActions.ResetActuators:
                        OpenGloveByDeviceName[deviceName].ResetActuators();
                        break;

                    case (int)OpenGloveActions.AddFlexor:
                        Region = Int32.Parse(regions);
                        Pin = Int32.Parse(values);
                        OpenGloveByDeviceName[deviceName].AddFlexor(Region, Pin);
                        break;

                    case (int)OpenGloveActions.AddFlexors:
                        Regions = regions.Split(',').Select(int.Parse).ToList();
                        Pins = values.Split(',').Select(int.Parse).ToList();
                        OpenGloveByDeviceName[deviceName].AddFlexors(Regions, Pins);
                        break;

                    case (int)OpenGloveActions.RemoveFlexor:
                        Region = Int32.Parse(regions);
                        OpenGloveByDeviceName[deviceName].RemoveFlexor(Region);
                        break;

                    case (int)OpenGloveActions.RemoveFlexors:
                        Regions = regions.Split(',').Select(int.Parse).ToList();
                        OpenGloveByDeviceName[deviceName].RemoveFlexors(Regions);
                        break;

                    case (int)OpenGloveActions.CalibrateFlexors:
                        OpenGloveByDeviceName[deviceName].CalibrateFlexors();
                        break;

                    case (int)OpenGloveActions.ConfirmCalibration:
                        OpenGloveByDeviceName[deviceName].ConfirmCalibration();
                        break;

                    case (int)OpenGloveActions.SetThreshold:
                        Value = values.Split(',')[0];
                        OpenGloveByDeviceName[deviceName].SetThreshold(Int32.Parse(Value));
                        break;

                    case (int)OpenGloveActions.TurnOnFlexors:
                        OpenGloveByDeviceName[deviceName].TurnOnFlexors();
                        break;

                    case (int)OpenGloveActions.TurnOffFlexors:
                        OpenGloveByDeviceName[deviceName].TurnOffFlexors();
                        break;

                    case (int)OpenGloveActions.ResetFlexors:
                        OpenGloveByDeviceName[deviceName].ResetFlexors();
                        break;

                    case (int)OpenGloveActions.StartIMU:
                        OpenGloveByDeviceName[deviceName].StartIMU();
                        break;

                    case (int)OpenGloveActions.SetIMUStatus:
                        Value = values.Split(',')[0];
                        OpenGloveByDeviceName[deviceName].SetIMUStatus(bool.Parse(Value));
                        break;

                    case (int)OpenGloveActions.SetRawData:
                        Value = values.Split(',')[0];
                        OpenGloveByDeviceName[deviceName].SetRawData(bool.Parse(Value));
                        break;

                    case (int)OpenGloveActions.SetIMUChoosingData:
                        IntegerValue = values.Split(',').Select(int.Parse).ToList()[0];
                        OpenGloveByDeviceName[deviceName].SetIMUChoosingData(IntegerValue);
                        break;

                    case (int)OpenGloveActions.CalibrateIMU:
                        socket.Send("[OpenGloveServer] CalibrateIMU: Not Implemented");
                        break;

                    case (int)OpenGloveActions.TurnOnIMU:
                        OpenGloveByDeviceName[deviceName].TurnOnIMU();
                        break;

                    case (int)OpenGloveActions.TurnOffIMU:
                        OpenGloveByDeviceName[deviceName].TurnOffIMU();
                        break;

                    case (int)OpenGloveActions.SetLoopDelay:
                        Value = values.Split(',')[0];
                        OpenGloveByDeviceName[deviceName].SetLoopDelay(Int32.Parse(Value));
                        break;

                    case (int)OpenGloveActions.GetOpenGloveArduinoSoftwareVersion:
                        OpenGloveByDeviceName[deviceName].GetOpenGloveArduinoSofwareVersion();
                        break;
                    default:
                        socket.Send("You said: " + message); // test echo message
                        break;
                }
            }
            catch
            {
                Debug.WriteLine("WebSocketServer ERROR: BAD FORMAT OR TYPE DATA in OnWebSocketMessageReceived");
            }
        }

        // Method for subcribe to messages from OpenGlove Devices
        public static void OnBluetoothMessage(object source, BluetoothEventArgs e)
        {
            SendDataIfWebSocketRequestedDataFromDevice(e);
        }

        public static void SendDataIfWebSocketRequestedDataFromDevice(BluetoothEventArgs e)
        {
            if (e.Message != null)
            {
                Debug.WriteLine(e.Message);
                if (webSocketByDeviceName.ContainsKey(e.DeviceName))
                    webSocketByDeviceName[e.DeviceName].Send(e.Message);
            }
        }

        public bool AddOpenGloveDeviceToServer(string deviceName, string configurationName)
        {
            if (OpenGloveByDeviceName !=null )
            {
                if (OpenGloveByDeviceName.ContainsKey(deviceName))
                {
                    LoadConfigurationToOpenGloveInstance(deviceName, configurationName);
                }
                else
                {
                    OpenGlove openGlove = new OpenGlove(deviceName);
                    OpenGloveByDeviceName.Add(openGlove.BluetoothDeviceName, openGlove);
                    LoadConfigurationToOpenGloveInstance(deviceName, configurationName);
                }

                return true;
            }
            return false;
        }

        public bool RemoveOpenGloveDeviceFromServer(string deviceName)
        {
            if (OpenGloveByDeviceName.ContainsKey(deviceName))
            {
                OpenGloveByDeviceName[deviceName].TurnOffActuators();
                OpenGloveByDeviceName[deviceName].TurnOffFlexors();
                OpenGloveByDeviceName[deviceName].TurnOffIMU();
                OpenGloveByDeviceName[deviceName].CloseDeviceConnection(); //TODO await this mehod for close connection
                OpenGloveByDeviceName.Remove(deviceName);
                return true;
            }
            return false;
        }

        public void LoadConfigurationToOpenGloveInstance(string deviceName, string configurationName)
        {
            if (configurationName != null && deviceName != null)
            {
                if (ConfigurationByName.ContainsKey(configurationName))
                    OpenGloveByDeviceName[deviceName].Configuration = ConfigurationByName[configurationName];
                else
                {
                    OpenGloveConfiguration configuration = new OpenGloveConfiguration(configurationName);
                    OpenGloveByDeviceName[deviceName].Configuration = configuration;
                    ConfigurationByName.Add(configurationName, configuration);
                }
            }
        }

        public void InitializeOpenGloveConfigurationOnDevice(string deviceName)
        {
            if(deviceName != null)
                if(OpenGloveByDeviceName.ContainsKey(deviceName))
                    OpenGloveByDeviceName[deviceName].InitializeOpenGloveConfigurationOnDevice(); //equals to .InitializeActuators, .InitializeFlexors and .InitializeIMU
        }

        public void SaveOpenGloveConfiguration(string deviceName, string configurationName)
        {
            if (configurationName != null)
            {
                if (ConfigurationByName.ContainsKey(configurationName))
                    ConfigurationByName.Remove(configurationName);
                ConfigurationByName.Add(configurationName, OpenGloveByDeviceName[deviceName].Configuration);
            }
        }

        public bool TryConnectToBluetoothDevice(IWebSocketConnection socket, string deviceName)
        {
            try
            {
                if (OpenGloveByDeviceName.ContainsKey(deviceName))
                {
                    OpenGloveByDeviceName[deviceName].OpenDeviceConnection();
                    return true;
                }
                else
                    socket.Send($"[OpenGloveServer] Connect Error: add this new OpenGloveDevice first");
                return false;
            }
            catch
            {
                socket.Send($"[OpenGloveServer] Connect Error: turn On Bluetooth or dont exist bounded device name");
                return false;
            }
        }

        public bool TryDisconnectFromBluetoothDevice( IWebSocketConnection socket, string deviceName)
        {
            try
            {
                if (OpenGloveByDeviceName.ContainsKey(deviceName))
                {
                    OpenGloveByDeviceName[deviceName].TurnOffActuators(); //TODO await turn off before CloseConnection
                    OpenGloveByDeviceName[deviceName].CloseDeviceConnection();
                    return true;
                }
                else
                {
                    socket.Send($"[OpenGloveServer] Disconnect Error: add this new OpenGloveDevice first");
                    return false;
                }
            }
            catch
            {
                socket.Send($"[OpenGloveServer] Disconnect Error: Turn On Bluetooth or dont exist bounded device name");
                return false;
            }
        }

        public void StartCaptureDataFromServer(IWebSocketConnection socket, string deviceName)
        {
            if(!webSocketByDeviceName.ContainsKey(deviceName))
                webSocketByDeviceName.Add(deviceName, socket);
        }

        public void StopCaptureDataFromServer(string deviceName)
        {
            webSocketByDeviceName.Remove(deviceName);
        }
    }
}
