using Microsoft.Advertising.WinRT.UI;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

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

        public void SetImage(NativeImage image)
        {
            var bitmapImage = new BitmapImage
            {
                UriSource = new Uri(image.Url)
            };

            NativeImage.Source = bitmapImage;

            NativeImage.Width = image.Width;
            NativeImage.Height = image.Height;
        }
    }
}
