using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Duskvern
{
    /// <summary>
    /// 游戏音频模块
    /// 
    /// 职责：
    /// 1. BGM播放管理
    /// 2. 音效播放管理
    /// 3. AudioSource对象池
    /// 4. 音频资源缓存
    /// 5. 音量 / 开关控制
    /// 6. 振动接口预留
    /// </summary>
    public class AudioModule : GameModule<AudioModule>, IGameModule
    {
        #region Module Life Cycle

        public override void OnLoad() { }

        public override void OnModulePause() { }

        public override void OnModuleResume() { }

        public override void OnPreLoad() { }

        public override void OnPreUnload() { }

        public override void Unload() { }

        #endregion


        #region Runtime Data

        /// <summary>
        /// 已加载音频缓存
        /// key: 资源名
        /// value: AudioClip
        /// </summary>
        private Dictionary<string, AudioClip> clipCache = new();

        /// <summary>
        /// 当前正在播放的音效
        /// </summary>
        private List<AudioSource> playingSources = new();

        /// <summary>
        /// AudioSource对象池
        /// </summary>
        private Queue<AudioSource> sourcePool = new();

        /// <summary>
        /// BGM播放源
        /// </summary>
        public AudioSource bgmSource;

        /// <summary>
        /// 音效AudioSource预制体
        /// </summary>
        public AudioSource audioSourcePrefab;

        /// <summary>
        /// 最大同时播放音效数量
        /// </summary>
        public int maxAudioNum = 15;

        /// <summary>
        /// 音效随机音高范围
        /// </summary>
        public Vector2 pitchRange = new(0.9f, 1.1f);

        /// <summary>
        /// 当前播放的BGM名称
        /// </summary>
        private string currentBGM;

        #endregion


        #region SaveData

        public AudioSaveData audioSaveData => SaveDataModule.Instance.GetData<AudioSaveData>();

        private float MusicVolume
        {
            get => audioSaveData.musicVolume;
            set => audioSaveData.musicVolume = value;
        }

        private float SoundVolume
        {
            get => audioSaveData.sfxVolume;
            set => audioSaveData.sfxVolume = value;
        }

        private float vibrationIntensity
        {
            get => audioSaveData.vibrationIntensity;
            set => audioSaveData.vibrationIntensity = value;
        }

        private bool MusicEnable
        {
            get => audioSaveData.onMusicEnabled;
            set => audioSaveData.onMusicEnabled = value;
        }

        private bool SoundEnable
        {
            get => audioSaveData.onSFXEnabled;
            set => audioSaveData.onSFXEnabled = value;
        }

        private bool VibrationEnable
        {
            get => audioSaveData.onVibrationEnabled;
            set => audioSaveData.onVibrationEnabled = value;
        }

        #endregion


        #region Update

        /// <summary>
        /// 每帧检测音效是否播放结束
        /// 自动回收AudioSource
        /// </summary>
        void Update()
        {
            for (int i = playingSources.Count - 1; i >= 0; i--)
            {
                var source = playingSources[i];

                if (!source.isPlaying)
                {
                    RecycleSource(source);
                    playingSources.RemoveAt(i);
                }
            }
        }

        #endregion


        #region BGM

        /// <summary>
        /// 切换背景音乐
        /// </summary>
        public void ChangeBGM(string name)
        {
            if (!MusicEnable)
                return;

            if (string.IsNullOrEmpty(name))
                return;

            if (currentBGM == name)
                return;

            currentBGM = name;

            LoadClip(name, clip =>
            {
                bgmSource.clip = clip;
                bgmSource.volume = MusicVolume;
                bgmSource.loop = true;
                bgmSource.Play();
            }).Forget();
        }

        /// <summary>
        /// 停止BGM
        /// </summary>
        public void StopBGM()
        {
            bgmSource.Stop();
        }

        #endregion


        #region SFX

        /// <summary>
        /// 播放一次音效
        /// </summary>
        public void PlayOneShot(string clipName, Vector3 pos = default, bool randomPitch = true)
        {
            if (!SoundEnable)
                return;

            LoadClip(clipName, clip =>
            {
                InternalPlay(clip, pos, randomPitch);
            }).Forget();
        }

        /// <summary>
        /// 内部音效播放逻辑
        /// </summary>
        private void InternalPlay(AudioClip clip, Vector3 pos, bool randomPitch)
        {
            if (playingSources.Count >= maxAudioNum)
            {
                var oldest = playingSources[0];
                RecycleSource(oldest);
                playingSources.RemoveAt(0);
            }

            var source = GetSource();

            source.transform.position = pos;

            source.volume = SoundVolume;

            source.pitch = randomPitch
                ? Random.Range(pitchRange.x, pitchRange.y)
                : 1f;

            source.PlayOneShot(clip);

            playingSources.Add(source);
        }

        #endregion


        #region AudioSource Pool

        /// <summary>
        /// 从对象池获取AudioSource
        /// </summary>
        private AudioSource GetSource()
        {
            if (sourcePool.Count > 0)
            {
                var s = sourcePool.Dequeue();
                s.gameObject.SetActive(true);
                return s;
            }

            return Instantiate(audioSourcePrefab);
        }

        /// <summary>
        /// 回收AudioSource到对象池
        /// </summary>
        private void RecycleSource(AudioSource source)
        {
            source.Stop();
            source.gameObject.SetActive(false);
            sourcePool.Enqueue(source);
        }

        #endregion


        #region Volume Control

        /// <summary>
        /// 设置音乐音量
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            MusicVolume = volume;

            if (bgmSource != null)
                bgmSource.volume = volume;
        }

        /// <summary>
        /// 设置音效音量
        /// </summary>
        public void SetSoundVolume(float volume)
        {
            SoundVolume = volume;

            foreach (var source in playingSources)
            {
                source.volume = volume;
            }
        }

        #endregion


        #region Enable Switch

        /// <summary>
        /// 开关音乐
        /// </summary>
        public void SetMusicEnable(bool enable)
        {
            MusicEnable = enable;

            if (!enable)
                bgmSource.Stop();
            else if (!string.IsNullOrEmpty(currentBGM))
                ChangeBGM(currentBGM);
        }

        /// <summary>
        /// 开关音效
        /// </summary>
        public void SetSoundEnable(bool enable)
        {
            SoundEnable = enable;

            if (!enable)
            {
                foreach (var source in playingSources)
                {
                    RecycleSource(source);
                }

                playingSources.Clear();
            }
        }

        /// <summary>
        /// 开关振动
        /// </summary>
        public void SetVibrationEnable(bool enable)
        {
            VibrationEnable = enable;
        }

        #endregion


        #region Vibration

        /// <summary>
        /// 触发设备振动
        /// 
        /// 注意：
        /// 这里只保留接口
        /// 底层实现后续补充
        /// </summary>
        public void Vibrate(float intensity = 1f)
        {
            if (!VibrationEnable)
                return;

            float finalIntensity = intensity * vibrationIntensity;

            // TODO: 具体平台实现
            // Android / iOS vibration
        }

        #endregion


        #region Asset Loading

        /// <summary>
        /// 加载AudioClip（带缓存）
        /// </summary>
        private async UniTask LoadClip(string name, System.Action<AudioClip> callback)
        {
            if (clipCache.TryGetValue(name, out var clip))
            {
                callback?.Invoke(clip);
                return;
            }

            var loadedClip = await AssetModule.Instance.LoadAsset<AudioClip>(name);

            if (loadedClip == null)
                return;

            clipCache[name] = loadedClip;

            callback?.Invoke(loadedClip);
        }

        #endregion
    }
}