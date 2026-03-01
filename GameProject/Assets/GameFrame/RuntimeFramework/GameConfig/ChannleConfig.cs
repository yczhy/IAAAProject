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
                    _instance = Resources.Load<ChannleConfig>("Configs/ChanngleConfig");
#if UNITY_EDITOR
                    if (_instance == null)
                        Debug.LogWarning("ChanngleConfig not found in Resources");
#endif
                }
                return _instance;
            }
        }

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
    }
}