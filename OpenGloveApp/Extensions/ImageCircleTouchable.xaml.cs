using System;
using System.Threading.Tasks;
using OpenGloveApp.Pages;
using Xamarin.Forms;

namespace OpenGloveApp.Extensions
{
    public partial class ImageCircleTouchable : ContentView
    {
        // Event for send data to UI thread on Main Xamarin.Forms project
        public event EventHandler<EventArgs> MenuItemClicked;

        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create("Source", typeof(string), typeof(ImageCircleTouchable));
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create("Text", typeof(string), typeof(ImageCircleTouchable));

        public string Source 
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public ImageCircleTouchable(ContentPage contentPage)
        {
            InitializeComponent();
            BindingContext = this;

            var tgr = new TapGestureRecognizer();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            tgr.Tapped += (s, e) => OnImageClicked(s, e);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            this.GestureRecognizers.Add(tgr);

            //subscribe ConfigurationPage to CircleImageTouchableItems
            this.MenuItemClicked += ((ConfigurationPage)contentPage).OnMenuItemClicked;
        }

        public async Task OnImageClicked(object source, System.EventArgs e)
        {
            uint ms = 40;
            await ((ImageCircleTouchable)source).ScaleTo(0.9, ms/2);
            await ((ImageCircleTouchable)source).ScaleTo(1, ms/2);
            this.OnMenuItemClicked();
        }

        // Method for raise the event
        protected virtual void OnMenuItemClicked()
        {
            if (MenuItemClicked != null)
            {
                MenuItemClicked(this, new EventArgs());
            }
        }
    }
}
