using Android.Gms.Ads;

namespace Zebble
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
            var error = "";
            switch (p0)
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

            AdView.View.OnAdFailed.Raise(error);
        }

        public override void OnAdOpened() => AdView.View.OnAdOpened.Raise();

    }
}