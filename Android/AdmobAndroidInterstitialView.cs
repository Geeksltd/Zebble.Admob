using Android.Gms.Ads;
using Android.Widget;

namespace Zebble
{
    class AdmobAndroidInterstitialView : FrameLayout, IZebbleAdNativeView<AdmobInterstitialView>
    {
        public AdmobInterstitialView View { get; set; }
        InterstitialAd interstitialAd;

        public AdmobAndroidInterstitialView(AdmobInterstitialView view) : base(Renderer.Context)
        {
            View = view;

            view.OnAdButtonTapped.Handle(() => { if (interstitialAd.IsLoaded) interstitialAd.Show(); });

            LoadAd();
        }

        void LoadAd()
        {
            interstitialAd = new InterstitialAd(Renderer.Context);
            interstitialAd.AdUnitId = View.UnitId;
            interstitialAd.AdListener = new AndroidAdListener(this);
            interstitialAd.LoadAd(new AdRequest.Builder().Build());
        }

        class AndroidAdListener : AdmobAndroidListener<AdmobInterstitialView>
        {
            AdmobAndroidInterstitialView NativeView;

            public AndroidAdListener(AdmobAndroidInterstitialView nativeView) : base(nativeView)
            {
                NativeView = nativeView;
            }

            public override void OnAdClosed()
            {
                NativeView.interstitialAd.LoadAd(new AdRequest.Builder().Build());
                base.OnAdClosed();
            }
        }
    }
}