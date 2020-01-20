using Google.MobileAds;

namespace Zebble.AdMob
{
    static class AdmobIOSListener
    {
        public static void OnError(RequestError error, out string errorMessage)
        {
            switch (error.Code)
            {
                case (int)AdmobListenerErrors.InternalError:
                    errorMessage = "Something happened internally; for instance, an invalid response was received from the ad server.";
                    break;
                case (int)AdmobListenerErrors.InvalidRequest:
                    errorMessage = "The ad request was invalid; for instance, the ad unit ID was incorrect.";
                    break;
                case (int)AdmobListenerErrors.NetwordError:
                    errorMessage = "The ad request was unsuccessful due to network connectivity.";
                    break;
                case (int)AdmobListenerErrors.NoFill:
                    errorMessage = "The ad request was successful, but no ad was returned due to lack of ad inventory.";
                    break;
                default:
                    errorMessage = null;
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