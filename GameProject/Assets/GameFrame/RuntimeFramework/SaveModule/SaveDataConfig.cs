using BayatGames.SaveGameFree;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Duskvern
{
    /// <summary>
    /// SaveDataBridge 类用于提供游戏数据保存和加载的桥梁功能
    /// 支持同步和异步操作，并可选择是否加密数据
    /// </summary>
    [CreateAssetMenu(fileName = "SaveDataConfig", menuName = "Duskvern/FrameConfig/SaveDataConfig", order = 0)]
    public class SaveGameFreeConfig : ISaveDataConfig
    {
        [Header("是否加密")]
        public bool isEncrypt;

        [Header("加密密钥")]
        public string encryptKey;

        [Header("存档路径方式")]
        public SaveGamePath saveGamePath;

        [Header("存档类型")]
        public E_SaveType e_SaveType;

        public override void Init()
        {
            SaveGame.Encode = isEncrypt;
            SaveGame.EncodePassword = encryptKey;
            SaveGame.SavePath = saveGamePath;
            switch (e_SaveType)
            {
                case E_SaveType.Binary:
                    SaveGame.Serializer = new BayatGames.SaveGameFree.Serializers.SaveGameBinarySerializer();
                    break;
                case E_SaveType.Json:
                    SaveGame.Serializer = new BayatGames.SaveGameFree.Serializers.SaveGameJsonSerializer();
                    break;
                case E_SaveType.XML:
                    SaveGame.Serializer = new BayatGames.SaveGameFree.Serializers.SaveGameXmlSerializer();
                    break;
            }
        }

        public override bool HaveSave(string key)
        {
            return SaveGame.Exists(key);
        }

        public override void Save<T>(string key, T data)
        {
            SaveGame.Save(key, data);
        }

        public override T Load<T>(string key, T defaultValue = default)
        {
            if (SaveGame.Exists(key))
            {
                return SaveGame.Load<T>(key, defaultValue);
            }
            else
            {
                return defaultValue;
            }
            
        }

        public override void Delete(string key)
        {
            SaveGame.Delete(key);
        }

        public override void DeleteAll()
        {
            SaveGame.DeleteAll();
        }
    }
}