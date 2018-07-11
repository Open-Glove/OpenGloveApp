using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace OpenGloveApp.Pages
{
    public partial class Device : ContentPage
    {
        public Device()
        {
            InitializeComponent();
        }

        public static int RuntimePlatform { get; internal set; }
    }
}
