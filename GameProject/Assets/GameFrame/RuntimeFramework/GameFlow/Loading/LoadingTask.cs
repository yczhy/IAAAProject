using System;
using System.Collections.Generic;
//using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Duskvern
{
    public class LoadingTask
    {
        private HashSet<string> dependence = new HashSet<string>();
        public IReadOnlyCollection<string> Dependence => dependence;
        private string taskName;
        public string TaskName => taskName;
        private float weight;
        public float Weight => weight;

        private Func<UniTask<bool>> task;
        private LoadingTaskInfo info;

        public void Init(LoadingTaskInfo info)
        {
            this.info = info;
            this.taskName = info.taskName;
            this.weight = info.weight;
            this.task = info.task;

            if (info.dependences != null)
            {
                foreach (var dep in info.dependences)
                {
                    if (!string.IsNullOrEmpty(dep))
                    {
                        dependence.Add(dep);
                    }
                }
            }
        }

        private bool executed = false; // 是否已经执行过
        private bool lastResult = false; // 上一次执行的结果

        /// <summary>
        /// 返回 false 表示任务失败
        /// </summary>
        public async UniTask<bool> ExecuteTask()
        {
            if (executed)
                return lastResult;

            if (task == null)
            {
                DebugLogger.LogError($"{taskName}的任务值为null", false);
                return false;
            }

            lastResult = await task();
            executed = true;
            return lastResult;
        }

        public void Clear()
        {
            dependence.Clear();
            info.Clear();
            info = null;
            task = default;
            weight = 0;
        }
    }

    public class LoadingTaskInfo
    {
        public string taskName;
        public List<string> dependences = new List<string>();
        public Func<UniTask<bool>> task;
        public float weight;

        public LoadingTaskInfo(string taskName, float weight, List<string> dependences, Func<UniTask<bool>> task)
        {
            this.taskName = taskName;
            this.dependences = dependences;
            this.task = task;
            this.weight = weight;
        }

        public void Clear()
        {
            taskName = string.Empty;
            dependences.Clear();
            task = default;
            weight = 0;
        }
    }
}