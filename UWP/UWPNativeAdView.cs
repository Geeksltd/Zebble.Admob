﻿using System;
using System.Threading.Tasks;
using controls = Windows.UI.Xaml.Controls;

namespace Zebble.AdMob
{
    public class UWPNativeAdView : IZebbleAdNativeView<NativeAdView>
    {
        public NativeAdView View { get; set; }
        controls.Canvas Result;
        NativeAdInfo CurrentAd;
        AdAgent Agent;

        public UWPNativeAdView(NativeAdView view)
        {
            View = view;

            Result = new controls.Canvas();

            Agent = (view.Agent ?? throw new Exception(".NativeAdView.Agent is null"));

            view.RotateRequested.Handle(LoadNext);
            LoadNext().RunInParallel();
        }

        public controls.Panel Render() => Result;

        async Task LoadNext()
        {
            var ad = await Agent.GetNativeAd(View.Parameters);
            CreateAdView(ad);
        }

        void CreateAdView(NativeAdInfo ad)
        {
            CurrentAd = ad;
            View.Ad.Value = ad;

            if (ad.Native.MainImages.Count > 0)
            {
                var mainImage = ad.Native.MainImages[0];
                var mainImageImage = ((View.MediaView.Native() as controls.Border)?.Child as AdmobUWPMediaView);
                if (mainImageImage != null) mainImageImage.SetImage(mainImage);
            }

            ad.Native.RegisterAdContainer(Result);
        }
    }
}