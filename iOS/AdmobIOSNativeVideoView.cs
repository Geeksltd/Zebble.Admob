using Foundation;
using Google.MobileAds;
using System;
using UIKit;

namespace Zebble
{
    class AdmobIOSNativeVideoView : UIView, IZebbleAdNativeView<NativeAdView>
    {
        public NativeAdView View { get; set; }
        UnifiedNativeAdView NativeView;

        public AdmobIOSNativeVideoView(NativeAdView view)
        {
            View = view;

            var adloader = new AdLoader(view.UnitId, UIRuntime.NativeRootScreen as UIViewController,
                new[] { AdLoaderAdType.UnifiedNative }, null);

            adloader.Delegate = new IOSNativeAdListener(this);
            adloader.LoadRequest(Request.GetDefaultRequest());
        }
        
        void CreateAdView(UnifiedNativeAd ad)
        {
            NativeView = new UnifiedNativeAdView();
            NativeView.Frame = View.GetFrame();

            Add(NativeView);

            if (View.HeadLineView != null && ad.Headline.HasValue())
                View.HeadLineView.Text = ad.Headline;
            else View.HeadLineView?.Ignored();

            if (View.BodyView != null && ad.Body.HasValue())
                View.BodyView.Text = ad.Body;
            else View.BodyView?.Ignored();

            if (View.CallToActionView != null && ad.CallToAction.HasValue())
                View.CallToActionView.Text = ad.CallToAction;
            else View.CallToActionView?.Ignored();

            if (View.IconView != null && ad.Icon != null)
                View.IconView.ImageData = ConvertUIImageToByteArray(ad.Icon?.Image);
            else View.IconView?.Ignored();

            if (View.PriceView != null && ad.Price.HasValue())
                View.PriceView.Text = ad.Price;
            else View.PriceView?.Ignored();

            if (View.AdvertiserView != null && ad.Advertiser.HasValue())
                View.AdvertiserView.Text = ad.Advertiser;
            else View.AdvertiserView?.Ignored();

            if (View.StoreView != null && ad.Store.HasValue())
                View.StoreView.Text = ad.Store;
            else View.StoreView?.Ignored();

            NativeView.MediaView = View.MediaView?.Native() as AdmobIOSMediaView;
            NativeView.MediaView.MediaContent = ad.MediaContent;

            NativeView.HeadlineView = View.HeadLineView?.Native();
            NativeView.BodyView = View.BodyView?.Native();
            NativeView.CallToActionView = View.CallToActionView?.Native();
            NativeView.IconView = View.IconView?.Native();
            NativeView.PriceView = View.PriceView?.Native();
            NativeView.StoreView = View.StoreView?.Native();
            NativeView.AdvertiserView = View.AdvertiserView?.Native();

            NativeView.NativeAd = ad;
        }

        byte[] ConvertUIImageToByteArray(UIImage img)
        {
            if (img == null) return new byte[0];

            using (NSData imageData = img.AsPNG())
            {
                var image = new byte[imageData.Length];
                System.Runtime.InteropServices.Marshal.Copy(imageData.Bytes, image, 0, Convert.ToInt32(imageData.Length));
                return image;
            }
        }

        class IOSNativeAdListener : AdmobIOSListener<NativeAdView>, IUnifiedNativeAdLoaderDelegate, IUnifiedNativeAdDelegate
        {
            AdmobIOSNativeVideoView NativeView;

            public IOSNativeAdListener(AdmobIOSNativeVideoView nativeView) : base(nativeView)
            {
                NativeView = nativeView;
            }

            public void DidReceiveUnifiedNativeAd(AdLoader adLoader, UnifiedNativeAd nativeAd)
            {
                NativeView.View.OnAdReady.Raise(new AdmobNativeInfo
                {
                    Headline = nativeAd.Headline,
                    Icon = NativeView.ConvertUIImageToByteArray(nativeAd.Icon?.Image),
                    Price = nativeAd.Price,
                    Advertiser = nativeAd.Advertiser,
                    Body = nativeAd.Body,
                    StarRating = (double?)(nativeAd.StarRating ?? null),
                    Store = nativeAd.Store,
                    HasData = nativeAd.Headline.OrNullIfEmpty() == null ? false : true
                });
                NativeView.CreateAdView(nativeAd);
            }

            public void DidRecordImpression(UnifiedNativeAd nativeAd) => NativeView.View.OnAdOpened.Raise();

            public void DidRecordClick(UnifiedNativeAd nativeAd) => NativeView.View.OnAdTapped.Raise();

            public void DidDismissScreen(UnifiedNativeAd nativeAd) => NativeView.View.OnAdClosed.Raise();

            public void DidFailToReceiveAd(AdLoader adLoader, RequestError error) => OnError(error);
        }
    }
}