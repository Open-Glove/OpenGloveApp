using System;

namespace OpenGloveApp.CustomEventArgs
{
    public class WebSocketEventArgs: EventArgs
    {
        public string DeviceName { get; set; }
        public int What { get; set; }
        public string Message { get; set; }
    }
}
