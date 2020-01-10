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
                case AdmobTypes.Native:
                    Result = new AdmobIOSNativeVideoView((NativeAdView)renderer.View);
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