using Google.MobileAds;
using System;
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
                if (IsDead(out var view)) return;
                if (t.IsFaulted) return;

                var ad = t.GetAlreadyCompletedResult();
                Thread.UI.Run(() => RenderAd(ad));
            });
        }

        void ConfigureAdView()
        {
            Thread.UI.Run(() =>
           {
               if (IsDead(out var view)) return;

               NativeView.MediaView = view.MediaView?.Native() as AdmobIOSMediaView;
               NativeView.HeadlineView = view.HeadLineView?.Native();
               NativeView.BodyView = view.BodyView?.Native();
               NativeView.CallToActionView = view.CallToActionView?.Native();
               NativeView.IconView = view.IconView?.Native();
               NativeView.PriceView = view.PriceView?.Native();
               NativeView.StoreView = view.StoreView?.Native();
               NativeView.AdvertiserView = view.AdvertiserView?.Native();
           });
        }

        void RenderAd(NativeAdInfo ad)
        {
            if (ad is null) return;
            if (IsDead(out var view)) return;

            if (ad is FailedNativeAdInfo)
            {
                view.HeadLineView.Text = ad.Headline;
                view.BodyView.Text = ad.Body;
                view.CallToActionView.Text = ad.CallToAction;
                return;
            }
            else
            {
                try
                {
                    view.Ad.Value = ad;
                    NativeView.NativeAd = ad.Native;
                    if (NativeView.MediaView != null)
                        NativeView.MediaView.MediaContent = ad.Native.MediaContent;
                }
                catch (Exception ex)
                {
                    Device.Log.Error(ex);
                }
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

        [EscapeGCop("In this case an out parameter can improve the code.")]
        bool IsDead(out NativeAdView result)
        {
            result = View;
            return result != null && !result.IsDisposing;
        }
    }
}