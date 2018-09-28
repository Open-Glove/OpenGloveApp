using BottomBar.XamarinForms;
using OpenGloveApp.Models;
using OpenGloveApp.OpenGloveAPI;
using Xamarin.Forms;
using static OpenGloveApp.AppConstants.AppConstants;

namespace OpenGloveApp.Pages
{
   
    public partial class Home : BottomBarPage
    {
        // For manage the current OpenGlove Bluetooth Device Configuration
        public static OpenGloveDevice OpenGloveDevice = new OpenGloveDevice("ForConfigurationApplicationUse");

        public Home()
        {
            InitializeComponent();
            switch(DeviceRuntime)
            {
                case Device_Android:
                    this.BarTextColor = Color.FromHex(AppConstants.Colors.ColorPrimary);
                    break;
                default:
                    break;
            }
        }

        protected override void OnCurrentPageChanged()
        {
            base.OnCurrentPageChanged();
            var pages = Children.GetEnumerator();
            this.Title = CurrentPage.Title;
        }
    }
}
