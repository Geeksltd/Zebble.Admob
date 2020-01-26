using System.Threading.Tasks;
using System;
using Android.Runtime;

namespace Zebble.AdMob
{
    [Preserve]
    public class AdmobViewRenderer : INativeRenderer
    {
        Android.Views.View Result;

        public Task<Android.Views.View> Render(Renderer renderer)
        {
            if (renderer.View is NativeAdView native) Result = new AndroidNativeAdView(native);
            else if (renderer.View is AdmobMediaView media) Result = new AdmobAndroidMediaView(media);
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