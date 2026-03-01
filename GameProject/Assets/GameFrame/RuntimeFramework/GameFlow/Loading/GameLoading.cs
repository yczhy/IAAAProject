using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Duskvern
{


    public class GameLoading : GameModule<GameLoading>, IGameModule
    {
        [ReadOnly] public Dictionary<E_LoadingType, LoadingSequence> loadingSeqDict;

        public override void OnLoad()
        {
            if (isLoaded) return;
            isLoaded = true;
            loadingSeqDict = new();
        }

        public override void Unload()
        {
            foreach (var seq in loadingSeqDict.Values)
                seq.Clear();

            loadingSeqDict.Clear();
        }

        public override void OnPreLoad()
        {

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

        public void RegisterSequence(E_LoadingType type, LoadingSequence sequence)
        {
            loadingSeqDict[type] = sequence;
        }

        public async UniTask<bool> ExecuteSequences(params E_LoadingType[] types)
        {
            List<UniTask<bool>> tasks = new List<UniTask<bool>>();

            foreach (var type in types)
            {
                if (loadingSeqDict.TryGetValue(type, out var seq))
                    tasks.Add(seq.Execute());
            }

            bool[] results = await UniTask.WhenAll(tasks);

            foreach (var r in results)
            {
                if (!r)
                    return false;
            }

            return true;
        }

        public float GetSequenceProgress(E_LoadingType type)
        {
            if (loadingSeqDict.TryGetValue(type, out var seq))
                return seq.Progress;

            return 0f;
        }
    }

    public enum E_LoadingType
    {
        LaunchGame,
    }

}