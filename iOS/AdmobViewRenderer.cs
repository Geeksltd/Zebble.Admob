using System;
using System.Threading.Tasks;
using UIKit;

namespace Zebble.AdMob
{
    class AdmobViewRenderer : INativeRenderer
    {
        UIView Result;

        public Task<UIView> Render(Renderer renderer)
        {
            if (renderer.View is NativeAdView native) Result = new IOSNativeAdView(native);
            else if (renderer.View is AdmobMediaView media) Result = new AdmobIOSMediaView(media);
            else throw new NotSupportedException();

            return Task.FromResult(Result);
        }

        public void Dispose()
        {
            Result.Dispose();
            Result = null;
        }
    }
}