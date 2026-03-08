
using System;

namespace Duskvern
{
    public enum E_AdUnitSource
    {
        Max,
    }

    public enum E_Adtype
    {
        All,
        InsertAd,
        RewardAd,
        BannerAd,
        NativeAd,
    }

    public enum E_AdPlayReason
    {

    }

    // 不同的广告源： Max 穿山甲
    public interface IAdSourceUnit
    {
        public E_AdUnitSource Source { get; }

        public bool IsEmpty { get; }

        public void LoadAd(E_Adtype adtype);

        public void PlayAd(E_Adtype adtype);

        public bool CanPlay(E_Adtype adtype);
    }

    // 广告的播放源单位 ： MAX的插屏、激励等 或者 穿山甲的插屏、激励
    public interface IPlayUnit
    {
        public string AdId { get; }

        public double ECPM { get; }

        public E_AdPlayReason Reason { get; }

        public bool CheckIsReady();

        public void LoadAd();
        public void PlayAd();
    }

    [Serializable]
    public struct AdIdParam
    {
        public string RewardId;
        public string InsertId;
        public string BannerId;
        public string NativeId;
    }

    [Serializable]
    public struct AdSourceConfig
    {
        public E_AdUnitSource Source;
        public AdIdParam IdParam;
    }
}