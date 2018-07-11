using BottomBar.XamarinForms;
using Xamarin.Forms;
using static OpenGloveApp.AppConstants.AppConstants;

namespace OpenGloveApp.Pages
{
   
    public partial class Home : BottomBarPage
    {
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
    }
}
