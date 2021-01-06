using Android.Gms.Ads;

namespace Zebble.AdMob
{
    public partial class Admob
    {
        public static void InitializeAd() => MobileAds.Initialize(UIRuntime.CurrentActivity);
    }
}