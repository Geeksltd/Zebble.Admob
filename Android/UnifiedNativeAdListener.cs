namespace Zebble.AdMob
{
    using Android.Gms.Ads.Formats;

    class UnifiedNativeAdListener : Java.Lang.Object, UnifiedNativeAd.IOnUnifiedNativeAdLoadedListener
    {
        AdAgent Agent;

        public UnifiedNativeAdListener(AdAgent agent) => Agent = agent;
        public void OnUnifiedNativeAdLoaded(UnifiedNativeAd ad) => Agent.OnFetched(new NativeAdInfo(ad));
    }
}
