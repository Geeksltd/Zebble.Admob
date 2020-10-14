using Android.Gms.Ads.Rewarded;
using System.Threading.Tasks;

namespace Zebble.AdMob
{
    public partial class RewardedVideoAd
    {
        RewardedAd NativeView;

        public Task LoadRewardedVideo()
        {
            return Thread.UI.Run(() =>
            {
                if (string.IsNullOrEmpty(UnitId))
                {
                    OnAdFailed.Raise("The UnitId of the RewardedVideoView has not specified!");
                    return;
                }

                NativeView = new RewardedAd(UIRuntime.CurrentActivity, UnitId);
                NativeView.LoadAd(new Android.Gms.Ads.AdRequest.Builder().Build(), new AdmobRewardedAdLoadCallback(this));
            });
        }

        public Task ShowVideo() => Thread.UI.Run(() =>
        {
            if (!NativeView.IsLoaded)
            {
                OnAdFailed.Raise("The RewardedVideo has not loaded yet!");
                return;
            }

            NativeView.Show(UIRuntime.CurrentActivity, new AdmobRewardedAdCallback(this));
        });

        class AdmobRewardedAdLoadCallback : RewardedAdLoadCallback
        {
            RewardedVideoAd Ad;

            public AdmobRewardedAdLoadCallback(RewardedVideoAd ad) => Ad = ad;

            public override void OnRewardedAdLoaded() => Ad.OnAdLoaded.Raise();

            public override void OnRewardedAdFailedToLoad(int p0)
            {
                AdmobAndroidListener.OnAdError(p0, out var error);
                Ad.OnAdFailed.Raise(error);
            }
        }

        class AdmobRewardedAdCallback : RewardedAdCallback
        {
            RewardedVideoAd Ad;

            public AdmobRewardedAdCallback(RewardedVideoAd ad) => Ad = ad;

            public override void OnRewardedAdClosed() => Ad.OnAdClosed.Raise();

            public override void OnRewardedAdFailedToShow(int p0)
            {
                AdmobAndroidListener.OnRewardedAdError(p0, out var error);
                Ad.OnAdShowFailed.Raise(error);
            }

            public override void OnRewardedAdOpened() => Ad.OnAdOpened.Raise();

            public override void OnUserEarnedReward(IRewardItem p0)
                => Ad.OnEarnedReward.Raise(new RewardItemArgs { Amount = p0.Amount, Type = p0.Type });
        }
    }
}