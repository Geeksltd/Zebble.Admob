
namespace Zebble
{
    public class AdmobMediaView : View, IZebbleAdView, IRenderedBy<AdmobViewRenderer>
    {
        public AdmobTypes AdType => AdmobTypes.Media;
    }
}
