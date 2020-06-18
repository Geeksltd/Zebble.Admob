using Android.Gms.Ads;
using Android.Gms.Ads.Formats;
using System;

namespace Zebble.AdMob
{
    class ZebbleAdListener : AdListener
    {
        readonly AdAgent Agent;
        public ZebbleAdListener(AdAgent agent) => Agent = agent;
        public override void OnAdFailedToLoad(int errorCode) => Agent.OnAdFailedToLoad($"Ad Loading Failed => {errorCode}");
    }
}