using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace Zebble.AdMob
{
    partial class AdAgent
    {
        AdLoader Loader;

        public void Initialize()
        {
            if (Loader != null) throw new InvalidOperationException("AdAgent.Initialize() should only be called once.");

            var builder = new AdLoader.Builder(Renderer.Context, UnitId);
            builder.ForUnifiedNativeAd(new UnifiedNativeAdListener(this));

            var adOptions = new NativeAdOptions.Builder()
                    .SetVideoOptions(new VideoOptions.Builder().SetStartMuted(IsVideoMuted).Build())
                    .Build();

            builder.WithNativeAdOptions(adOptions);
            // builder.WithAdListener(new AdListener<AdmobNativeVideoView>(this));
            Loader = builder.Build();
        }

        void RequestNativeAd(AdParameters request)
        {
            var builder = new AdRequest.Builder();
            if (request.Keywords.HasValue()) builder.AddKeyword(request.Keywords);
            Loader.LoadAd(builder.Build());
        }

        class UnifiedNativeAdListener : Java.Lang.Object, UnifiedNativeAd.IOnUnifiedNativeAdLoadedListener
        {
            AdAgent Agent;

            public UnifiedNativeAdListener(AdAgent agent) => Agent = agent;

            public void OnUnifiedNativeAdLoaded(UnifiedNativeAd ad) => Agent.OnNativeAdReady(new NativeAdInfo(ad));
        }

        //class AdListener<TView> : AdListener where TView : AdmobView
        //{
        //    IZebbleAdNativeView<TView> AdView;

        //    public AdmobAndroidListener(IZebbleAdNativeView<TView> adView)
        //    {
        //        AdView = adView;


        //    }

        //    public override void OnAdClicked() => AdView.View.OnAdTapped.Raise();

        //    public override void OnAdClosed() => AdView.View.OnAdClosed.Raise();

        //    public override void OnAdLoaded() => AdView.View.OnAdLoaded.Raise();

        //    public override void OnAdFailedToLoad(int p0)
        //    {
        //        string error;
        //        AdmobAndroidListener.OnAdError(p0, out error);
        //        AdView.View.OnAdFailed.Raise(error);
        //    }

        //    public override void OnAdOpened() => AdView.View.OnAdOpened.Raise();
        //}
    }
}
