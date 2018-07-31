using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Fleck;
using OpenGloveApp.CustomEventArgs;
using OpenGloveApp.AppConstants;

namespace OpenGloveApp.Server
{
    public class OpenGloveServer
    {
        private string url;
        private WebSocketServer server; // sample "ws://127.0.0.1:7070"
        private List<IWebSocketConnection> allSockets = new List<IWebSocketConnection>();
        private static Dictionary<string, IWebSocketConnection> webSocketByDeviceName = new Dictionary<string, IWebSocketConnection>();
        public event EventHandler<WebSocketEventArgs> WebSocketDataReceived;


        public OpenGloveServer(string url)
        {
            this.url = url;
        }

        public void ConfigureServer()
        {
            server.RestartAfterListenError = true;
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
                    allSockets.Remove(socket);
                    webSocketByDeviceName.ContainsValue(socket);
                };
                socket.OnMessage = message =>
                {
                    HandleMessage(socket, message);
                };
            });
        }


        /* WebSocket format message:  Action;DeviceName;Intensity;Regions
         * sample: Activate;OpenGloveIZQ;255;0,2,6,7,29
         * Action:      one of this Actions : Activate, StartCaptureData, StopCaptureData,
         * DeviceName:  Name of bluetooth device to send the command
         * Intensity:   0 to 255
         * Regions:     a list of regions to activate (int)
        */
        // Handle Message from WebSocket Client
        public void HandleMessage(IWebSocketConnection socket, string message)
        {
            Debug.WriteLine(message);
            string[] words;

            if(message != null)
            {
                try
                {
                    words = message.Split(',');
                    switch (words[0])
                    {
                        case "ReadDataFrom":
                            webSocketByDeviceName.Add(words[1], socket);
                            break;
                        case "StopReadDataFrom":
                            webSocketByDeviceName.Remove(words[1]);
                            break;
                        case "Activate":
                            //OnWebSocketDataReceived(OpenGloveActions.ACTIVATE_MOTORS, words[1], );
                            break;
                        case "hello":
                            socket.Send("world!");
                            break;
                        default:
                            socket.Send(message);
                            break;
                    }
                }
                catch
                {
                    Debug.WriteLine("ERROR: BAD FORMAT, HandleMessage from WebSocketClient");
                }
            }

        }

        // Event Handler subcribe to Data from OpenGloves Devices
        public static void OnBluetoothMessage(object source, BluetoothEventArgs e)
        {
            SendDataIfWebSocketRequestedDataFromDevice(e);
        }

        public static void SendDataIfWebSocketRequestedDataFromDevice(BluetoothEventArgs e)
        {
            if (e.Message != null)
            {
                Debug.WriteLine($" [{e.DeviceName}] OpenGloveServer.OnBluetoothMessage: {e.Message}");

                if (webSocketByDeviceName.ContainsKey(e.DeviceName))
                    webSocketByDeviceName[e.DeviceName].Send(e.Message);
            }
        }

        // Method for raise event: 

        protected virtual void OnWebSocketDataReceived(int what, string deviceName, string message)
        {
            if (WebSocketDataReceived != null)
                WebSocketDataReceived(this, new WebSocketEventArgs()
                { What = what, DeviceName = deviceName, Message = message });
        }


        public void CloseAllSockets()
        {
            allSockets.ToList().ForEach(s => s.Close());
        }

        public void Start()
        {
            this.server = new WebSocketServer(url);
            ConfigureServer();
            StartWebsockerServer();
        }

        public void Stop()
        {
            CloseAllSockets();
            server.Dispose(); //this method does not allow more incoming connections, and disconnect the server. So it is necessary to disconnect all sockets first to cut off all communication
            allSockets.Clear();
        }
    }
}
