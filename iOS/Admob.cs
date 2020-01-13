using Google.MobileAds;

namespace Zebble.AdMob
{
    public partial class Admob
    {
        public static void InitilizeAd()
        {
            MobileAds.SharedInstance.Start(null);
        }
    }
}