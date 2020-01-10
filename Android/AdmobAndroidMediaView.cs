using Android.Gms.Ads.Formats;
using Android.Views;

namespace Zebble.AdMob
{
    class AdmobAndroidMediaView : MediaView
    {
        AdmobMediaView View;

        public AdmobAndroidMediaView(AdmobMediaView view) : base(Renderer.Context)
        {
            View = view;

            LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
        }
    }
}