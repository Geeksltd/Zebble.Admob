using Android.Gms.Ads;
using Android.Gms.Ads.Formats;
using System;

namespace Zebble.AdMob
{
    partial class AdAgent
    {
        AdLoader Loader;

        public void Initialize()
        {
            if (Loader != null) throw new InvalidOperationException("AdAgent.Initialize() should only be called once.");

            var builder = new AdLoader.Builder(Renderer.Context, UnitId);
            builder.ForUnifiedNativeAd(new UnifiedNativeAdListener(this));

            var adOptions = new NativeAdOptions.Builder()
                    .SetVideoOptions(new VideoOptions.Builder().SetStartMuted(IsVideoMuted).Build())
                    .Build();

            builder.WithNativeAdOptions(adOptions);

            Loader = builder.Build();
        }

        void RequestNativeAd(AdParameters request)
        {
            if (Loader == null)
                Initialize();

            var builder = new AdRequest.Builder();
            if (request.Keywords.HasValue()) builder.AddKeyword(request.Keywords);

            Loader.LoadAd(builder.Build());
        }

        class UnifiedNativeAdListener : Java.Lang.Object, UnifiedNativeAd.IOnUnifiedNativeAdLoadedListener
        {
            AdAgent Agent;

            public UnifiedNativeAdListener(AdAgent agent) => Agent = agent;

            public void OnUnifiedNativeAdLoaded(UnifiedNativeAd ad) => Agent.OnNativeAdReady(new NativeAdInfo(ad));
        }
    }
}
