using System.Threading.Tasks;
using System;
using Windows.UI.Xaml;

namespace Zebble.AdMob
{
    class AdmobViewRenderer : INativeRenderer
    {
        FrameworkElement Result;

        Task<FrameworkElement> INativeRenderer.Render(Renderer renderer)
        {
            if (renderer.View is NativeAdView native) Result = new UWPNativeAdView(native).Render();
            else if (renderer.View is AdmobMediaView media) Result = new AdmobUWPMediaView(media);
            else throw new NotSupportedException();

            return Task.FromResult(Result);
        }

        public void Dispose()
        {
            Result = null;
        }
    }
}