using Android.Gms.Ads;
using Android.Gms.Ads.Formats;
using System;
using System.Collections.Generic;

namespace Zebble.AdMob
{
    class ZebbleAdListener : AdListener
    {
        readonly AdAgent Agent;
        public ZebbleAdListener(AdAgent agent) => Agent = agent;
        public override void OnAdFailedToLoad(int errorCode) => Agent.OnAdFailedToLoad("Ad Loading Failed");
    }

    partial class AdAgent
    {
        AdLoader Loader;

        public IList<NativeAdInfo> Ads { get; set; }
        public DateTime LastUpdate { get; set; }

        public void Initialize()
        {
            if (Loader != null) throw new InvalidOperationException("AdAgent.Initialize() should only be called once.");

            var builder = new AdLoader.Builder(Renderer.Context, UnitId);
            builder.ForUnifiedNativeAd(new UnifiedNativeAdListener(this));

            var adOptions = new NativeAdOptions.Builder()
                    .SetVideoOptions(new VideoOptions.Builder().SetStartMuted(IsVideoMuted).Build())
                    .Build();

            builder.WithNativeAdOptions(adOptions);

            builder.WithAdListener(new ZebbleAdListener(this));

            Loader = builder.Build();
        }

        void RequestNativeAd(AdParameters request)
        {
            if (Loader == null)
                Initialize();

            var builder = new AdRequest.Builder();

            foreach (var id in Config.Get("Admob.Android.Test.Device.Ids").OrEmpty().Split(',').Trim())
                builder.AddTestDevice(id);

            if (request.Keywords.HasValue()) builder.AddKeyword(request.Keywords);

            Loader.LoadAds(builder.Build(), 5);
        }

        class UnifiedNativeAdListener : Java.Lang.Object, UnifiedNativeAd.IOnUnifiedNativeAdLoadedListener
        {
            AdAgent Agent;

            public UnifiedNativeAdListener(AdAgent agent) => Agent = agent;

            public void OnUnifiedNativeAdLoaded(UnifiedNativeAd ad)
            {
                var currentAd = new NativeAdInfo(ad);
                if (Agent.Ads == null)
                {
                    Agent.OnNativeAdReady(currentAd);
                    Agent.Ads = new List<NativeAdInfo>();
                }
                else Agent.Ads.Add(currentAd);
                
                Agent.LastUpdate = DateTime.Now;
            }
        }

    }
}
