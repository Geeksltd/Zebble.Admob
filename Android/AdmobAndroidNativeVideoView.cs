using System;
using System.Collections.Concurrent;
using System.IO;
using Android.Gms.Ads;
using Android.Gms.Ads.Formats;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Widget;
using Zebble.AndroidOS;
using Android.Views;

namespace Zebble
{
    class AdmobAndroidNativeVideoView : FrameLayout, IZebbleAdNativeView<AdmobNativeVideoView>
    {
        public AdmobNativeVideoView View { get; set; }
        UnifiedNativeAdView NativeView;
        UnifiedNativeAd NativeAd;
        Android.Widget.Button ActionButton;

        ConcurrentList<BaseGestureRecognizer> Recognizers = new ConcurrentList<BaseGestureRecognizer>();
        WeakReference<Zebble.View> LatestHandler = new WeakReference<Zebble.View>(null);
        Point LatestPoint;

        public AdmobAndroidNativeVideoView(AdmobNativeVideoView view) : base(Renderer.Context)
        {
            View = view;

            AddGestureRecognizer(new TapGestureRecognizer { OnGestureRecognized = HandleTapped });
            AddGestureRecognizer(new PanGestureRecognizer(p => DetectHandler(p)));

            var builder = new AdLoader.Builder(Renderer.Context, View.UnitId);
            builder.ForUnifiedNativeAd(new UnifiedNativeAdListener(this));

            var videoOptions = new VideoOptions.Builder()
                .SetStartMuted(View.IsVideoMuted)
                .Build();

            var adOptions = new NativeAdOptions.Builder()
                    .SetVideoOptions(videoOptions)
                    .Build();

            builder.WithNativeAdOptions(adOptions);
            builder.WithAdListener(new AdmobAndroidListener<AdmobNativeVideoView>(this));
            var adLoader = builder.Build();

            adLoader.LoadAd(new AdRequest.Builder().Build());
        }

        void CreateAdView(UnifiedNativeAd ad)
        {
            NativeAd = ad;
            NativeView = new UnifiedNativeAdView(Renderer.Context)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent)
            };

            AddView(NativeView);

            if (View.HeadLineView != null && ad.Headline.HasValue())
                View.HeadLineView.Text = ad.Headline;
            else View.HeadLineView?.Ignored();

            if (View.BodyView != null && ad.Body.HasValue())
                View.BodyView.Text = ad.Body;
            else View.BodyView?.Ignored();

            if (View.CallToActionView != null && ad.CallToAction.HasValue())
            {
                ActionButton = new Android.Widget.Button(Renderer.Context) { Visibility = ViewStates.Invisible, Text = ad.CallToAction };
                NativeView.AddView(ActionButton);

                View.CallToActionView.Text = ad.CallToAction;
            }
            else View.CallToActionView?.Ignored();

            if (View.IconView != null && ad.Icon != null)
                View.IconView.ImageData = ConvertDrawableToByteArray(ad.Icon?.Drawable);
            else View.IconView?.Ignored();

            if (View.PriceView != null && ad.Price.HasValue())
                View.PriceView.Text = ad.Price;
            else View.PriceView?.Ignored();

            if (View.AdvertiserView != null && ad.Advertiser.HasValue())
                View.AdvertiserView.Text = ad.Advertiser;
            else View.AdvertiserView?.Ignored();

            if (View.StoreView != null && ad.Store.HasValue())
                View.StoreView.Text = ad.Store;
            else View.StoreView?.Ignored();

            NativeView.MediaView = View.MediaView?.Native() as AdmobAndroidMediaView;
            NativeView.HeadlineView = View.HeadLineView?.Native();
            NativeView.BodyView = View.BodyView?.Native();
            NativeView.CallToActionView = ActionButton;
            NativeView.IconView = View.IconView?.Native();
            NativeView.PriceView = View.PriceView?.Native();
            NativeView.StoreView = View.StoreView?.Native();
            NativeView.AdvertiserView = View.AdvertiserView?.Native();

