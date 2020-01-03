namespace Zebble
{
    public class AdmobBannerView : AdmobView, IZebbleAdView, IRenderedBy<AdmobViewRenderer>
    {
        internal readonly AsyncEvent Paused = new AsyncEvent();
        internal readonly AsyncEvent Resumed = new AsyncEvent();

        public AdmobTypes AdType => AdmobTypes.Banner;

        public Size BannerSize { get; set; }

        public void Pause() => Paused.Raise();

        public void Resume() => Resumed.Raise();
    }
}
