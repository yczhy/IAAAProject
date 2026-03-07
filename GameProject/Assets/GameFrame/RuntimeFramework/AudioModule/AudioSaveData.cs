using System.Collections;
using System.Collections.Generic;
using Duskvern;
using UnityEngine;

namespace Duskvern
{
    public class AudioSaveData : ISaveData
    {
        public bool onMusicEnabled;
        public bool onSFXEnabled;
        public float musicVolume;
        public float sfxVolume;
        public bool onVibrationEnabled;
        public float vibrationIntensity;
    }
}
