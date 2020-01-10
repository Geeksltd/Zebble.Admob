using Android.Gms.Ads.Formats;
using Android.Graphics;
using Android.Graphics.Drawables;
using System;
using System.IO;

namespace Zebble.AdMob
{
    partial class NativeAdInfo
    {
        public UnifiedNativeAd Native { get; private set; }

        public NativeAdInfo(UnifiedNativeAd ad)
        {
            Native = ad;
            Headline = ad.Headline;
            Price = ad.Price;
            Advertiser = ad.Advertiser;
            Body = ad.Body;
            StarRating = ad.StarRating?.DoubleValue();
            Store = ad.Store;
            Icon = ToByteArray(ad.Icon?.Drawable);
            CallToAction = ad.CallToAction;
        }

        static byte[] ToByteArray(Drawable drawable)
        {
            if (drawable == null) return new byte[0];

            var bitmap = ((BitmapDrawable)drawable).Bitmap;
            using (var stream = new MemoryStream())
            {
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                return stream.ReadAllBytes();
            }
        }
    }
}