/****************************************************
	文件：TimeSvc.cs
	作者：AnderTroy
	邮箱: 1329524041@qq.com
	日期：2019/05/22 15:07   	
	功能：计时服务
*****************************************************/
using System;
using System.Collections.Generic;
using PEProtocol;

namespace Server.Net
{
    class TimeSvc
    {
        private static TimeSvc _instance;
        public static TimeSvc Instance => _instance ?? (_instance = new TimeSvc());
        private PETimer PETimer = null;
        private readonly Queue<TaskPack> taskPackQueue = new Queue<TaskPack>();
        private static readonly string queueLock = "queueLock";
        public void Init()
        {
            PETimer = new PETimer(100);
            taskPackQueue.Clear();
            PETimer.SetLog((string info) => PeRoot.Log(info));//设置日志输出
            PETimer.SetHandle((Action<int> cb, int tid) =>
            {
                if (cb != null)
                {
                    lock (queueLock)
                    {
                        taskPackQueue.Enqueue(new TaskPack(tid, cb));
                    }
                }
            });
            PeRoot.Log("TimerSvc Init Done.||计时服务.");
        }
        public void Update()
        {
            lock (queueLock)
            {
                while (taskPackQueue.Count > 0)
                {
                    TaskPack taskPack = null;
                    taskPack = taskPackQueue.Dequeue();
                    taskPack?.Action(taskPack.TimeId);
                }
            }
        }
        public int AddTimeTask(Action<int> callback, double delay, PETimeUnit timeUint = PETimeUnit.Millisecond,
            int count = 1)
        {
            return PETimer.AddTimeTask(callback, delay, timeUint, count);
        }
        public long GetNowTime()
        {
            return (long) PETimer.GetMillisecondsTime();
        }
    }
    class TaskPack
    {
        public int TimeId;
        public Action<int> Action;

        public TaskPack(int timeId, Action<int> action)
        {
            this.TimeId = timeId;
            this.Action = action;
        }
    }
}
