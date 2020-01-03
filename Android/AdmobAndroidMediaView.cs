using Android.Gms.Ads.Formats;

namespace Zebble
{
    class AdmobAndroidMediaView : MediaView
    {
        AdmobMediaView View;

        public AdmobAndroidMediaView(AdmobMediaView view) : base(Renderer.Context)
        {
            View = view;
        }
    }
}