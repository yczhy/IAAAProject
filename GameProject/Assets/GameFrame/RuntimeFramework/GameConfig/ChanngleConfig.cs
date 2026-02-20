using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duskvern
{
    [CreateAssetMenu(fileName = "ChanngleConfig.asset", menuName = "Duskvern/FrameConfig/ChanngleConfig")]
    public class ChanngleConfig : ScriptableObject
    {
        private static ChanngleConfig _instance;
        public static ChanngleConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    // 尝试在 Resources 里加载
                    _instance = Resources.Load<ChanngleConfig>("Configs/ChanngleConfig");
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
    }
}