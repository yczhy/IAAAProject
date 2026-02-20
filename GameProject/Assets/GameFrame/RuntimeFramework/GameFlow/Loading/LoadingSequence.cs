using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Duskvern
{
    public class LoadingSequence
    {
        /// <summary>
        /// 该进度下所有的任务 --- 名字 + Task
        /// </summary>
        private Dictionary<string, LoadingTask> taskDict = new Dictionary<string, LoadingTask>();

        /// <summary>
        /// 具体一个任务的依赖数量 ---- 名字 + 依赖数
        /// </summary>
        private Dictionary<string, int> dependencyCount = new Dictionary<string, int>();

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, List<string>> reverseDependency = new Dictionary<string, List<string>>();

        private int totalTaskCount; // 总任务数量
        private int finishedTaskCount; // 完成的任务数量
        private bool hasFailed; // 是否失败

        public float Progress =>
            totalTaskCount == 0 ? 1f : (float)finishedTaskCount / totalTaskCount;

        public void AddTask(LoadingTask task)
        {
            if (taskDict.ContainsKey(task.TaskName)) return;

            taskDict.Add(task.TaskName, task);
        }

        public void RemoveTask(string taskName)
        {
            taskDict.Remove(taskName);
        }

        private void BuildDependencyGraph()
        {
            dependencyCount.Clear();
            reverseDependency.Clear();

            foreach (var task in taskDict.Values)
            {
                dependencyCount[task.TaskName] = task.Dependence.Count;

                foreach (var dep in task.Dependence)
                {
                    if (!reverseDependency.TryGetValue(dep, out var list))
                    {
                        list = new List<string>();
                        reverseDependency.Add(dep, list);
                    }

                    list.Add(task.TaskName);
                }
            }

            totalTaskCount = taskDict.Count;
            finishedTaskCount = 0;
            hasFailed = false;
        }

        public async UniTask<bool> Execute()
        {
            BuildDependencyGraph();

            Queue<string> readyQueue = new Queue<string>();

            foreach (var kv in dependencyCount)
            {
                if (kv.Value == 0)
                    readyQueue.Enqueue(kv.Key);
            }

            while (readyQueue.Count > 0)
            {
                if (hasFailed)
                    return false;

                List<UniTask<(string, bool)>> runningTasks = new List<UniTask<(string, bool)>>();

                int batchCount = readyQueue.Count;
                for (int i = 0; i < batchCount; i++)
                {
                    var taskName = readyQueue.Dequeue();
                    var task = taskDict[taskName];

                    runningTasks.Add(ExecuteWrapper(task));
                }

                var results = await UniTask.WhenAll(runningTasks);

                foreach (var (taskName, success) in results)
                {
                    if (!success)
                    {
                        hasFailed = true;
                        return false;
                    }

                    finishedTaskCount++;

                    if (reverseDependency.TryGetValue(taskName, out var dependents))
                    {
                        foreach (var depTask in dependents)
                        {
                            dependencyCount[depTask]--;
                            if (dependencyCount[depTask] == 0)
                                readyQueue.Enqueue(depTask);
                        }
                    }
                }
            }

            return finishedTaskCount == totalTaskCount;
        }

        private async UniTask<(string, bool)> ExecuteWrapper(LoadingTask task)
        {
            bool result = await task.ExecuteTask();
            return (task.TaskName, result);
        }

        public void Clear()
        {
            taskDict.Clear();
            dependencyCount.Clear();
            reverseDependency.Clear();
            totalTaskCount = 0;
            finishedTaskCount = 0;
            hasFailed = false;
        }
    }

    public class LoadingTaskNode
    {
        public LoadingTask Task;
        public List<LoadingTaskNode> Parents = new List<LoadingTaskNode>();
        public List<LoadingTaskNode> Children = new List<LoadingTaskNode>();

        public LoadingTaskNode(LoadingTask task)
        {
            Task = task;
        }

        public void Clear()
        {
            Task.Clear();
            Task = null;
            Parents.Clear();
            Children.Clear();
        }
    }
}