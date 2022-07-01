using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GersonFrame.Tool
{

    public class PETimeTask
    {
        public int tid;
        public float destTime;//单位:毫秒
        public Action callBack;
        public float delay;
        public int count;//次数

        public PETimeTask(int tid, float destTime, Action callBack, float delay, int count)
        {
            this.tid = tid;
            this.destTime = destTime;
            this.callBack = callBack;
            this.count = count;
            this.delay = delay;
        }
    }

    public class PEFrameTask
    {
        public int tid;
        public int destFrame;//单位:毫秒
        public Action callBack;
        public int delay;
        public int count;//次数

        public PEFrameTask(int tid, int destFrame, Action callBack, int delay, int count)
        {
            this.tid = tid;
            this.destFrame = destFrame;
            this.callBack = callBack;
            this.count = count;
            this.delay = delay;
        }
    }

    public enum EPETimeUnit
    {
        Millisecond = 0,
        Second,
        Minute,
        Hour,
        Day
    }
    /// <summary>
    /// 支持时间定时，帧定时
    /// 定时任务可循环 取消 替换
    /// </summary>
    public class TimeInvoke : MonoBehaviour
    {
        //单例
        public static TimeInvoke Instance;
        //声明锁
        static readonly string obj = "lock";
        int tid;
        List<int> tids = new List<int>();
        /// <summary>
        /// tid缓存回收
        /// </summary>
        List<int> recTids = new List<int>();
        /// <summary>
        /// 临时列表 支持多线程操作 错开时间操作 避免使用锁 提升操作效率 
        /// </summary>
        List<PETimeTask> tmpTimes = new List<PETimeTask>();
        List<PETimeTask> taskTimes = new List<PETimeTask>();

        int frameCounter;
        List<PEFrameTask> tmpFrames = new List<PEFrameTask>();
        List<PEFrameTask> taskFrames = new List<PEFrameTask>();

        private void Awake()
        {
            Instance = this;
        }


        void Update()
        {
            CheckTimeTask();
            CheckFrameTask();
            if (recTids.Count > 0)
            {
                RecycleTid();
            }
        }

        void CheckTimeTask()
        {
            //加入缓存区中的定时任务
            for (int i = 0; i < tmpTimes.Count; i++)
            {
                taskTimes.Add(tmpTimes[i]);
            }

            tmpTimes.Clear();
            //遍历检测任务是否到达条件
            for (int i = 0; i < taskTimes.Count; i++)
            {
                PETimeTask task = taskTimes[i];
                if (Time.realtimeSinceStartup * 1000 < task.destTime)
                {
                    continue;
                }
                else
                {
                    try
                    {
                        //时间到 callBack不为空调用
                        task.callBack?.Invoke();
                    }
                    catch (Exception e)
                    {
                        MyDebuger.Log(e.ToString());
                    }
                    if (task.count == 1)
                    {
                        if (taskTimes.Count > i)
                            taskTimes.RemoveAt(i);
                        i--;
                        recTids.Add(task.tid);
                    }
                    else
                    {
                        if (task.count != 0)
                        {
                            task.count -= 1;
                        }
                        //重新赋值时间
                        task.destTime += task.delay * Time.timeScale;
                    }
                }

            }
        }
        void CheckFrameTask()
        {
            //加入缓存区中的定时任务
            for (int i = 0; i < tmpFrames.Count; i++)
            {
                taskFrames.Add(tmpFrames[i]);
            }

            tmpFrames.Clear();

            frameCounter += 1;
            //遍历检测任务是否到达条件
            for (int i = 0; i < taskFrames.Count; i++)
            {
                PEFrameTask task = taskFrames[i];
                if (frameCounter < task.destFrame)
                {
                    continue;
                }
                else
                {
                    try
                    {
                        //时间到 callBack不为空调用
                        task.callBack?.Invoke();
                    }
                    catch (Exception e)
                    {
                        MyDebuger.Log(e.ToString());
                    }

                    if (task.count == 1)
                    {
                        taskFrames.RemoveAt(i);
                        i--;
                        recTids.Add(task.tid);
                    }
                    else
                    {
                        if (task.count != 0)
                        {
                            task.count -= 1;
                        }
                        //重新赋值时间
                        task.destFrame += task.delay;
                    }

                }

            }
        }


        #region TimeTask
        /// <summary>
        /// 添加一个计时器
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="delay"></param>
        /// <param name="count"></param>
        /// <param name="timeUnit"></param>
        /// <returns></returns>
        public int AddTimeTask(Action callBack, float delay, EPETimeUnit timeUnit = EPETimeUnit.Second, int count = 1)
        {
            //时间单位换算 最小毫秒
            if (timeUnit != EPETimeUnit.Millisecond)
            {
                switch (timeUnit)
                {
                    case EPETimeUnit.Second:
                        delay = delay * 1000;
                        break;
                    case EPETimeUnit.Minute:
                        delay = delay * 1000 * 60;
                        break;
                    case EPETimeUnit.Hour:
                        delay = delay * 1000 * 60 * 60;
                        break;
                    case EPETimeUnit.Day:
                        delay = delay * 1000 * 60 * 60 * 24;
                        break;
                    default:
                        MyDebuger.Log("Add Task TimeUnit Type error");
                        break;
                }
            }
            int tid = GetTid();
            //从游戏开始到现在的时间
            float destTime = Time.realtimeSinceStartup * 1000 + delay;

            tmpTimes.Add(new PETimeTask(tid, destTime, callBack, delay, count));
            tids.Add(tid);
            return tid;
        }
        /// <summary>
        /// 移除一个计时器
        /// </summary>
        /// <param name="tid"></param>
        /// <returns></returns>
        public bool DeleteTimeTask(int tid)
        {
            bool exist = false;

            for (int i = 0; i < taskTimes.Count; i++)
            {
                PETimeTask task = taskTimes[i];
                if (task.tid == tid)
                {
                    taskTimes.RemoveAt(i);
                    for (int j = 0; j < tids.Count; j++)
                    {
                        if (tids[j] == tid)
                        {
                            tids.RemoveAt(j);
                            break;
                        }
                    }
                    exist = true;
                    break;
                }
            }

            if (!exist)
            {
                for (int i = 0; i < tmpTimes.Count; i++)
                {
                    PETimeTask task = tmpTimes[i];
                    if (task.tid == tid)
                    {
                        tmpTimes.RemoveAt(i);

                        for (int j = 0; j < tids.Count; j++)
                        {
                            if (tids[j] == tid)
                            {
                                tids.RemoveAt(j);
                                break;
                            }
                        }
                        exist = true;
                        break;
                    }
                }
            }
            return exist;
        }
        /// <summary>
        /// 替换
        /// </summary>
        /// <param name="tid"></param>
        /// <param name="callBack"></param>
        /// <param name="delay"></param>
        /// <param name="count"></param>
        /// <param name="timeUnit"></param>
        /// <returns></returns>
        public bool ReplaceTimeTask(int tid, Action callBack, float delay, int count = 1, EPETimeUnit timeUnit = EPETimeUnit.Millisecond)
        {
            //时间单位换算 最小毫秒
            if (timeUnit != EPETimeUnit.Millisecond)
            {
                switch (timeUnit)
                {
                    case EPETimeUnit.Second:
                        delay = delay * 1000;
                        break;
                    case EPETimeUnit.Minute:
                        delay = delay * 1000 * 60;
                        break;
                    case EPETimeUnit.Hour:
                        delay = delay * 1000 * 60 * 60;
                        break;
                    case EPETimeUnit.Day:
                        delay = delay * 1000 * 60 * 60 * 24;
                        break;
                    default:
                        MyDebuger.Log("Add Task TimeUnit Type error");
                        break;
                }
            }
            //从游戏开始到现在的时间
            float destTime = Time.realtimeSinceStartup * 1000 + delay;
            PETimeTask newTask = new PETimeTask(tid, destTime, callBack, delay, count);

            bool isRep = false;
            for (int i = 0; i < taskTimes.Count; i++)
            {
                if (taskTimes[i].tid == tid)
                {
                    taskTimes[i] = newTask;
                    isRep = true;
                    break;
                }
            }

            if (!isRep)
            {
                for (int i = 0; i < tmpTimes.Count; i++)
                {
                    if (tmpTimes[i].tid == tid)
                    {
                        tmpTimes[i] = newTask;
                        isRep = true;
                        break;
                    }
                }
            }
            return isRep;
        }
        #endregion
        #region FrameTask
        /// <summary>
        /// 添加一个帧计时器
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="delay"></param>
        /// <param name="count"></param>
        /// <param name="timeUnit"></param>
        /// <returns></returns>
        public int AddFrameTask(Action callBack, int delay, int count = 1)
        {
            int tid = GetTid();
            taskFrames.Add(new PEFrameTask(tid, frameCounter + delay, callBack, delay, count));
            tids.Add(tid);
            return tid;
        }
        /// <summary>
        /// 移除一个帧计时器
        /// </summary>
        /// <param name="tid"></param>
        /// <returns></returns>
        public bool DeleteFrameTask(int tid)
        {
            bool exist = false;

            for (int i = 0; i < taskFrames.Count; i++)
            {
                PEFrameTask task = taskFrames[i];
                if (task.tid == tid)
                {
                    taskFrames.RemoveAt(i);
                    for (int j = 0; j < tids.Count; j++)
                    {
                        if (tids[j] == tid)
                        {
                            tids.RemoveAt(j);
                            break;
                        }
                    }
                    exist = true;
                    break;
                }
            }

            if (!exist)
            {
                for (int i = 0; i < tmpFrames.Count; i++)
                {
                    PEFrameTask task = tmpFrames[i];
                    if (task.tid == tid)
                    {
                        tmpFrames.RemoveAt(i);

                        for (int j = 0; j < tids.Count; j++)
                        {
                            if (tids[j] == tid)
                            {
                                tids.RemoveAt(j);
                                break;
                            }
                        }
                        exist = true;
                        break;
                    }
                }
            }
            return exist;
        }
        /// <summary>
        /// 替换帧计时器
        /// </summary>
        /// <param name="tid"></param>
        /// <param name="callBack"></param>
        /// <param name="delay"></param>
        /// <param name="count"></param>
        /// <param name="timeUnit"></param>
        /// <returns></returns>
        public bool ReplaceFrameTask(int tid, Action callBack, int delay, int count = 1)
        {
            PEFrameTask newTask = new PEFrameTask(tid, frameCounter + delay, callBack, delay, count);

            bool isRep = false;
            for (int i = 0; i < taskFrames.Count; i++)
            {
                if (taskFrames[i].tid == tid)
                {
                    taskFrames[i] = newTask;
                    isRep = true;
                    break;
                }
            }

            if (!isRep)
            {
                for (int i = 0; i < tmpFrames.Count; i++)
                {
                    if (tmpFrames[i].tid == tid)
                    {
                        tmpFrames[i] = newTask;
                        isRep = true;
                        break;
                    }
                }
            }
            return isRep;
        }
        #endregion

        #region Tool Methonds
        public int GetTid()
        {
            lock (obj)
            {
                tid += 1;
                //安全代码，以防万一（服务器）
                while (true)
                {
                    if (tid == int.MaxValue)
                    {
                        tid = 0;
                    }

                    //最后一个归0后从新赋值唯一id
                    bool used = false;
                    for (int i = 0; i < tids.Count; i++)
                    {
                        if (tid == tids[i])
                        {
                            used = true;
                            break;
                        }
                    }
                    if (!used)
                    {
                        break;
                    }
                    else
                    {
                        tid += 1;
                    }
                }
            }
            return tid;
        }
        /// <summary>
        /// tid回收
        /// </summary>
        void RecycleTid()
        {
            for (int i = 0; i < recTids.Count; i++)
            {
                int tid = recTids[i];

                for (int j = 0; j < tids.Count; j++)
                {
                    if (tids[j] == tid)
                    {
                        tids.RemoveAt(j);
                        break;
                    }
                }
            }
            recTids.Clear();
        }
        #endregion



        public void ClearAllTimeTask()
        {
            tids.Clear();
            recTids.Clear();
            tmpTimes.Clear();
            taskTimes.Clear();
            tmpFrames.Clear();
            taskFrames.Clear();
        }


        /// <summary>
        /// 暂停时间
        /// </summary>
        /// <param name="frame"></param>
        public void PasuseTime(int frame, float timescal = 0)
        {
            this.StartCoroutine(PuseFrame(frame, timescal));
        }

        IEnumerator PuseFrame(int frame, float timescal)
        {
            float during = frame / 60f;
            Time.timeScale = timescal;
            yield return new WaitForSecondsRealtime(during);
            Time.timeScale = 1;
        }



    }
}