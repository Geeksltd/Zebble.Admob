using Foundation;
using Google.MobileAds;
using System.Threading.Tasks;
using UIKit;

namespace Zebble.AdMob
{
    public partial class RewardedVideoAd
    {
        RewardedAd NativeAd;

        public Task LoadRewardedVideo()
        {
            return Thread.UI.Run(async () =>
            {
                if (string.IsNullOrEmpty(UnitId))
                {
                    await OnAdFailed.Raise("The UnitId of the RewardedVideoView has not specified!");
                    return;
                }

                NativeAd = new RewardedAd(UnitId);
                var error = await NativeAd.LoadRequestAsync(Request.GetDefaultRequest());
                if (error != null)
                {
                    AdmobIOSListener.OnError(error, out var errorMessage);
                    await OnAdFailed.Raise(errorMessage);
                }
                else await OnAdLoaded.Raise();
            });
        }

        public Task ShowVideo() => Thread.UI.Run(() =>
        {
            if (!NativeAd.IsReady)
            {
                OnAdFailed.Raise("The RewardedVideo has not loaded yet!");
                return;
            }

            if (UIRuntime.NativeRootScreen is UIViewController rootViewController)
                NativeAd.Present(rootViewController, new RewardedVideoDelegate(this));
        });

        class RewardedVideoDelegate : NSObject, IRewardedAdDelegate
        {
            RewardedVideoAd Ad;

            public RewardedVideoDelegate(RewardedVideoAd ad)
            {
                Ad = ad;
            }

            public void UserDidEarnReward(RewardedAd rewardedAd, AdReward reward) =>
                Ad.OnEarnedReward.Raise(new RewardItemArgs { Amount = (int)reward.Amount, Type = reward.Type });

            public void DidFailToPresent(RewardedAd rewardedAd, NSError error)
            {
                AdmobIOSListener.OnRewardedAdError((int)error.Code, out var errorMessage);
                Ad.OnAdShowFailed.Raise(errorMessage);
            }

            public void DidPresent(RewardedAd rewardedAd) => Ad.OnAdOpened.Raise();

            public void DidDismiss(RewardedAd rewardedAd) => Ad.OnAdClosed.Raise();
        }
    }
}