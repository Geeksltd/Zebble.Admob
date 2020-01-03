using System;
using Google.MobileAds;
using UIKit;

namespace Zebble
{
    class AdmobIOSBannerView : UIView, IZebbleAdNativeView<AdmobBannerView>
    {
        public AdmobBannerView View { get; set; }
        BannerView NativeView;

        public AdmobIOSBannerView(AdmobBannerView view)
        {
            View = view;

            LoadAd();
        }

        void LoadAd()
        {
            NativeView = new BannerView();
            NativeView.Frame = View.GetFrame();

            NativeView.AdUnitId = View.UnitId;
            NativeView.RootViewController = UIRuntime.NativeRootScreen as UIViewController;

            if (View.BannerSize.Width != 0) NativeView.AdSize = AdSizeCons.GetFromCGSize(new CoreGraphics.CGSize(View.BannerSize.Width, View.BannerSize.Height));

            NativeView.AdSize = AdSizeCons.Banner;

            NativeView.LoadRequest(Request.GetDefaultRequest());
            NativeView.Delegate = new IOSBannerAdListener(this);

            Add(NativeView);
        }

        class IOSBannerAdListener : AdmobIOSListener<AdmobBannerView>, IBannerViewDelegate
        {
            AdmobIOSBannerView NativeView;

            public IOSBannerAdListener(AdmobIOSBannerView nativeView) : base(nativeView)
            {
                NativeView = nativeView;
            }

            public void DidReceiveAd(BannerView adView) => NativeView.View.OnAdLoaded.Raise();

            public void DidFailToReceiveAd(BannerView adView, RequestError error) => OnError(error);

            public void DidDismissScreen(BannerView adView) => NativeView.View.OnAdClosed.Raise();

            public void WillPresentScreen(BannerView adView) => NativeView.View.OnAdTapped.Raise();
        }

    }
}