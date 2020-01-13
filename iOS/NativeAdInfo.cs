﻿using Foundation;
using Google.MobileAds;
using System;
using System.IO;
using UIKit;

namespace Zebble.AdMob
{
    partial class NativeAdInfo
    {
        public UnifiedNativeAd Native { get; private set; }

        public NativeAdInfo(UnifiedNativeAd ad)
        {
            Native = ad;
            Headline = ad.Headline;
            Icon = ConvertUIImageToByteArray(ad.Icon?.Image);
            Price = ad.Price;
            Advertiser = ad.Advertiser;
            Body = ad.Body;
            StarRating = (double?)(ad.StarRating ?? null);
            Store = ad.Store;
            CallToAction = ad.CallToAction;
        }

        byte[] ConvertUIImageToByteArray(UIImage img)
        {
            if (img == null) return new byte[0];

            using (NSData imageData = img.AsPNG())
            {
                var image = new byte[imageData.Length];
                System.Runtime.InteropServices.Marshal.Copy(imageData.Bytes, image, 0, Convert.ToInt32(imageData.Length));
                return image;
            }
        }
    }
}