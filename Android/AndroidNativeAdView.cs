﻿using System;
using System.Linq;
using Android.Gms.Ads;
using Android.Gms.Ads.Formats;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Olive;
using Zebble.AndroidOS;

namespace Zebble.AdMob
{
    [Preserve]
    class AndroidNativeAdView : FrameLayout, IZebbleAdNativeView<NativeAdView>
    {
        bool IsDisposing;
        public NativeAdView View { get; set; }
        UnifiedNativeAdView NativeView;
        NativeAdInfo CurrentAd;
        AdAgent Agent;
        VideoControllerCallback VideoCallBack;

        [Preserve]
        public AndroidNativeAdView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        public AndroidNativeAdView(NativeAdView view) : base(UIRuntime.CurrentActivity)
        {
            try
            {
                View = view;
                View.RotateRequested += LoadNext;

                EscalatePanningEvent();

                AddView(NativeView = new UnifiedNativeAdView(UIRuntime.CurrentActivity)
                {
                    LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent),
                    AdChoicesView = new AdChoicesView(UIRuntime.CurrentActivity) { LayoutParameters = new ViewGroup.LayoutParams(25, 25) }
                });

                View.WhenShown(ConfigureAdView).RunInParallel();

                Agent = view.Agent ?? throw new Exception(".NativeAdView.Agent is null");

                LoadNext();
            }
            catch (Exception ex)
            {
                Log.For(this).Error(ex, "[Zebble.Admob]");
            }
        }

        void EscalatePanningEvent()
        {
            if (IsDead(out var view)) return;

            view.Panning.Handle(args =>
            {
                CurrentAd?.Native?.CancelUnconfirmedClick();

                var handlerParent = view.GetAllParents().FirstOrDefault(x => x?.Panning?.IsHandled() == true);
                handlerParent?.RaisePanning(args);
            });
        }

        protected override void Dispose(bool disposing)
        {
            IsDisposing = true;
            if (disposing && View != null)
            {
                View.RotateRequested -= LoadNext;
                View = null;
            }

            base.Dispose(disposing);
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
            Thread.UI.Post(() =>
            {
                if (IsDead(out var view)) return;

                NativeView.MediaView = view.MediaView?.Native() as AdmobAndroidMediaView;
                NativeView.HeadlineView = view.HeadLineView?.Native();
                NativeView.BodyView = view.BodyView?.Native();
                NativeView.CallToActionView = view.CallToActionView?.Native();
                NativeView.IconView = view.IconView?.Native();
                NativeView.PriceView = view.PriceView?.Native();
                NativeView.StoreView = view.StoreView?.Native();
                NativeView.AdvertiserView = view.AdvertiserView?.Native();

                VideoCallBack = new VideoControllerCallback(view);
            });
        }

        void RenderAd(NativeAdInfo ad)
        {
            if (IsDead(out var view)) return;
            if (ad is null) return;

            CurrentAd = ad;
            view.Ad.Value = ad;

            if (ad is FailedNativeAdInfo)
            {
                view.HeadLineView.Text = ad.Headline;
                view.BodyView.Text = ad.Body;
                view.CallToActionView.Text = ad.CallToAction;
                return;
            }
            else
            {
                NativeView.SetNativeAd(ad.Native);

                var vc = ad.Native.VideoController;
                if (vc.HasVideoContent)
                {
                    var callback = vc.GetVideoLifecycleCallbacks();
                    if (callback == null)
                        vc.SetVideoLifecycleCallbacks(VideoCallBack);
                }
            }
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            this.TraverseUpToFind<AndroidGestureView>()?.OnTouchEvent(ev);
            return base.OnInterceptTouchEvent(ev);
        }

        [EscapeGCop("In this case an out parameter can improve the code.")]
        bool IsDead(out NativeAdView result)
        {
            result = View;
            if (IsDisposing || result is null) return true;
            return result.IsDisposing;
        }

        class VideoControllerCallback : VideoController.VideoLifecycleCallbacks
        {
            NativeAdView View;

            public VideoControllerCallback(NativeAdView view) => View = view;

            public override void OnVideoEnd()
            {
                View.OnVideoEnded.Raise();
                base.OnVideoEnd();
            }
        }
    }
}