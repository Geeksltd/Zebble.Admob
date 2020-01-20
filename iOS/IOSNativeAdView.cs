using Google.MobileAds;
using System;
using System.Threading.Tasks;
using UIKit;

namespace Zebble.AdMob
{
    class IOSNativeAdView : UIView, IZebbleAdNativeView<NativeAdView>
    {
        public NativeAdView View { get; set; }
        UnifiedNativeAdView NativeView;
        NativeAdInfo CurrentAd;
        AdAgent Agent;

        public IOSNativeAdView(NativeAdView view)
        {
            View = view;

            NativeView = new UnifiedNativeAdView { Frame = View.GetFrame() };
            Add(NativeView);

            Agent = (view.Agent ?? throw new Exception(".NativeAdView.Agent is null"));

            view.RotateRequested.Handle(LoadNext);
            LoadNext().RunInParallel();
        }

        async Task LoadNext()
        {
            var ad = await Agent.GetNativeAd(View.Parameters);
            CreateAdView(ad);
        }

        void CreateAdView(NativeAdInfo ad)
        {
            CurrentAd = ad;
            View.Ad.Value = ad;

            NativeView.MediaView = View.MediaView?.Native() as AdmobIOSMediaView;
            NativeView.MediaView.MediaContent = ad.Native.MediaContent;

            NativeView.HeadlineView = View.HeadLineView?.Native();
            NativeView.BodyView = View.BodyView?.Native();
            NativeView.CallToActionView = View.CallToActionView?.Native();
            NativeView.IconView = View.IconView?.Native();
            NativeView.PriceView = View.PriceView?.Native();
            NativeView.StoreView = View.StoreView?.Native();
            NativeView.AdvertiserView = View.AdvertiserView?.Native();

            NativeView.NativeAd = ad.Native;
        }
    }
}