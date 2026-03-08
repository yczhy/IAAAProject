using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duskvern
{
    [CreateAssetMenu(fileName = "ChannleConfig.asset", menuName = "Duskvern/FrameConfig/ChannleConfig")]
    public class ChannleConfig : ScriptableObject
    {
        private static ChannleConfig _instance;
        public static ChannleConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    // 尝试在 Resources 里加载
                    _instance = Resources.Load<ChannleConfig>("Configs/ChannleConfig");
#if UNITY_EDITOR
                    if (_instance == null)
                        Debug.LogWarning("ChanngleConfig not found in Resources");
#endif
                }
                return _instance;
            }
        }


        #region 广告

        public List<AdSourceConfig> AdSourceConfigs = new List<AdSourceConfig>();


        public string GetAdId(E_AdUnitSource source, E_Adtype adType)
        {
            string adId = "";
            foreach (var cfg in AdSourceConfigs)
            {
                if (cfg.Source != source) continue;
                adId = adType switch
                {
                    E_Adtype.InsertAd => cfg.IdParam.InsertId,
                    E_Adtype.RewardAd => cfg.IdParam.RewardId,
                    E_Adtype.BannerAd => cfg.IdParam.BannerId,
                    E_Adtype.NativeAd => cfg.IdParam.NativeId,
                    _ => "",
                };
                break;
            }
            if (string.IsNullOrEmpty(adId))
            {
                DebugLogger.LogError($"广告Id没有配置：源 {source} --- 类型 {adType}");
            }
            return adId;
        }

        #endregion

        #region GM开关

        [SerializeField] private bool openGM;

        public bool OpenGM
        {
            get
            {
#if UNITY_EDITOR
                openGM = true;
#endif
                return openGM;
            }
        }

        [SerializeField] private bool openLog;
        public bool OpenLog
        {
            get
            {
#if UNITY_EDITOR
                openLog = true;
#endif
                return openLog;
            }
        }

        #endregion

    }
}