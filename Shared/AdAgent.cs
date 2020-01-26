using System;
using System.Threading.Tasks;

namespace Zebble.AdMob
{
    public partial class AdAgent
    {
        public bool IsVideoMuted { get; set; }
        public string UnitId { get; set; }

        public AdAgent(string unitId) => UnitId = unitId;

        TaskCompletionSource<NativeAdInfo> NextNativeAd;

        public void OnAdFailedToLoad(string reason) => NextNativeAd?.TrySetResult(FailedNativeAdInfo.Create(reason));

        public void OnNativeAdReady(NativeAdInfo ad) => NextNativeAd?.TrySetResult(ad);

        public Task<NativeAdInfo> GetNativeAd(AdParameters request)
        {
            NextNativeAd = new TaskCompletionSource<NativeAdInfo>();
            RequestNativeAd(request);
            return NextNativeAd.Task;
        }
    }

    public class AdParameters
    {
        public string Keywords;
    }

    public class FailedNativeAdInfo : NativeAdInfo
    {
        static Func<string, FailedNativeAdInfo> CustomProvider;
        public virtual string ImageUrl { get; set; }
        public virtual string TargetUrl { get; set; }

        public static void OnRequested(Func<string, FailedNativeAdInfo> provider) => CustomProvider = provider;

        internal static FailedNativeAdInfo Create(string reason)
        {
            return CustomProvider?.Invoke(reason) ?? new

            FailedNativeAdInfo
            {
                Headline = "Ad loading failed",
                Body = reason,
                CallToAction = "Try again later",
            };
        }
    }

    public partial class NativeAdInfo
    {
        public virtual string Headline { get; set; } = "...";
        public virtual string Price { get; set; }
        public virtual string Advertiser { get; set; }
        public virtual string Body { get; set; }
        public virtual double? StarRating { get; set; }
        public virtual string Store { get; set; }
        public virtual string CallToAction { get; set; } = "Open";
        public virtual byte[] Icon { get; set; }

        public bool HasData
        {
            get
            {
                if (Headline.None() && Body.None() && CallToAction.None()) return false;
                return true;
            }
        }

        public NativeAdInfo()
        {
        }
    }
}