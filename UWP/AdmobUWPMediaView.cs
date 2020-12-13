using Windows.UI.Xaml.Controls;

namespace Zebble.AdMob
{
    class AdmobUWPMediaView : Panel
    {
        AdmobMediaView View;
        Image NativeImage;

        public AdmobUWPMediaView(AdmobMediaView view)
        {
            View = view;
            NativeImage = new Image();
            Children.Add(NativeImage);
        }

        // TODO: Render the image or video
    }
}
