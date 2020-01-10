namespace Zebble.AdMob
{
    public partial class NativeAdView : AdmobView, IZebbleAdView, IRenderedBy<AdmobViewRenderer>
    {
        public readonly Bindable<NativeAdInfo> Ad = new Bindable<NativeAdInfo>(new NativeAdInfo());
        public readonly AsyncEvent OnVideoEnded = new AsyncEvent();
        internal readonly AsyncEvent RotateRequested = new AsyncEvent();

        public void Rotate() => RotateRequested.RaiseOn(Thread.UI);

        public AdmobTypes AdType => AdmobTypes.Native;

        public TextView HeadLineView { get; set; }
        public TextView BodyView { get; set; }
        public Button CallToActionView { get; set; }
        public ImageView IconView { get; set; }
        public TextView PriceView { get; set; }
        public TextView StoreView { get; set; }
        public TextView AdvertiserView { get; set; }
        public AdmobMediaView MediaView { get; set; }

        public AdAgent Agent { get; set; }
        public readonly AdParameters Parameters = new AdParameters();
    }
}
