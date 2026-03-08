using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duskvern
{
    public class Max_Reward : IPlayUnit
    {
        private ChannleConfig channleConfig => ChannleConfig.Instance;

        private string adId; public string AdId => adId;
        private double ecpm; public double ECPM => ecpm;
        private E_AdPlayReason reason; public E_AdPlayReason Reason => reason;


        private string m_placement;

        public Max_Reward()
        {
            adId = channleConfig.GetAdId(E_AdUnitSource.Max, E_Adtype.RewardAd);
        }

        public bool CheckIsReady()
        {
            return MaxSdk.IsInterstitialReady(adId);
        }

        public void PlayAd()
        {
            if (CheckIsReady())
            {
                MaxSdk.ShowInterstitial(adId, m_placement);
            }
            else
            {

            }
        }

        public void LoadAd()
        {
            MaxSdk.LoadInterstitial(adId);
        }

        #region SDK广告回调

        public void InitAdCallBack()
        {
            // MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent; // 加载成功
            // MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent; // 加载失败
            // MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent; // 开始展示/展示成功

            // MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent; // 用户点击，可能发生也可能不发生
            // MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent; // 产生收入/付费回传，可能在展示后任意时刻触发，且可能多次
            // MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent; // 广告关闭/消失
            // MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent; // 展示失败 通常是在你调用 Show 之后
            // MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent; // 用户满足激励
        }

        public void OnRewardedAdLoadedEvent(string adid, MaxSdkBase.AdInfo adInfo)
        {
            
        }

        #endregion
    }
}
