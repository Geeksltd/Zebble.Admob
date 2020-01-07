namespace Zebble
{
    enum AdmobListenerErrors
    {
        InternalError = 0,
        InvalidRequest = 1,
        NetwordError = 2,
        NoFill = 3
    }

    enum AdmobListenerRewardedError
    {
        InternalError = 0,
        AdReused = 1,
        NotReady = 2,
        AppNotForeground = 3
    }

}
