using Android.Gms.Ads;

namespace Zebble.AdMob
{
    class AdmobAndroidListener<TView> : AdListener where TView : AdmobView
    {
        IZebbleAdNativeView<TView> AdView;

        public AdmobAndroidListener(IZebbleAdNativeView<TView> adView)
        {
            AdView = adView;
        }

        public override void OnAdClicked() => AdView.View.OnAdTapped.Raise();

        public override void OnAdClosed() => AdView.View.OnAdClosed.Raise();

        public override void OnAdLoaded() => AdView.View.OnAdLoaded.Raise();

        public override void OnAdFailedToLoad(int p0)
        {
            string error;
            AdmobAndroidListener.OnAdError(p0, out error);
            AdView.View.OnAdFailed.Raise(error);
        }

        public override void OnAdOpened() => AdView.View.OnAdOpened.Raise();
    }

    static class AdmobAndroidListener
    {
        public static void OnAdError(int arg, out string error)
        {
            switch (arg)
            {
                case (int)AdmobListenerErrors.InternalError:
                    error = "Something happened internally; for instance, an invalid response was received from the ad server.";
                    break;
                case (int)AdmobListenerErrors.InvalidRequest:
                    error = "The ad request was invalid; for instance, the ad unit ID was incorrect.";
                    break;
                case (int)AdmobListenerErrors.NetwordError:
                    error = "The ad request was unsuccessful due to network connectivity.";
                    break;
                case (int)AdmobListenerErrors.NoFill:
                    error = "The ad request was successful, but no ad was returned due to lack of ad inventory.";
                    break;
                default:
                    error = null;
                    break;
            }
        }
        public static void OnRewardedAdError(int arg, out string error)
        {
            switch (arg)
            {
                case (int)AdmobListenerRewardedError.InternalError:
                    error = "Something happened internally; for instance, an invalid response was received from the ad server.";
                    break;
                case (int)AdmobListenerRewardedError.AdReused:
                    error = "The rewarded ad has already been shown. RewardedAd objects are one-time use objects and can only be shown once. Instantiate and load a new RewardedAd to display a new ad.";
                    break;
                case (int)AdmobListenerRewardedError.NotReady:
                    error = "The ad has not been successfully loaded.";
                    break;
                case (int)AdmobListenerRewardedError.AppNotForeground:
                    error = "The ad can not be shown when the app is not in foreground.";
                    break;
                default:
                    error = null;
                    break;
            }
        }

    }
}