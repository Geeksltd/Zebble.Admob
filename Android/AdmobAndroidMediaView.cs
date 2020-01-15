using Android.Gms.Ads.Formats;
using Android.Views;
using System;
using Android.Runtime;

namespace Zebble.AdMob
{
    [Preserve]
    class AdmobAndroidMediaView : MediaView
    {
        AdmobMediaView View;

        [Preserve]
        public AdmobAndroidMediaView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        public AdmobAndroidMediaView(AdmobMediaView view) : base(Renderer.Context)
        {
            View = view;
            LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) View = null;
            base.Dispose(disposing);
        }
    }
}