﻿namespace Zebble.AdMob
{
    public partial class InterstitialAd
    {
        internal readonly AsyncEvent OnShow = new AsyncEvent();

        public readonly AsyncEvent OnAdTapped = new AsyncEvent();
        public readonly AsyncEvent OnAdClosed = new AsyncEvent();
        public readonly AsyncEvent OnAdLoaded = new AsyncEvent();
        public readonly AsyncEvent OnAdOpened = new AsyncEvent();
        public readonly AsyncEvent<string> OnAdFailed = new AsyncEvent<string>();

        public string UnitId { get; set; }

        public AdmobTypes AdType => AdmobTypes.Interstitial;
    }
}
