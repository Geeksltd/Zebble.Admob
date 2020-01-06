namespace Zebble
{
    public class AdmobNativeInfo
    {
        public bool HasData { get; set; }
        public string Headline { get; internal set; }
        public byte[] Icon { get; internal set; }
        public string Price { get; internal set; }
        public string Advertiser { get; internal set; }
        public string Body { get; internal set; }
        public double? StarRating { get; internal set; }
        public string Store { get; internal set; }
    }
}
