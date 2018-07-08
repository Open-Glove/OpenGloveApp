using System.Diagnostics;
using Fleck;

namespace OpenGloveApp.Server
{
    public class OpenGloveServer
    {
        private string mUrl;
        private WebSocketServer mServer; // sample "ws://127.0.0.1:8181"


        public OpenGloveServer(string url)
        {
            this.mUrl = url;
            this.mServer = new WebSocketServer(url);
        }

        public void ConfigureServer()
        {
            mServer.RestartAfterListenError = true;
        }

        public void StartWebsockerServer()
        {
            mServer.Start(socket =>
            {
                socket.OnOpen = () => Debug.WriteLine("Open!");
                socket.OnClose = () => Debug.WriteLine("Close!");
                socket.OnMessage = message => socket.Send(message);
            }); 
        }

        public void Start()
        {
            ConfigureServer();
            StartWebsockerServer();
        }

        public void Stop()
        {
            mServer.Dispose(); //TODO comprobar
        }
    }
}
