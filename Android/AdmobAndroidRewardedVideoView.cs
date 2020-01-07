using Android.Gms.Ads.Rewarded;

namespace Zebble
{
    class AdmobAndroidRewardedVideoView
    {
        public AdmobRewardedView View { get; set; }
        RewardedAd NativeView;

        public AdmobAndroidRewardedVideoView(AdmobRewardedView view)
        {
            View = view;

            LoadAd();
        }

        void LoadAd()
        {
            NativeView = new RewardedAd(Renderer.Context, View.UnitId);
            NativeView.LoadAd(new Android.Gms.Ads.AdRequest.Builder().Build(), new AdmobRewardedAdLoadCallback(View));

            View.OnShow.Handle(() => NativeView.Show(UIRuntime.CurrentActivity, new AdmobRewardedAdCallback(View)));
        }

        class AdmobRewardedAdLoadCallback : RewardedAdLoadCallback
        {
            AdmobRewardedView View;

            public AdmobRewardedAdLoadCallback(AdmobRewardedView view)
            {
                View = view;
            }

            public override void OnRewardedAdLoaded() => View.OnAdLoaded.Raise();

            public override void OnRewardedAdFailedToLoad(int p0)
            {
                string error;
                AdmobAndroidListener.OnAdError(p0, out error);
                View.OnAdFailed.Raise(error);
            }
        }

        class AdmobRewardedAdCallback : RewardedAdCallback
        {
            AdmobRewardedView View;

            public AdmobRewardedAdCallback(AdmobRewardedView view)
            {
                View = view;
            }

            public override void OnRewardedAdClosed() => View.OnAdClosed.Raise();

            public override void OnRewardedAdFailedToShow(int p0)
            {
                string error;
                AdmobAndroidListener.OnRewardedAdError(p0, out error);
                View.OnAdShowFailed.Raise(error);
            }

            public override void OnRewardedAdOpened() => View.OnAdOpened.Raise();

            public override void OnUserEarnedReward(IRewardItem p0) => View.OnEarnedReward.Raise(new RewardItemArgs { Amount = p0.Amount, Type = p0.Type });
        }
    }
}