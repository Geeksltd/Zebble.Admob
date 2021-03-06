﻿using System;
using controls = Windows.UI.Xaml.Controls;
using Olive;

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

            Agent = view.Agent ?? throw new Exception(".NativeAdView.Agent is null");

            view.RotateRequested += LoadNext;
            LoadNext();
        }

        public controls.Panel Render() => Result;

        void LoadNext()
        {
            Agent.Fetch().ContinueWith(t =>
            {
                if (t.IsFaulted) return;

                var ad = t.GetAlreadyCompletedResult();
                Thread.UI.Run(() => RenderAd(ad));
            });
        }

        void RenderAd(NativeAdInfo ad)
        {
            CurrentAd = ad;
            View.Ad.Value = ad;
        }
    }
}
