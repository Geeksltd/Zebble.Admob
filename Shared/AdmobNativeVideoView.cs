using System.Threading.Tasks;

namespace Zebble
{
    public partial class AdmobNativeVideoView : AdmobView, IZebbleAdView, IRenderedBy<AdmobViewRenderer>
    {
        public readonly AsyncEvent OnAdReady = new AsyncEvent();
        public readonly AsyncEvent OnVideoEnded = new AsyncEvent();

        public AdmobTypes AdType => AdmobTypes.Native;

        public TextView HeadLineView { get; set; }

        public TextView BodyView { get; set; }

        public Button CallToActionView { get; set; }

        public ImageView IconView { get; set; }

        public TextView PriceView { get; set; }

        public TextView StoreView { get; set; }

        public TextView AdvertiserView { get; set; }

        public AdmobMediaView MediaView { get; set; }

        public bool IsVideoMuted { get; set; }

    }
}
