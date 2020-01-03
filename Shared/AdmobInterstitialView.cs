using System.Threading.Tasks;

namespace Zebble
{
    public class AdmobInterstitialView : AdmobView, IZebbleAdView, IRenderedBy<AdmobViewRenderer>
    {
        internal readonly AsyncEvent OnAdButtonTapped = new AsyncEvent();

        public AdmobTypes AdType => AdmobTypes.Interstitial;

        public override async Task OnInitializing()
        {
            await base.OnInitializing();

            Tapped.Handle(() => OnAdButtonTapped.Raise());
        }
    }
}
