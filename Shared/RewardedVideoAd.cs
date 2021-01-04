namespace Zebble.AdMob
{
    public partial class RewardedVideoAd
    {
        public readonly AsyncEvent OnAdClosed = new AsyncEvent();
        public readonly AsyncEvent OnAdLoaded = new AsyncEvent();
        public readonly AsyncEvent OnAdOpened = new AsyncEvent();
        public readonly AsyncEvent<RewardItemArgs> OnEarnedReward = new AsyncEvent<RewardItemArgs>();
        public readonly AsyncEvent<string> OnAdFailed = new AsyncEvent<string>();
        public readonly AsyncEvent<string> OnAdShowFailed = new AsyncEvent<string>();

        public string UnitId { get; set; }

        public AdmobTypes AdType => AdmobTypes.Rewarded;
    }

    public class RewardItemArgs
    {
        public int Amount { get; set; }
        public string Type { get; set; }
    }
}
