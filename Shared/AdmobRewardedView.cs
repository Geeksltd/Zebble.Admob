using System.Threading.Tasks;

namespace Zebble
{
    public class AdmobRewardedView
    {
        internal readonly AsyncEvent OnShow = new AsyncEvent();

        public readonly AsyncEvent OnAdClosed = new AsyncEvent();
        public readonly AsyncEvent OnAdLoaded = new AsyncEvent();
        public readonly AsyncEvent OnAdOpened = new AsyncEvent();
        public readonly AsyncEvent<RewardItemArgs> OnEarnedReward = new AsyncEvent<RewardItemArgs>();
        public readonly AsyncEvent<string> OnAdFailed = new AsyncEvent<string>();
        public readonly AsyncEvent<string> OnAdShowFailed = new AsyncEvent<string>();

        public string UnitId { get; set; }

        public AdmobTypes AdType => AdmobTypes.Rewarded;

        public Task Show() => OnShow.Raise();
    }

    public class RewardItemArgs
    {
        public int Amount { get; set; }
        public string Type { get; set; }
    }
}
