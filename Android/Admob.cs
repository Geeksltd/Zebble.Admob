using Android.Gms.Ads;

namespace Zebble.AdMob
{
    public partial class Admob
    {
        public static void InitilizeAd(string applicationCode = null)
        {
            if (applicationCode == null)
                MobileAds.Initialize(UIRuntime.CurrentActivity);
            else
                MobileAds.Initialize(UIRuntime.CurrentActivity, applicationCode);
        }
    }
}