using Foundation;
using Google.MobileAds;
using ObjCRuntime;
using System;

namespace Zebble
{
    abstract class AdmobIOSListener<TView> : NSObject, INativeObject, IDisposable where TView : AdmobView
    {
        IZebbleAdNativeView<TView> AdView;

        public AdmobIOSListener(IZebbleAdNativeView<TView> adView)
        {
            AdView = adView;
        }

        internal void OnError(RequestError error)
        {
            var errorMessage = "";
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

            AdView.View.OnAdFailed.Raise(errorMessage);
        }
    }
}