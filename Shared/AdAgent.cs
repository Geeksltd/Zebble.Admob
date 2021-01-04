using System;
using System.Linq;
using System.Threading.Tasks;
using Olive;

namespace Zebble.AdMob
{
    public partial class AdAgent
    {
        int Iterator;
        public string UnitId { get; set; }
        TaskCompletionSource<NativeAdInfo> NextNativeAd;
        internal readonly ConcurrentList<NativeAdInfo> Ads = new ConcurrentList<NativeAdInfo>();
        internal DateTime LastFetched { get; set; }
        public string Keywords { get; set; }
        internal TimeSpan RefetchMinimumWait = 2.Minutes();

        public AdAgent(string unitId)
        {
            UnitId = unitId;
            Thread.Pool.Run(FetchNew).RunInParallel();
        }

        TimeSpan SinceLastFetched => DateTime.UtcNow.Subtract(LastFetched);

        public void OnAdFailedToLoad(string reason) => NextNativeAd?.TrySetResult(FailedNativeAdInfo.Create(reason));

        public void OnNativeAdReady(NativeAdInfo ad) => NextNativeAd?.TrySetResult(ad);

        /// <summary>
        /// Refreshes the Ads and then returns the first one.
        /// </summary>
        public Task<NativeAdInfo> Fetch()
        {
            if (SinceLastFetched < RefetchMinimumWait)
            {
                if (TryReuse(out var result)) return result;
            }

            return FetchNew();
        }

        Task<NativeAdInfo> FetchNew()
        {
            LastFetched = DateTime.UtcNow;
            NextNativeAd = new TaskCompletionSource<NativeAdInfo>();
            RequestNativeAds();

            return NextNativeAd.Task;
        }

        bool TryReuse(out Task<NativeAdInfo> result)
        {
            var ads = Ads?.ToArray();

            if (ads.None())
            {
                if (NextNativeAd != null)
                {
                    result = NextNativeAd.Task;
                    return true;
                }
                else
                {
                    result = null;
                    return false;
                }
            }

            lock (this)
            {
                if (Iterator >= ads.Length) Iterator = 0;

                var item = ads.ElementAtOrDefault(Iterator) ?? ads.First();
                Iterator++;
                result = Task.FromResult(item);
                return true;
            }
        }

        internal void OnFetched(NativeAdInfo ad)
        {
            Ads.Add(ad);

            if (Ads.IsSingle())
                OnNativeAdReady(ad);
        }
    }
}