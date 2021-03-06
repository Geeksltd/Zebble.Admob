﻿using System;
using System.Threading.Tasks;
using Olive;

namespace Zebble.AdMob
{
    public partial class NativeAdView : AdmobView, IZebbleAdView, IRenderedBy<AdmobViewRenderer>
    {
        public void Rotate() => RotateRequested?.Invoke();

        public readonly Bindable<NativeAdInfo> Ad = new Bindable<NativeAdInfo>(new NativeAdInfo());
        public readonly AsyncEvent OnVideoEnded = new AsyncEvent();
        internal event Action RotateRequested;
        internal TextView Adbadge = new TextView("Ad");

        public override async Task OnInitializing()
        {
            await base.OnInitializing();

            Adbadge.Height(25).Width(25).Margin(all: 5).Border(radius: 5)
                .CssClass("AdBadge").Padding(all: 5).Font(size: 10)
                .Background(color: Colors.Orange).TextColor(Colors.White)
                .Absolute();
            await Add(Adbadge);
        }

        public AdmobTypes AdType => AdmobTypes.Native;

        public TextView HeadLineView { get; set; }
        public TextView BodyView { get; set; }
        public Button CallToActionView { get; set; }
        public ImageView IconView { get; set; }
        public TextView PriceView { get; set; }
        public TextView StoreView { get; set; }
        public TextView AdvertiserView { get; set; }
        public AdmobMediaView MediaView { get; set; }

        public AdAgent Agent { get; set; }
    }
}
