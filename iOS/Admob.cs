using Google.MobileAds;

namespace Zebble
{
    public partial class Admob
    {
        public static void InitilizeAd()
        {
            MobileAds.SharedInstance.Start(null);
        }
    }
}