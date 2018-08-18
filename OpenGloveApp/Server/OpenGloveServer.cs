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
    public class OpenGloveServer: Server
    {
        private string url;
        private WebSocketServer server; // sample "ws://127.0.0.1:7070"
        private List<IWebSocketConnection> allSockets = new List<IWebSocketConnection>();
        private static Dictionary<string, IWebSocketConnection> webSocketByDeviceName = new Dictionary<string, IWebSocketConnection>();
        public static Dictionary<string, OpenGlove> OpenGloveByDeviceName = new Dictionary<string, OpenGlove>();
        public static Dictionary<string, OpenGloveConfiguration> ConfigurationByDeviceName = new Dictionary<string, OpenGloveConfiguration>();


        public OpenGloveServer(string url)
        {
            this.url = url;
        }

        public void StartWebsockerServer()
        {
            FleckLog.Level = LogLevel.Debug;
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Debug.WriteLine("Open WebSocket!");
                    allSockets.Add(socket);
                };
                socket.OnClose = () =>
                { 
                    Debug.WriteLine("Close WebSocket!");
                    try
                    {
                        if (webSocketByDeviceName.ContainsValue(socket) || allSockets.Contains(socket))
                        {
                            var item = webSocketByDeviceName.First(entry => entry.Value == socket);
                            webSocketByDeviceName.Remove(item.Key);
                            allSockets.Remove(socket);
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
            allSockets.ToList().ForEach(s => s.Close());
        }

        override public void Start()
        {
            this.server = new WebSocketServer(url);
            ConfigureServer();
            StartWebsockerServer();
        }

        override public void Stop()
        {
            CloseAllSockets();
            server.Dispose(); //this method does not allow more incoming connections, and disconnect the server. So it is necessary to disconnect all sockets first to cut off all communication
            allSockets.Clear();
        }

        override public void ConfigureServer()
        {
            server.RestartAfterListenError = true;
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
                    case (int)OpenGloveActions.AddOpenGloveDevice:
                        AddOpenGloveDevice(deviceName);
                        break;

                    case (int)OpenGloveActions.RemoveOpenGloveDevice:
                        RemoveOpenGloveDevice(deviceName);
                        break;

                    case (int)OpenGloveActions.SaveOpenGloveDevice:
                        socket.Send($"[OpenGloveServer] SaveOpenGloveDevice: not implemented");
                        break;

                    case (int)OpenGloveActions.ConnectToBluetoothDevice:
                        TryConnectToBluetoothDevice(socket, deviceName);
                        break;

                    case (int)OpenGloveActions.DisconnectFromBluetoothDevice:
                        TryDisconnectFromBluetoothDevice(socket, deviceName);
                        break;

                    case (int)OpenGloveActions.StartCaptureDataFromServer:
                        webSocketByDeviceName.Add(deviceName, socket);
                        break;

                    case (int)OpenGloveActions.StopCaptureDataFromServer:
                        webSocketByDeviceName.Remove(deviceName);
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

                    case (int)OpenGloveActions.SetLoopDelay:
                        Value = values.Split(',')[0];
                        OpenGloveByDeviceName[deviceName].SetLoopDelay(Int32.Parse(Value));
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
                if (webSocketByDeviceName.ContainsKey(e.DeviceName))
                    webSocketByDeviceName[e.DeviceName].Send(e.Message);
            }
        }

        public bool AddOpenGloveDevice(string deviceName)
        {
            // TODO verify if exist a configuration for this bluetoothDeviceName
            if (!OpenGloveByDeviceName.ContainsKey(deviceName))
            {
                OpenGlove openGlove = new OpenGlove(deviceName);
                OpenGloveByDeviceName.Add(openGlove.BluetoothDeviceName, openGlove);
                return true;
            }
            return false;
        }

        public bool RemoveOpenGloveDevice(string deviceName)
        {
            if (OpenGloveByDeviceName.ContainsKey(deviceName))
            {
                OpenGloveByDeviceName[deviceName].TurnOffActuators();
                OpenGloveByDeviceName[deviceName].TurnOffFlexors();
                OpenGloveByDeviceName[deviceName].CloseDeviceConnection(); //TODO await this mehod for close connection
                OpenGloveByDeviceName.Remove(deviceName);
                return true;
            }
            return false;
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

    }
}
