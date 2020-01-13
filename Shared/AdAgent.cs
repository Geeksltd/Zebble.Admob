using System;
using System.Threading.Tasks;

namespace Zebble.AdMob
{
    public partial class AdAgent
    {
        public readonly AsyncEvent<NativeAdInfo> OnAdReady = new AsyncEvent<NativeAdInfo>();

        public bool IsVideoMuted { get; set; }
        public string UnitId { get; set; }

        public AdAgent(string unitId) => UnitId = unitId;

        TaskCompletionSource<NativeAdInfo> NextNativeAd;

        public void OnNativeAdReady(NativeAdInfo ad)
        {
            if (NextNativeAd == null) throw new Exception("No body is waiting for the ad.");
            NextNativeAd.TrySetResult(ad);
            OnAdReady.Raise(ad);
        }

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

    public partial class NativeAdInfo
    {
        public string Headline { get; internal set; } = "...";
        public string Price { get; internal set; }
        public string Advertiser { get; internal set; }
        public string Body { get; internal set; }
        public double? StarRating { get; internal set; }
        public string Store { get; internal set; }
        public string CallToAction { get; internal set; } = "Open";
        public byte[] Icon { get; internal set; }

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
