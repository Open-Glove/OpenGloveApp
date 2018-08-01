using System;
using OpenGloveApp.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace OpenGloveApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            /*
            NavigationPage navigationPage = new NavigationPage(new Home())
            {
                BarTextColor = Color.White,
                BarBackgroundColor = Color.FromHex(AppConstants.Colors.ColorPrimary),
            };

            Current.Resources["primary_colour"] = AppConstants.Colors.ColorPrimary;
            Current.Resources["primary_dark"] = AppConstants.Colors.ColorPrimaryDark;
            Current.Resources["color_accent"] = AppConstants.Colors.ColorAccent;
            Current.Resources["color_text"] = AppConstants.Colors.ColorText;
            */

            MainPage = new OpenGloveAppPage(); //navigationPage;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

    }
}
