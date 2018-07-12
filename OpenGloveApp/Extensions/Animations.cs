using System.Threading.Tasks;
using Xamarin.Forms;

namespace OpenGloveApp.Extensions
{
    public static class Animations
    {

        public async static Task AnimationTouch(View view)
        {
            uint ms = 40;
            await view.ScaleTo(0.9, ms / 2);
            await view.ScaleTo(1, ms / 2);
        } 
    }
}
