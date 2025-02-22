﻿using A.TaskDispatching;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace A.TaskDispatchingTest
{
    public static class TaskConfigExtensions
    {
        /// <summary>
        /// 从配置文件构建任务执行器(任务总调度)
        /// </summary>
        /// <param name="taskConfig"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static TaskDispatcher BuildTaskDispatcher(this TaskConfig taskConfig)
        {
            if (taskConfig.Tasks.Length == 0)
            {
                throw new ArgumentException(message: "应至少指定一个任务", paramName: nameof(taskConfig));
            }

            List<SchedulerTask> taskQueue = new List<SchedulerTask>();
            List<PrimitiveSchedulerTask> primitiveTasks = new List<PrimitiveSchedulerTask>();

            List<PrimitiveSchedulerTask> group = new List<PrimitiveSchedulerTask>();
            bool runOnPreviousCompleted = true;

            foreach (TaskItem item in taskConfig.Tasks.OrderBy(t => t.Number))
            {
                PrimitiveSchedulerTask primitiveTask = BuildTask(item, taskConfig);
                primitiveTasks.Add(primitiveTask);

                // 当本条任务需要等待前一条任务时，生成一个组.  group.Count != 0 第一条不参与分组
                if (runOnPreviousCompleted && group.Count != 0)
                {
                    // 需要等待的。 ref内部修改会传到外部
                    taskQueue.Add(BuildGroup(ref group));
                }

                group.Add(primitiveTask);
                runOnPreviousCompleted = item.RunNextOnCompleted;
            }
            // 最后一组
            taskQueue.Add(BuildGroup(ref group));

            return new TaskDispatcher(taskQueue, primitiveTasks);
        }

        private static SchedulerTask BuildGroup(ref List<PrimitiveSchedulerTask> group)
        {
            group[0].Delay = TimeSpan.FromSeconds(2);

            for (int i = 1; i < group.Count; i++)
            {
                // 后续任务延迟 5 秒启动
                group[i].Delay = TimeSpan.FromSeconds(5 * i);
            }

            SchedulerTask schedulerTask = new ParallelCompositeSchedulerTask(group);

            // 创建一个新的组
            group = new List<PrimitiveSchedulerTask>();

            return schedulerTask;
        }

        private static PrimitiveSchedulerTask BuildTask(TaskItem item, TaskConfig taskConfig)
        {
            return ConstructWorker(item).ArrangeScheduler(item.Number, item.RunNextOnFailed, null);
        }

        private static WorkerTask ConstructWorker(TaskItem item)
        {
            return new WorkerTask(
                   item.Name,
                   command: ComposeDemoPath(item.LocalPath),
                   arguments: item.Arguments
               );
        }

        // Demo
        private static string ComposeDemoPath(string localPath) =>
            Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    @"..\..\..\..\\Mock\bin\Debug\",
                    localPath
                );
    }
}
