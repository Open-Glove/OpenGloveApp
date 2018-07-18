using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Fleck;

namespace OpenGloveApp.Server
{
    public class OpenGloveServer
    {
        private string mUrl;
        private WebSocketServer mServer; // sample "ws://127.0.0.1:7070"
        private List<IWebSocketConnection> allSockets = new List<IWebSocketConnection>();
        private static Dictionary<string, IWebSocketConnection> WebSocketByDeviceName = new Dictionary<string, IWebSocketConnection>();

        public OpenGloveServer(string url)
        {
            this.mUrl = url;
        }

        public void ConfigureServer()
        {
            mServer.RestartAfterListenError = true;
        }

        public void StartWebsockerServer()
        {
            FleckLog.Level = LogLevel.Debug;
            mServer.Start(socket =>
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
                };
                socket.OnMessage = message =>
                {
                    HandleMessage(socket, message);
                };
            });
        }

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
                            WebSocketByDeviceName.Add(words[1], socket);
                            break;
                        case "StopReadDataFrom":
                            WebSocketByDeviceName.Remove(words[1]);
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
                    Debug.WriteLine("ERROR: BAD FORMAT");
                }
            }

        }

        public void CloseAllSockets()
        {
            allSockets.ToList().ForEach(s => s.Close());
        }

        public void Start()
        {
            this.mServer = new WebSocketServer(mUrl);
            ConfigureServer();
            StartWebsockerServer();
        }

        public void Stop()
        {
            CloseAllSockets();
            mServer.Dispose(); //this method does not allow more incoming connections, and disconnect the server. So it is necessary to disconnect all sockets first to cut off all communication
            allSockets.Clear();
        }


        //Event Handler subcribe to Data from OpenGloves Devices
        public static void OnBluetoothMessage(object source, BluetoothEventArgs e)
        {
            SendDataIfWebSocketRequestedDataFromDevice(e);
        }

        public static void SendDataIfWebSocketRequestedDataFromDevice(BluetoothEventArgs e){
            if (e.Message != null)
            {
                Debug.WriteLine($" [{e.DeviceName}] OpenGloveServer.OnBluetoothMessage: {e.Message}");

                if (WebSocketByDeviceName.ContainsKey(e.DeviceName))
                    WebSocketByDeviceName[e.DeviceName].Send(e.Message);
            }
        }
    }
}
