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

            switch(message){
                case "hello":
                        socket.Send("world!");
                        break;
                default:
                    socket.Send(message);
                    break;
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
            mServer.Dispose(); //this method does not allow more incoming connections, so it is necessary to disconnect all sockets first to cut off all communication
            allSockets.Clear();
        }
    }
}
