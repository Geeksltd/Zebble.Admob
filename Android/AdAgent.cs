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
            if (Loader != null)
                throw new InvalidOperationException("AdAgent.Initialize() should only be called once.");

            var builder = new AdLoader.Builder(Renderer.Context, UnitId);
            builder.ForUnifiedNativeAd(new UnifiedNativeAdListener(this));
            builder.WithAdListener(new ZebbleAdListener(this));

            Loader = builder.Build();
        }

        void RequestNativeAds()
        {
            if (Loader == null)
                Initialize();

            var builder = new AdRequest.Builder();

            foreach (var id in Config.Get("Admob.Android.Test.Device.Ids").OrEmpty().Split(',').Trim())
                builder.AddTestDevice(id);

            if (Keywords.HasValue()) builder.AddKeyword(Keywords);

            try
            {
                Loader.LoadAds(builder.Build(), 5);
            }
            catch (Exception ex)
            {
                // Should not happen.
                Zebble.Device.Log.Error(ex);
            }
        }
    }
}
