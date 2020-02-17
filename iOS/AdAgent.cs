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

        void RequestNativeAd(AdParameters request)
        {
            if (Loader == null)
                Initialize();

            var adRequest = Request.GetDefaultRequest();

            foreach (var id in Config.Get("Admob.iOS.Test.Device.Ids").OrEmpty().Split(',').Trim())
                adRequest.TestDevices.AddLine(id);

            if (request.Keywords.HasValue()) adRequest.Keywords.AddLine(request.Keywords);
            Loader.LoadRequest(adRequest);
        }

        class IOSNativeAdListener : NSObject, IUnifiedNativeAdLoaderDelegate
        {
            AdAgent Agent;

            public IOSNativeAdListener(AdAgent agent) => Agent = agent;

            public void DidFailToReceiveAd(AdLoader adLoader, RequestError error)
            {
                string errorMessage;
                AdmobIOSListener.OnError(error, out errorMessage);
                Device.Log.Error(errorMessage);
            }

            public void DidReceiveUnifiedNativeAd(AdLoader adLoader, UnifiedNativeAd nativeAd)
            {
                var currentAd = new NativeAdInfo(nativeAd);
                Agent.LastUpdate = DateTime.Now;

                if (Agent.Ads.None())
                {
                    Agent.OnNativeAdReady(currentAd);
                    currentAd.IsShown = true;
                }

                Agent.Ads.Add(currentAd);
            }
        }
    }
}
