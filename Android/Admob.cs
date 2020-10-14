using Android.Gms.Ads;

namespace Zebble.AdMob
{
    public partial class Admob
    {
        public static void InitilizeAd() => MobileAds.Initialize(UIRuntime.CurrentActivity);
    }
}