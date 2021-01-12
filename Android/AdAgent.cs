using Android.Gms.Ads;
using Android.Gms.Ads.Formats;
using System;
using Olive;

namespace Zebble.AdMob
{
    partial class AdAgent
    {
        AdLoader Loader;

        public void Initialize()
        {
            if (Loader != null)
                throw new InvalidOperationException("AdAgent.Initialize() should only be called once.");

            var builder = new AdLoader.Builder(UIRuntime.CurrentActivity, UnitId);
            builder.ForUnifiedNativeAd(new UnifiedNativeAdListener(this));

            var adOptions = new NativeAdOptions.Builder()
                     .SetVideoOptions(new VideoOptions.Builder().SetStartMuted(true).Build())
                     .Build();

            builder.WithNativeAdOptions(adOptions);

            builder.WithAdListener(new ZebbleAdListener(this));

            Loader = builder.Build();
        }

        void RequestNativeAds()
        {
            if (Loader == null)
                Initialize();

            var builder = new AdRequest.Builder();

            if (Keywords.HasValue()) builder.AddKeyword(Keywords);

            try
            {
                Loader.LoadAds(builder.Build(), 5);
            }
            catch (Exception ex)
            {
               //Should not happen.
               Log.For(this).Error(ex);
            }
        }
    }
}
