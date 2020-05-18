using Microsoft.Advertising.WinRT.UI;

namespace Zebble.AdMob
{
    partial class AdAgent
    {
        NativeAdsManagerV2 NativeAdsManager;

        public void Initialize()
        {
            NativeAdsManager = new NativeAdsManagerV2(Admob.ApplicationCode, UnitId);
            NativeAdsManager.AdReady += NativeAdsManager_AdReady;
        }

        void RequestNativeAds()
        {
            if (NativeAdsManager == null)
                Initialize();

            NativeAdsManager.RequestAd();
        }

        void NativeAdsManager_AdReady(object sender, NativeAdReadyEventArgs e) => OnNativeAdReady(new NativeAdInfo(e.NativeAd));
    }
}
