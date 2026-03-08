using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duskvern
{
    public class MaxAdUnit : AdUnit
    {
        private E_AdUnitSource source = E_AdUnitSource.Max; public override E_AdUnitSource Source => source;

        public override bool IsEmpty
        {
            get
            {
                foreach (var unit in _ads.Values)
                {
                    if (string.IsNullOrEmpty(unit.AdId)) return false;
                }
                return true;
            }
        }

        public override void Init()
        {
            _ads = new Dictionary<E_Adtype, IPlayUnit>
            {
                { E_Adtype.RewardAd, new Max_Reward() },
                { E_Adtype.InsertAd, new Max_Interstitial() },
                { E_Adtype.BannerAd, new Max_Banner() },
                { E_Adtype.NativeAd, new Max_Native() }
            };
        }

        public override void LoadAd(E_Adtype adtype)
        {
            if (_ads.TryGetValue(adtype, out var unit))
            {
                unit.LoadAd();
            }
        }

        public override void PlayAd(E_Adtype adtype)
        {

        }

        public override bool CanPlay(E_Adtype adtype)
        {
            if (_ads.TryGetValue(adtype, out var unit) && string.IsNullOrEmpty(unit.AdId))
            {
                return unit.CheckIsReady();
            }
            return false;
        }
    }
}

