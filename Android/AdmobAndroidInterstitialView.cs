using Android.Gms.Ads;

namespace Zebble.AdMob
{
    class AdmobAndroidInterstitialView
    {
        public AdmobInterstitialView View { get; set; }
        InterstitialAd interstitialAd;

        public AdmobAndroidInterstitialView(AdmobInterstitialView view)
        {
            View = view;

            View.OnShow.Handle(() =>
            {
                if (interstitialAd.IsLoaded)
                    interstitialAd.Show();
                else
                    View.OnAdFailed.Raise("The ad has not loaded yet! please use the Show method just after the ad loaded completely");
            });

            LoadAd();
        }

        void LoadAd()
        {
            interstitialAd = new InterstitialAd(Renderer.Context);
            interstitialAd.AdUnitId = View.UnitId;
            interstitialAd.AdListener = new AndroidAdListener(View);
            interstitialAd.LoadAd(new AdRequest.Builder().Build());
        }

        class AndroidAdListener : AdListener
        {
            AdmobInterstitialView View;

            public AndroidAdListener(AdmobInterstitialView view)
            {
                View = view;
            }

            public override void OnAdClicked() => View.OnAdTapped.Raise();

            public override void OnAdClosed() => View.OnAdClosed.Raise();

            public override void OnAdLoaded() => View.OnAdLoaded.Raise();

            public override void OnAdFailedToLoad(int p0)
            {
                string error;
                AdmobAndroidListener.OnAdError(p0, out error);
                View.OnAdFailed.Raise(error);
            }

            public override void OnAdOpened() => View.OnAdOpened.Raise();
        }
    }
}