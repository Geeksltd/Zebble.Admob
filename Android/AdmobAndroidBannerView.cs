using Android.Gms.Ads;
using Android.Views;
using Android.Widget;

namespace Zebble
{
    class AdmobAndroidBannerView : RelativeLayout, IZebbleAdNativeView<AdmobBannerView>
    {
        public AdmobBannerView View { get; set; }
        AdView AdView;

        public AdmobAndroidBannerView(AdmobBannerView view) : base(Renderer.Context)
        {
            View = view;

            view.Paused.Handle(Pause);
            view.Resumed.Handle(Resume);

            LoadAd();
        }

        void LoadAd()
        {
            AdView = new AdView(Renderer.Context);

            if (View.BannerSize.Width != 0) AdView.AdSize = new AdSize((int)View.BannerSize.Width, (int)View.BannerSize.Height);

            AdView.AdSize = AdSize.Banner;
            AdView.AdUnitId = View.UnitId;

            var layout = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            layout.AddRule(LayoutRules.AlignParentTop);
            layout.AddRule(LayoutRules.AlignParentBottom);
            layout.AddRule(LayoutRules.AlignParentLeft);
            layout.AddRule(LayoutRules.AlignParentRight);

            AdView.LayoutParameters = layout;

            AddView(AdView);

            AdRequest request = new AdRequest.Builder().Build();
            AdView.AdListener = new AdmobAndroidListener<AdmobBannerView>(this);
            AdView.LoadAd(request);
        }

        void Pause()
        {
            if (AdView != null)
                AdView.Pause();
        }

        void Resume()
        {
            if (AdView != null)
                AdView.Resume();
        }
    }
}