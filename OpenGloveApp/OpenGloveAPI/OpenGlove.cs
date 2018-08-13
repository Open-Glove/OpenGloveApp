using OpenGloveApp.Models;

namespace OpenGloveApp.OpenGloveAPI
{
    public class OpenGlove : LegacyOpenGlove
    {
        public string Name { get; set; }
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
    }
}
