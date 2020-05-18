using Foundation;
using Google.MobileAds;
using System;
using UIKit;

namespace Zebble.AdMob
{
    partial class AdAgent
    {
        AdLoader Loader;

        public void Initialize()
        {
            if (Loader != null) throw new InvalidOperationException("AdAgent.Initialize() should only be called once.");

            var multipleAds = new MultipleAdsAdLoaderOptions { NumberOfAds = 5 };
            Loader = new AdLoader(UnitId, UIRuntime.NativeRootScreen as UIViewController,
                new[] { AdLoaderAdType.UnifiedNative }, new AdLoaderOptions[] { multipleAds })
            {
                Delegate = new IOSNativeAdListener(this)
            };
        }

        void RequestNativeAds()
        {
            if (Loader == null) Initialize();

            var adRequest = Request.GetDefaultRequest();

            foreach (var id in Config.Get("Admob.iOS.Test.Device.Ids").OrEmpty().Split(',').Trim())
                adRequest.TestDevices.AddLine(id);

            if (Keywords.HasValue()) adRequest.Keywords = new[] { Keywords };
            Loader.LoadRequest(adRequest);
        }

        class IOSNativeAdListener : NSObject, IUnifiedNativeAdLoaderDelegate
        {
            AdAgent Agent;

            public IOSNativeAdListener(AdAgent agent) => Agent = agent;

            public void DidFailToReceiveAd(AdLoader adLoader, RequestError error)
            {
                AdmobIOSListener.OnError(error, out var errorMessage);
                Agent.OnAdFailedToLoad(errorMessage);
            }

            public void DidReceiveUnifiedNativeAd(AdLoader adLoader, UnifiedNativeAd nativeAd)
            {
                Agent.OnFetched(new NativeAdInfo(nativeAd));
            }
        }
    }
}
