namespace Zebble.AdMob
{
    public partial class Admob
    {
        internal static string ApplicationCode;

        public static void InitilizeAd(string applicationCode)
        {
            ApplicationCode = applicationCode;
        }
    }
}