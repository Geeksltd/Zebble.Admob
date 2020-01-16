using System;
using System.Collections.Concurrent;
using Android.Gms.Ads;
using Android.Gms.Ads.Formats;
using Android.Widget;
using Zebble.AndroidOS;
using Android.Views;
using System.Threading.Tasks;
using Android.Runtime;

namespace Zebble.AdMob
{
    [Preserve]
    class AndroidNativeAdView : FrameLayout, IZebbleAdNativeView<NativeAdView>
    {
        public NativeAdView View { get; set; }
        UnifiedNativeAdView NativeView;
        NativeAdInfo CurrentAd;
        AdAgent Agent;
        VideoControllerCallback VideoCallBack;

        ConcurrentList<BaseGestureRecognizer> Recognizers = new ConcurrentList<BaseGestureRecognizer>();
        WeakReference<Zebble.View> LatestHandler = new WeakReference<Zebble.View>(null);
        Point LatestPoint;
        bool IsHandlerMine;

        [Preserve]
        public AndroidNativeAdView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        public AndroidNativeAdView(NativeAdView view) : base(Renderer.Context)
        {
            View = view;

            Recognizers.Add(new TapGestureRecognizer { OnGestureRecognized = HandleTapped, NativeView = this });
            Recognizers.Add(new PanGestureRecognizer(p => DetectHandler(p)) { NativeView = this });

            AddView(NativeView = new UnifiedNativeAdView(Renderer.Context)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent)
            });

            Agent = (view.Agent ?? throw new Exception(".NativeAdView.Agent is null"));

            view.RotateRequested.Handle(LoadNext);
            LoadNext().RunInParallel();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) View = null;
            base.Dispose(disposing);
        }

        async Task LoadNext()
        {
            var ad = await Agent.GetNativeAd(View.Parameters);
            CreateAdView(ad);
        }

        void CreateAdView(NativeAdInfo ad)
        {
            CurrentAd = ad;
            View.Ad.Value = ad;

            NativeView.MediaView = View.MediaView?.Native() as AdmobAndroidMediaView;
            NativeView.HeadlineView = View.HeadLineView?.Native();
            NativeView.BodyView = View.BodyView?.Native();
            NativeView.CallToActionView = View.CallToActionView?.Native();
            NativeView.IconView = View.IconView?.Native();
            NativeView.PriceView = View.PriceView?.Native();
            NativeView.StoreView = View.StoreView?.Native();
            NativeView.AdvertiserView = View.AdvertiserView?.Native();

            NativeView.SetNativeAd(ad.Native);

            var vc = ad.Native.VideoController;

            if (vc.HasVideoContent && VideoCallBack == null)
                vc.SetVideoLifecycleCallbacks(VideoCallBack = new VideoControllerCallback(View));
        }

        void HandleTapped(View handler, Point point, int touches)
        {
            Device.Keyboard.Hide();

            View.CallToActionView?.Native()?.PerformClick();

            point = point.RelativeTo(handler);
            handler.RaiseTapped(new Zebble.TouchEventArgs(handler, point, touches));
        }

        View DetectHandler(Point point)
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

            if (IsHandlerInAdView(handler))
                foreach (var r in Recognizers)
                    r.ProcessMotionEvent(handler, ev);

            return true;
        }

        bool IsHandlerInAdView(View handler)
        {
            if (View.AllDescendents().ContainsAny(new View[] { handler })) return IsHandlerMine = true;
            return IsHandlerMine = false;
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            base.OnInterceptTouchEvent(ev);
            return OnTouch(ev);
        }

        public override bool OnTouchEvent(MotionEvent ev)
        {
            if (IsHandlerMine) return OnTouch(ev);
            else return base.OnTouchEvent(ev);
        }

        class VideoControllerCallback : VideoController.VideoLifecycleCallbacks
        {
            NativeAdView View;

            public VideoControllerCallback(NativeAdView view)
            {
                View = view;
            }

            public override void OnVideoEnd()
            {
                View.OnVideoEnded.Raise();
                base.OnVideoEnd();
            }
        }
    }
}