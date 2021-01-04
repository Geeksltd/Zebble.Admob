using Olive;

namespace Zebble.AdMob
{
    public partial class NativeAdInfo
    {
        public virtual string Headline { get; set; } = "...";
        public virtual string Price { get; set; }
        public virtual string Advertiser { get; set; }
        public virtual string Body { get; set; }
        public virtual double? StarRating { get; set; }
        public virtual string Store { get; set; }
        public virtual string CallToAction { get; set; } = "Open";
        public virtual byte[] Icon { get; set; }

        public bool HasData
        {
            get
            {
                if (Headline.None() && Body.None() && CallToAction.None()) return false;
                return true;
            }
        }
    }
}
