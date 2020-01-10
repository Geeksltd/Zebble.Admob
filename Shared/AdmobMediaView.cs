namespace Zebble.AdMob
{
    public class AdmobMediaView : View, IZebbleAdView, IRenderedBy<AdmobViewRenderer>
    {
        public AdmobTypes AdType => AdmobTypes.Media;
    }
}
