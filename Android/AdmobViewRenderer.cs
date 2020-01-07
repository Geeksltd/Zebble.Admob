using System.Threading.Tasks;

namespace Zebble
{
    class AdmobViewRenderer : INativeRenderer
    {
        Android.Views.View Result;

        public Task<Android.Views.View> Render(Renderer renderer)
        {
            var view = (IZebbleAdView)renderer.View;
            switch (view.AdType)
            {
                case AdmobTypes.Banner:
                    Result = new AdmobAndroidBannerView((AdmobBannerView)renderer.View);
                    return Task.FromResult(Result);
                case AdmobTypes.Native:
                    Result = new AdmobAndroidNativeVideoView((AdmobNativeVideoView)renderer.View);
                    return Task.FromResult(Result);
                case AdmobTypes.Media:
                    Result = new AdmobAndroidMediaView((AdmobMediaView)renderer.View);
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