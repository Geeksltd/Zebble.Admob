using System;

namespace Zebble.AdMob
{
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
}
