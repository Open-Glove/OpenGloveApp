using OpenGloveApp.Models;

namespace OpenGloveApp.OpenGloveAPI
{
    public class OpenGlove : LegacyOpenGlove
    {
        public string BluetoothDeviceName { get; set; }
        public OpenGloveConfiguration Configuration { get; set; }

        public OpenGlove()
            : base ()
        {
        }

        public OpenGlove(OpenGloveConfiguration configuration)
            : base()
        {
            this.Configuration = configuration;
        }

        public OpenGlove(string bluetoothDeviceName, OpenGloveConfiguration configuration)
            : base()    
        {
            this.BluetoothDeviceName = bluetoothDeviceName;
            this.Configuration = configuration;
        }
    }
}
