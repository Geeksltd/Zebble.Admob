using System;
using System.Linq;
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

        PanGestureRecognizer PanRecognizer;
        TapGestureRecognizer TapRecognizer;
        WeakReference<Zebble.View> LatestHandler = new WeakReference<Zebble.View>(null);
        Point LatestPoint;
        bool IsHandlerMine;

        [Preserve]
        public AndroidNativeAdView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        public AndroidNativeAdView(NativeAdView view) : base(Renderer.Context)
        {
            try
            {
                View = view;

                TapRecognizer = new TapGestureRecognizer { OnGestureRecognized = HandleTapped, NativeView = this };
                PanRecognizer = new PanGestureRecognizer(p => DetectHandler(p)) { NativeView = this };

                view.Panning.Handle(args =>
                {
                    CurrentAd?.Native?.CancelUnconfirmedClick();

                    var handlerParent = view.GetAllParents().FirstOrDefault(x => x?.Panning?.IsHandled() == true);
                    if (handlerParent != null)
                        handlerParent.RaisePanning(args);
                });

                AddView(NativeView = new UnifiedNativeAdView(Renderer.Context)
                {
                    LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent)
                });

                Agent = (view.Agent ?? throw new Exception(".NativeAdView.Agent is null"));

                view.RotateRequested.Handle(LoadNext);
                LoadAds().RunInParallel();
            }
            catch (Exception ex)
            {
                Device.Log.Error($"[Zebble.Admob] => {ex.Message}");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) View = null;
            base.Dispose(disposing);
        }

        async Task LoadAds()
        {
            if (Agent.Ads.None())
            {
                var ad = await Agent.GetNativeAd(View.Parameters);
                await CreateAdView(ad);
            }
            else await LoadNext();
        }

        async Task LoadNext()
        {
            var ad = Agent.Ads.FirstOrDefault(x => !x.IsShown);
            if (ad == null)
            {
                var ts = DateTime.Now.Subtract(Agent.LastUpdate).TotalSeconds;
                if (ts > Agent.WaitingToLoad.TotalSeconds)
                    LoadAds().RunInParallel();
                else
                {
                    Agent.ResetAdsList();
                    Task.Delay(Agent.WaitingToLoad).ContinueWith(t =>
                    {
                        if (t.IsCompleted)
                        {
                            Agent.Ads.Clear();
                            LoadAds().RunInParallel();
                        }
                    }).RunInParallel();
                }
            }
            else
            {
                await CreateAdView(ad);
                ad.IsShown = true;
            }
        }

        Task CreateAdView(NativeAdInfo ad)
        {
            CurrentAd = ad;
            View.Ad.Value = ad;

            if (ad is FailedNativeAdInfo)
            {
                View.HeadLineView.Text = ad.Headline;
                View.BodyView.Text = ad.Body;
                View.CallToActionView.Text = ad.CallToAction;
            }
            else
            {
                var adChoices = new AdChoicesView(Renderer.Context) { LayoutParameters = new ViewGroup.LayoutParams(25, 25) };

                NativeView.MediaView = View.MediaView?.Native() as AdmobAndroidMediaView;

                NativeView.HeadlineView = View.HeadLineView?.Native();
                NativeView.BodyView = View.BodyView?.Native();
                NativeView.CallToActionView = View.CallToActionView?.Native();
                NativeView.IconView = View.IconView?.Native();
                NativeView.PriceView = View.PriceView?.Native();
                NativeView.StoreView = View.StoreView?.Native();
                NativeView.AdvertiserView = View.AdvertiserView?.Native();
                NativeView.AdChoicesView = adChoices;

                NativeView.SetNativeAd(ad.Native);

                var vc = ad.Native.VideoController;

                if (vc.HasVideoContent && VideoCallBack == null)
                    vc.SetVideoLifecycleCallbacks(VideoCallBack = new VideoControllerCallback(View));
            }

            return Task.CompletedTask;
        }

        void HandleTapped(View handler, Point point, int touches)
        {
            Device.Keyboard.Hide();

            if (CurrentAd is FailedNativeAdInfo ad)
            {
                Device.OS.OpenBrowser(ad.TargetUrl);
            }
            else if (handler is AdmobMediaView || handler.ToString() == View.CallToActionView.ToString())
            {
                View.CallToActionView?.Native()?.PerformClick();
                point = point.RelativeTo(handler);
                handler.RaiseTapped(new Zebble.TouchEventArgs(handler, point, touches));
            }
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
            {
                PanRecognizer.ProcessMotionEvent(handler, ev);
                TapRecognizer.ProcessMotionEvent(handler, ev);
            }

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