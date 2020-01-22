using Microsoft.Advertising.WinRT.UI;
using System;
using System.Threading.Tasks;

namespace Zebble.AdMob
{
    partial class NativeAdInfo
    {
        public NativeAdV2 Native { get; private set; }

        public NativeAdInfo(NativeAdV2 ad)
        {
            Native = ad;
            Headline = ad.Title;
            Price = ad.Price;
            Advertiser = ad.SponsoredBy;
            Body = ad.Description;
            StarRating = ad.Rating.None() ? 0 : Convert.ToDouble(ad.Rating);
            Icon = ToByteArray(ad.IconImage).Result;
            CallToAction = ad.CallToActionText;
        }

        static async Task<byte[]> ToByteArray(NativeImage img)
        {
            //if (img == null) 
                return new byte[0];

            var data = await Device.Network.Download(new Uri(img.Url));
            return data;
        }
    }
}