            NativeView.SetNativeAd(ad);

            var vc = ad.VideoController;

            if (vc.HasVideoContent) vc.SetVideoLifecycleCallbacks(new VideoControllerCallback(View));
        }

        byte[] ConvertDrawableToByteArray(Drawable drawable)
        {
            if (drawable == null) return new byte[0];

            var bitmap = ((BitmapDrawable)drawable).Bitmap;
            using (var stream = new MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                return stream.ReadAllBytes();
            }
        }

        void HandleTouched(View handler, Point point)
        {
            point = point.RelativeTo(handler);
            var args = new Zebble.TouchEventArgs(handler, point, 1);
            handler.RaiseTouched(args);
        }

        void HandleTapped(View handler, Point point, int touches)
        {
            Device.Keyboard.Hide();

            if (handler is Button btn && btn.Text == NativeAd.CallToAction)
            {
                ActionButton.PerformClick();
            }

            point = point.RelativeTo(handler);
            handler.RaiseTapped(new Zebble.TouchEventArgs(handler, point, touches));
        }

        View DetectHandler(Point point, Zebble.View view = null)
        {
            point.X = Scaler.ToZebble(point.X);
            point.X += View.CalculateAbsoluteX();

            point.Y = Scaler.ToZebble(point.Y);
            point.Y += View.CalculateAbsoluteY();

            return new HitTester(point).FindHandler();
        }

        bool OnTouch(MotionEvent ev)
        {
            var point = ev.GetPoint();
            Zebble.View handler = null;

            if (point.X == LatestPoint.X && point.Y == LatestPoint.Y && ev.EventTime > ev.DownTime && ev.EventTime - ev.DownTime < 200)
                LatestHandler.TryGetTarget(out handler);
            else LatestPoint = point;

            if (handler is null) handler = DetectHandler(point);
            if (handler is null) return true;

            LatestHandler.SetTarget(handler);

            if (ev.Action == MotionEventActions.Down)
                HandleTouched(handler, point);

            foreach (var r in Recognizers)
                r.ProcessMotionEvent(handler, ev);

            return true;
        }

        public override bool DispatchTouchEvent(MotionEvent e)
        {
            Parent.RequestDisallowInterceptTouchEvent(true);
            return base.DispatchTouchEvent(e);
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev) => OnTouch(ev);

        public override bool OnTouchEvent(MotionEvent ev) => OnTouch(ev);

        public void AddGestureRecognizer(BaseGestureRecognizer gesture) => Recognizers.Add(gesture.Set(x => x.NativeView = this));

        class VideoControllerCallback : VideoController.VideoLifecycleCallbacks
        {
            AdmobNativeVideoView View;

            public VideoControllerCallback(AdmobNativeVideoView view)
            {
                View = view;
            }

            public override void OnVideoEnd()
            {
                View.OnVideoEnded.Raise();
                base.OnVideoEnd();
            }
        }

        class UnifiedNativeAdListener : Java.Lang.Object, UnifiedNativeAd.IOnUnifiedNativeAdLoadedListener
        {
            AdmobAndroidNativeVideoView Result;

            public UnifiedNativeAdListener(AdmobAndroidNativeVideoView result)
            {
                Result = result;
            }

            public void OnUnifiedNativeAdLoaded(UnifiedNativeAd ad)
            {
                Result.View.OnAdReady.Raise(new AdmobNativeInfo
                {
                    Headline = ad.Headline,
                    Icon = Result.ConvertDrawableToByteArray(ad.Icon?.Drawable),
                    Price = ad.Price,
                    Advertiser = ad.Advertiser,
                    Body = ad.Body,
                    StarRating = ad.StarRating?.DoubleValue(),
                    Store = ad.Store,
                    HasData = ad.Headline.OrNullIfEmpty() == null ? false : true
                });
                Result.CreateAdView(ad);
            }
        }
    }
}