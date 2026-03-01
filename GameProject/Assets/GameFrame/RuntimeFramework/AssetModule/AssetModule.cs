using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Duskvern
{
    public class AssetModule : GameModule<AssetModule>, IGameModule
    {
        private IAssetLoader loader;

        public override void OnLoad()
        {
           
        }

        public override void Unload()
        {
        }

        public override void OnPreLoad()
        {
#if Asset_Addressable
            Init(E_AssetMode.Addressables).Forget();
#elif Asset_Resources
            Init(AssetMode.Resources);
#endif
        }

        public async UniTask Init(E_AssetMode mode)
        {
            switch (mode)
            {
                case E_AssetMode.Resources:
                    loader = new ResourcesLoader();
                    break;
                case E_AssetMode.Addressables:
                    loader = new AddressablesLoader();
                    await Addressables.InitializeAsync();
                    break;
            }
        }

        public override void OnPreUnload()
        {
        }

        public override void OnModulePause()
        {
        }

        public override void OnModuleResume()
        {
        }

        public async UniTask<T> LoadAsset<T>(string key) where T : Object
        {
            var asset = await loader.LoadAsync<T>(key);
            return asset;
        }
    }
}