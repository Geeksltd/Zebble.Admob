using Ads = Android.Gms.Ads;
using System.Threading.Tasks;

namespace Zebble.AdMob
{
    public partial class InterstitialAd
    {
        Ads.InterstitialAd interstitialAd;

        public Task LoadAd()
        {
            return Thread.UI.Run(() =>
            {
                if (string.IsNullOrEmpty(UnitId))
                {
                    OnAdFailed.Raise("The UnitId of the Interstitial ad has not specified!");
                    return;
                }

                interstitialAd = new Ads.InterstitialAd(UIRuntime.CurrentActivity);
                interstitialAd.AdUnitId = UnitId;
                interstitialAd.AdListener = new AndroidAdListener(this);
                interstitialAd.LoadAd(new Ads.AdRequest.Builder().Build());
            });
        }

        public Task ShowAd()
        {
            return Thread.UI.Run(() =>
            {
                if (interstitialAd.IsLoaded)
                    interstitialAd.Show();
                else
                    OnAdFailed.Raise("The ad has not loaded yet! please use the Show method just after the ad loaded completely");
            });
        }

        internal class AndroidAdListener : Ads.AdListener
        {
            InterstitialAd Ad;

            public AndroidAdListener(InterstitialAd ad)
            {
                Ad = ad;
            }

            public override void OnAdClicked() => Ad.OnAdTapped.Raise();

            public override void OnAdClosed() => Ad.OnAdClosed.Raise();

            public override void OnAdLoaded() => Ad.OnAdLoaded.Raise();

            public override void OnAdFailedToLoad(int p0)
            {
                string error;
                AdmobAndroidListener.OnAdError(p0, out error);
                Ad.OnAdFailed.Raise(error);
            }

            public override void OnAdOpened() => Ad.OnAdOpened.Raise();
        }
    }
}