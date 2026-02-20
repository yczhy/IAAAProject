using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace Duskvern
{
    public class GameLoadingTest : MonoBehaviour
    {
        private async void Start()
        {
            // 创建一个加载序列
            var sequence = new LoadingSequence();

            // Task A
            var taskA = new LoadingTask();
            taskA.Init(new LoadingTaskInfo(
                "TaskA",
                0.2f,
                new List<string>(), // 无依赖
                async () =>
                {
                    Debug.Log("TaskA start");
                    await UniTask.Delay(500);
                    Debug.Log("TaskA done");
                    return true;
                }
            ));
            sequence.AddTask(taskA);

            // Task B
            var taskB = new LoadingTask();
            taskB.Init(new LoadingTaskInfo(
                "TaskB",
                0.5f,
                new List<string>(), // 无依赖
                async () =>
                {
                    Debug.Log("TaskB start");
                    await UniTask.Delay(700);
                    Debug.Log("TaskB done");
                    return true;
                }
            ));
            sequence.AddTask(taskB);

            // Task C，依赖 A 和 B
            var taskC = new LoadingTask();
            taskC.Init(new LoadingTaskInfo(
                "TaskC",
                0.3f,
                new List<string> { "TaskA", "TaskB" }, // 依赖 A、B
                async () =>
                {
                    Debug.Log("TaskC start");
                    await UniTask.Delay(300);
                    Debug.Log("TaskC done");
                    return true;
                }
            ));
            sequence.AddTask(taskC);

            // 注册到 GameLoading 单例
            GameLoading.Instance.RegisterSequence(E_LoadingType.LaunchGame, sequence);

            // 执行加载序列并监控进度
            await ExecuteTestSequence(E_LoadingType.LaunchGame);
        }

        private async UniTask ExecuteTestSequence(E_LoadingType type)
        {
            var seq = GameLoading.Instance.loadingSeqDict[type];

            Debug.Log("Loading start");

            // 并行打印进度
            var progressTask = UniTask.RunOnThreadPool(async () =>
            {
                while (seq.Progress < 1f)
                {
                    Debug.Log($"Progress: {seq.Progress:P}");
                    await UniTask.Delay(100);
                }
            });

            // 执行加载序列
            bool result = await seq.Execute();

            await progressTask;

            Debug.Log(result ? "Loading complete" : "Loading failed");
        }
    }
}