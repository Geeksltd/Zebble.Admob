[logo]: https://raw.githubusercontent.com/Geeksltd/Zebble.Admob/master/icon.png "Zebble.Admob"


## Zebble.Admob

![logo]

A Zebble plugin to add Google Admob to your application.


[![NuGet](https://img.shields.io/nuget/v/Zebble.Admob.svg?label=NuGet)](https://www.nuget.org/packages/Zebble.Admob/)

> This plugin enables you to show advertisement by using Google Admob in Android, UWP and iOS.

<br>


### Setup
* Available on NuGet: [https://www.nuget.org/packages/Zebble.Admob/](https://www.nuget.org/packages/Zebble.Admob/)
* Install in your platform client projects.
* Available for iOS, Android and UWP.
<br>


### Api Usage

If you want to use Admob platform on your Zebble application you need to choose your advertisement unit type and use the folowing information.

##### Banner Adv:

Banner ads occupy a spot within an app's layout, either at the top or bottom of the device screen. They stay on screen while users are interacting with the app, and can refresh automatically after a certain period of time. If you're new to mobile advertising, they're a great place to start.

```xml
<z-Component z-type="AdPage"
             z-base="Page"
             z-namespace="UI.Pages"
             Title="Ads"
             xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
             xsi:noNamespaceSchemaLocation="../.zebble-schema.xml">
  <z-place inside="Body">

        ...

        <AdmobBannerView Id="AdvBanner" BannerSize="@new Size(100,70)" />

  </z-place>
</z-Component>
```

```csharp
namespace UI.Pages
{
    partial class AdPage
    {
        public override async Task OnInitializing()
        {
            await base.OnInitializing();
            
            AdvBanner.OnAdTapped.Handle(() =>
            {
                //Your code 
            });
        }
    }
}
```

##### Native Adv:

Native ads are ad assets that are presented to users via UI components that are native to the platform. They're shown using the same classes you already use in your views, and can be formatted to match your app's visual design.

```xml
<z-Component z-type="AdPage"
             z-base="Page"
             z-namespace="UI.Pages"
             Title="Ads"
             xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
             xsi:noNamespaceSchemaLocation="../.zebble-schema.xml">
  <z-place inside="Body">

    <AdmobNativeVideoView Id="AdvVideoView">

      <Stack Direction="Horizontal">

        <ImageView Id="IconView" />
        
        <Stack>
          <TextView Id="HeadLineTxt" />
          <TextView Id="PriceTxt" />
          <TextView Id="BodyTxt" />
        </Stack>
        
      </Stack>

      <Stack>

        <AdmobMediaView Id="AdMedia" />
        
      </Stack>

    </AdmobNativeVideoView>
   

  </z-place>
</z-Component>
```

```csharp
namespace UI.Pages
{
    partial class AdPage
    {
        public override async Task OnInitializing()
        {
            await base.OnInitializing();
            
            AdvVideoView.UnitId = "Your Unit Id";
            AdvVideoView.OnAdReady.Handle(() => {

                AdvVideoView.HeadLineView = HeadLineTxt;
                AdvVideoView.PriceView = PriceTxt;
                AdvVideoView.MediaView = AdMedia;
                AdvVideoView.IconView = IconView;
                AdvVideoView.BodyView = BodyTxt;

            });

            AdvVideoView.OnAdFailed.Handle(error => Zebble.Device.Log.Error(error));
            
        }
    }
}
```

### Platform Specific Notes

Some platforms require certain settings before the adv can start work on them.

#### Android:

In Android platform you need to add this code to android manifest file like below:

```xml
<meta-data android:name="com.google.android.gms.ads.APPLICATION_ID" android:value="Your Admob applicatin code"/>
```
�
#### IOS:

In IOS platform you need to set the scheme and application URL in the �Info.plist� file like below:

```xml
<plist version="1.0">
  <dict>
    �
  <key>GADApplicationIdentifier</key>
  <string>Your Admob applicatin code</string>
  </dict>
</plist>
```