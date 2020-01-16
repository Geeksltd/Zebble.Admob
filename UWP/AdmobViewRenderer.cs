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
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            Result = null;
        }
    }
}