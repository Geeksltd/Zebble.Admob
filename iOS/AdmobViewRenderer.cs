using System.Threading.Tasks;
using UIKit;

namespace Zebble
{
    class AdmobViewRenderer : INativeRenderer
    {
        UIView Result;

        public Task<UIView> Render(Renderer renderer)
        {
            var view = (IZebbleAdView)renderer.View;
            switch (view.AdType)
            {
                case AdmobTypes.Banner:
                    Result = new AdmobIOSBannerView((AdmobBannerView)renderer.View);
                    return Task.FromResult(Result);
                case AdmobTypes.Interstitial:
                    throw new System.NotImplementedException();
                case AdmobTypes.Rewarded:
                    throw new System.NotImplementedException();
                case AdmobTypes.Native:
                    Result = new AdmobIOSNativeVideoView((AdmobNativeVideoView)renderer.View);
                    return Task.FromResult(Result);
                case AdmobTypes.Media:
                    Result = new AdmobIOSMediaView((AdmobMediaView)renderer.View);
                    return Task.FromResult(Result);
                default: return null;
            }
        }

        public void Dispose()
        {
            Result.Dispose();
            Result = null;
        }
    }
}