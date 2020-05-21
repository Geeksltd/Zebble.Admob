using Firebase.Analytics;
using Google.MobileAds;
using System;
using System.Linq;
using System.Threading.Tasks;
using UIKit;

namespace Zebble.AdMob
{
    class IOSNativeAdView : UIView, IZebbleAdNativeView<NativeAdView>
    {
        public NativeAdView View { get; set; }
        UnifiedNativeAdView NativeView;
        AdAgent Agent;

        public IOSNativeAdView(NativeAdView view)
        {
            try
            {
                View = view;
                View.RotateRequested += LoadNext;

                NativeView = new UnifiedNativeAdView { Frame = View.GetFrame() };
                Add(NativeView);
                View.WhenShown(ConfigureAdView).RunInParallel();

                Agent = (view.Agent ?? throw new Exception(".NativeAdView.Agent is null"));
                LoadNext();

            }
            catch (Exception ex)
            {
                Device.Log.Error($"[Zebble.Admob] => {ex.Message}");
            }
        }

        void LoadNext()
        {
            Agent.Fetch().ContinueWith(t =>
            {
                if (View == null || View.IsDisposing) return;
                if (t.IsFaulted) return;

                var ad = t.GetAlreadyCompletedResult();
                Thread.UI.Run(() => RenderAd(ad));
            });
        }

        Task ConfigureAdView()
        {
            NativeView.MediaView = View.MediaView?.Native() as AdmobIOSMediaView;
            NativeView.HeadlineView = View.HeadLineView?.Native();
            NativeView.BodyView = View.BodyView?.Native();
            NativeView.CallToActionView = View.CallToActionView?.Native();
            NativeView.IconView = View.IconView?.Native();
            NativeView.PriceView = View.PriceView?.Native();
            NativeView.StoreView = View.StoreView?.Native();
            NativeView.AdvertiserView = View.AdvertiserView?.Native();

            return Task.CompletedTask;
        }

        void RenderAd(NativeAdInfo ad)
        {
            if (ad is null) return;

            if (ad is FailedNativeAdInfo)
            {
                View.HeadLineView.Text = ad.Headline;
                View.BodyView.Text = ad.Body;
                View.CallToActionView.Text = ad.CallToAction;
                return;
            }
            else
            {
                View.Ad.Value = ad;
                NativeView.MediaView.MediaContent = ad.Native.MediaContent;
                NativeView.NativeAd = ad.Native;
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && View != null)
                {
                    View.RotateRequested -= LoadNext;
                    View = null;
                }
                base.Dispose(disposing);
            }
            catch (Exception ex)
            {
                Device.Log.Error(ex);
            }
        }
    }
}