using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Fleck;
using OpenGloveApp.AppConstants;
using OpenGloveApp.CustomEventArgs;

namespace OpenGloveApp.Server
{
    public class OpenGloveServer: Server
    {
        public static event EventHandler<WebSocketEventArgs> WebSocketMessageReceived;

        private string url;
        private WebSocketServer server; // sample "ws://127.0.0.1:7070"
        private List<IWebSocketConnection> allSockets = new List<IWebSocketConnection>();
        private static Dictionary<string, IWebSocketConnection> webSocketByDeviceName = new Dictionary<string, IWebSocketConnection>();

        //public static List<OpenGlove>


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
        private void HandleMessage(IWebSocketConnection socket, string message)
        {
            Debug.WriteLine(message);
            string[] words;

            if(message != null)
            {
                try
                {
                    words = message.Split(';');
                    int action = Int32.Parse(words[0]);
                    switch (action)
                    {
                        case (int) OpenGloveActions.StartCaptureData:
                            webSocketByDeviceName.Add(words[1], socket);
                            break;
                        case (int) OpenGloveActions.StopCaptureData:
                            webSocketByDeviceName.Remove(words[1]);
                            break;
                        case (int) OpenGloveActions.ActivateActuators:
                            // Transform string to list of regions and intensities
                            List<int> regions = words[2].Split(',').Select(int.Parse).ToList();
                            List<string> instensities = words[3].Split(',').ToList();
                            //List<Actuator> actuators = 

                            //OnWebSocketDataReceived(OpenGloveActions.ActivateActuators, words[1], );
                            break;
                        default:
                            socket.Send("You said: " + message); // test echo message
                            break;
                    }
                }
                catch
                {
                    Debug.WriteLine("ERROR: BAD FORMAT, HandleMessage from WebSocketClient");
                }
            }

        }

        // Method for raise event to subcribers (aka. Connected Thread on Communication implementation iOS/Android of OpenGlove devices)
        protected virtual void OnWebSocketMessageReceived(int what, string deviceName, List<int> regions, List<int> intensities, string message)
        {
            WebSocketMessageReceived?.Invoke(this, new WebSocketEventArgs()
            { What = what, DeviceName = deviceName, Message = message });
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
                //Debug.WriteLine($" [{e.DeviceName}] OpenGloveServer.OnBluetoothMessage: {e.Message}");
                if (webSocketByDeviceName.ContainsKey(e.DeviceName))
                    webSocketByDeviceName[e.DeviceName].Send(e.Message);
            }
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

    }
}
