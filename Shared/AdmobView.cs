namespace Zebble.AdMob
{
    public abstract class AdmobView : View
    {
        public readonly AsyncEvent OnAdTapped = new AsyncEvent();
        public readonly AsyncEvent OnAdClosed = new AsyncEvent();
        public readonly AsyncEvent OnAdLoaded = new AsyncEvent();
        public readonly AsyncEvent OnAdOpened = new AsyncEvent();
        public readonly AsyncEvent<string> OnAdFailed = new AsyncEvent<string>();
    }
}
