using GersonFrame.Tool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GersonFrame
{



    public static class CoroutineExtension
    {
        /// <summary>
        ///扩展mono开启协程方法
        /// </summary>
        /// <param name="mono"></param>
        /// <param name="runner"></param>
        /// <returns></returns>
        public static CoroutineController StartMyCoroutine(this MonoBehaviour mono, IEnumerator runner)
        {
            CoroutineController controller = new CoroutineController(mono, runner);
            controller.Start();
            return controller;
        }
    }


    public class CoroutineController
    {
        private static int s_id;
        public static int Id
        {
            get { return s_id; }
            private set { s_id = value; }
        }

        private MonoBehaviour m_mono;

        private MyCoroutine m_myCoroutine;

        private IEnumerator m_runner;
        private Coroutine m_coroutine;


        public CoroutineController(MonoBehaviour mono, IEnumerator runner)
        {
            Id = s_id++;
            this.m_myCoroutine = new MyCoroutine();
            this.m_mono = mono;
            this.m_runner = runner;
        }


        /// <summary>
        /// 更新运行协程
        /// </summary>
        /// <param name="runner"></param>
        /// <returns></returns>
        public bool UpdateIErunner(IEnumerator runner)
        {
            this.m_runner = runner;
            if (this.m_myCoroutine?.m_State == MyCoroutine.CoroutineState.Stop)
                return true;
            else
            {
                MyDebuger.LogWarning("请等待当前协程停止后 再替换运行的协程");
                return false;
            }
        }

        /// <summary>
        /// 启动协程
        /// </summary>
        public void Start()
        {
            this.m_myCoroutine.m_State = MyCoroutine.CoroutineState.Running;
            this.m_coroutine = m_mono.StartCoroutine(this.m_myCoroutine.HelperIE(m_runner));
        }

        /// <summary>
        /// 暂停协程
        /// </summary>
        public void Pause()
        {
            this.m_myCoroutine.m_State = MyCoroutine.CoroutineState.Pause;

        }

        /// <summary>
        /// 恢复协程
        /// </summary>
        public void Resume()
        {
            this.m_myCoroutine.m_State = MyCoroutine.CoroutineState.Running;
        }

        /// <summary>
        ///停止协程
        /// </summary>
        public void Stop()
        {
            this.m_myCoroutine.m_State = MyCoroutine.CoroutineState.Stop;
            if (m_coroutine != null)
                m_mono.StopCoroutine(this.m_coroutine);

        }

        /// <summary>
        /// 重启协程
        /// </summary>
        public void Restart()
        {
            Stop();
            Start();
        }

        /// <summary>
        /// 获取当前状态
        /// </summary>
        /// <returns></returns>
        public MyCoroutine.CoroutineState RunState => this.m_myCoroutine.m_State;


        public void Destoy()
        {
            this.Stop();
            this.m_runner = null;
            this.m_mono = null;
            this.m_myCoroutine = null;
            this.m_coroutine = null;
        }
    }





    public class MyCoroutine
    {

        public enum CoroutineState
        {
            Waitting,
            Running,
            Pause,
            Stop
        }

        public CoroutineState m_State = CoroutineState.Stop;

        /// <summary>
        /// 流程控制
        /// </summary>
        /// <returns></returns>
        public IEnumerator HelperIE(IEnumerator runnner)
        {
            while (m_State == CoroutineState.Waitting) 
                yield return null;
            while (m_State == CoroutineState.Running)
            {
                while (m_State == CoroutineState.Pause) 
                    yield return null;
                if (runnner != null && runnner.MoveNext())
                    yield return runnner.Current;
                else 
                    m_State = CoroutineState.Stop;
                while (m_State == CoroutineState.Pause) 
                    yield return null;
            }
            MyDebuger.Log("Coroutine over");
        }

    }
}