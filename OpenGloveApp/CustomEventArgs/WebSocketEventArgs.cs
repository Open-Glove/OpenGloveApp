using System;
using System.Collections.Generic;

namespace OpenGloveApp.CustomEventArgs
{
    public class WebSocketEventArgs: EventArgs
    {
        public int What { get; set; }
        public string DeviceName { get; set; }
        public int Region { get; set; }
        public List<int> Regions { get; set; }
        public List<int> Intensities { get; set; }
        public int Pin { get; set; }
        public List<int> Pins { get; set; }
        public List<string> Values { get; set; }
        public string Message { get; set; }
    }
}
