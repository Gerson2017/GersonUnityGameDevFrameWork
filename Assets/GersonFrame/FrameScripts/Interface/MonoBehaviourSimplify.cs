
using GersonFrame.ABFrame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GersonFrame
{
    public abstract partial class MonoBehaviourSimplify : MonoBehaviour
    {
        #region Delay
        public void Delay(float seconds, Action onFinished)
        {
            if (gameObject.activeInHierarchy)
                StartCoroutine(DelayCoroutine(seconds, onFinished));
            else 
                MyDebuger.LogWarning("Delay startfail "+this.gameObject.name);
        }



        public void Delay(float seconds, Action<object> onFinishe, object param = null)
        {
            if (gameObject.activeInHierarchy)
                StartCoroutine(DelayCoroutine(seconds, onFinishe, param));
            else
                MyDebuger.LogWarning("Delay startfail " + this.gameObject.name);
        }


        public void Delay(float seconds, Action<object, object, object> onFinished, object param = null, object param1 = null, object param2 = null)
        {
            if (gameObject.activeInHierarchy)
                StartCoroutine(DelayCoroutine(seconds, onFinished, param, param1, param2));
            else
                MyDebuger.LogWarning("Delay startfail " + this.gameObject.name);
        }

        public void DelayFrame(int framecount, Action onFinished)
        {
            if (gameObject.activeInHierarchy)
                StartCoroutine(DelayFrameCorotinue(framecount, onFinished));
            else
                MyDebuger.LogWarning("DelayFrame startfail " + this.gameObject.name);
        }

        public void DelayFrame(int framecount, Action<object> onFinished, object param = null)
        {
            if (gameObject.activeInHierarchy)
                StartCoroutine(DelayFrameCorotinue(framecount, onFinished,param));
            else
                MyDebuger.LogWarning("DelayFrame startfail " + this.gameObject.name);
        }


        public void DelayFrame(int framecount, Action<object, object,object> onFinished, object param = null, object param1 = null, object param2 = null)
        {
            if (gameObject.activeInHierarchy)
                StartCoroutine(DelayFrameCorotinue(framecount, onFinished, param, param1, param2));
            else
                MyDebuger.LogWarning("DelayFrame startfail " + this.gameObject.name);
        }

        private IEnumerator DelayCoroutine(float seconds, Action onFinished)
        {
            yield return new WaitForSeconds(seconds);
            onFinished();
        }

        private IEnumerator DelayCoroutine(float seconds, Action<object> onFinished, object param = null)
        {
            yield return new WaitForSeconds(seconds);
            onFinished(param);
        }

        private IEnumerator DelayCoroutine(float seconds, Action<object, object, object> onFinished, object param = null, object param1= null,object param2 = null)
        {
            yield return new WaitForSeconds(seconds);
            onFinished(param, param1,param2);
        }

        #endregion



        private IEnumerator DelayFrameCorotinue(int framecount, Action<object> onFinished, object param = null)
        {
            for (int i = 0; i < framecount; i++)
            {
                yield return null;
            }
            onFinished(param);
        }
        private IEnumerator DelayFrameCorotinue(int framecount, Action onFinished)
        {
            for (int i = 0; i < framecount; i++)
            {
                yield return null;
            }
            onFinished();
        }
        private IEnumerator DelayFrameCorotinue(int framecount, Action<object, object, object> onFinished, object param = null, object param1 = null, object param2 = null)
        {
            for (int i = 0; i < framecount; i++)
            {
                yield return null;
            }
            onFinished(param, param1, param2);
        }

        protected abstract void OnBeforeDestroy();

        #region MsgDispatcher
        List<MsgRecord> mMsgRecorder = new List<MsgRecord>();

        class MsgRecord
        {
            private MsgRecord() { }

            static Stack<MsgRecord> mMsgRecordPool = new Stack<MsgRecord>();

            /// <summary>
            /// 根据消息类型获取消息对象
            /// </summary>
            /// <param name="msgtype"></param>
            /// <param name="onMsgReceived"></param>
            /// <returns></returns>
            public static MsgRecord Allocate(MsgType msgtype, Action<object, object, object> onMsgReceived)
            {
                var retRecord = mMsgRecordPool.Count > 0 ? mMsgRecordPool.Pop() : new MsgRecord();
                retRecord.Msgtype = msgtype;
                retRecord.MsgName = null;
                retRecord.OnMsgReceived = onMsgReceived;
                return retRecord;
            }

            /// <summary>
            /// 根据字符串获得消息对象
            /// </summary>
            /// <param name="msgName"></param>
            /// <param name="onMsgReceived"></param>
            /// <returns></returns>
            public static MsgRecord Allocate(string msgName, Action<object, object, object> onMsgReceived)
            {
                var retRecord = mMsgRecordPool.Count > 0 ? mMsgRecordPool.Pop() : new MsgRecord();
                retRecord.Msgtype = MsgType.None;
                retRecord.MsgName = msgName;
                retRecord.OnMsgReceived = onMsgReceived;
                return retRecord;
            }

            public void Recycle()
            {
                Msgtype = MsgType.None;
                MsgName = null;
                OnMsgReceived = null;
                mMsgRecordPool.Push(this);
            }

            public MsgType Msgtype;
            public string MsgName = null;
            public Action<object, object, object> OnMsgReceived;

        }

        /// <summary>
        /// 根据消息类型注册消息
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="onMsgReceived"></param>
        public void RegisterMsg(MsgType msgName, Action<object, object, object> onMsgReceived)
        {
            MsgDispatcher.Register(msgName, onMsgReceived);
            mMsgRecorder.Add(MsgRecord.Allocate(msgName, onMsgReceived));
        }

        /// <summary>
        /// 根据字符串注册消息
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="onMsgReceived"></param>
        public void RegisterMsg(string msgName, Action<object, object, object> onMsgReceived)
        {
            MsgDispatcher.Register(msgName, onMsgReceived);
            mMsgRecorder.Add(MsgRecord.Allocate(msgName, onMsgReceived));
        }

        /// <summary>
        /// 根据消息类型发送消息
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="data"></param>
        public void SendMsg(MsgType msgName, object data1 = null, object data2 = null, object data3 = null)
        {
            MsgDispatcher.Send(msgName, data1, data2, data3);
        }

        /// <summary>
        /// 根据字符串发送消息
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="data"></param>
        public void SendMsg(string msgName, object data1 = null, object data2 = null, object data3 = null)
        {
            MsgDispatcher.Send(msgName, data1, data2, data3);
        }

        /// <summary>
        /// 根据消息类型卸载所有消息
        /// </summary>
        /// <param name="msgName"></param>
        public void UnRegisterMsg(MsgType msgName)
        {
            var selectedRecords = mMsgRecorder.FindAll(record => record.Msgtype == msgName);
            selectedRecords.ForEach(record =>
            {
                MsgDispatcher.UnRegister(record.Msgtype, record.OnMsgReceived);
                mMsgRecorder.Remove(record);
                record.Recycle();
            });
            selectedRecords.Clear();
        }

        /// <summary>
        /// 根据消息类型卸载指定消息
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="onMsgReceived"></param>
        public void UnRegisterMsg(MsgType msgName, Action<object, object, object> onMsgReceived)
        {
            var selectedRecords = mMsgRecorder.FindAll(record => record.Msgtype == msgName && record.OnMsgReceived == onMsgReceived);
            selectedRecords.ForEach(record =>
            {
                MsgDispatcher.UnRegister(record.Msgtype, record.OnMsgReceived);
                mMsgRecorder.Remove(record);
                record.Recycle();
            });
            selectedRecords.Clear();
        }


        /// <summary>
        /// 根据字符串卸载所有消息
        /// </summary>
        /// <param name="msgName"></param>
        public void UnRegisterMsg(string msgName)
        {
            if (string.IsNullOrEmpty(msgName))
                return;
            var selectedRecords = mMsgRecorder.FindAll(record => record.MsgName == msgName);
            selectedRecords.ForEach(record =>
            {
                MsgDispatcher.UnRegister(record.MsgName, record.OnMsgReceived);
                mMsgRecorder.Remove(record);
                record.Recycle();
            });
            selectedRecords.Clear();
        }

        /// <summary>
        /// 根据字符串卸载指定消息
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="onMsgReceived"></param>
        public void UnRegisterMsg(string msgName, Action<object, object, object> onMsgReceived)
        {
            if (string.IsNullOrEmpty(msgName))
                return;
            var selectedRecords = mMsgRecorder.FindAll(record => record.MsgName == msgName && record.OnMsgReceived == onMsgReceived);
            selectedRecords.ForEach(record =>
            {
                MsgDispatcher.UnRegister(record.MsgName, record.OnMsgReceived);
                mMsgRecorder.Remove(record);
                record.Recycle();
            });
            selectedRecords.Clear();
        }




        public void UnRegisterAll()
        {
            foreach (var msgRecord in mMsgRecorder)
            {
                MsgDispatcher.UnRegister(msgRecord.Msgtype, msgRecord.OnMsgReceived);
                MsgDispatcher.UnRegister(msgRecord.MsgName, msgRecord.OnMsgReceived);
                msgRecord.Recycle();
            }
            mMsgRecorder.Clear();
        }

        protected virtual void OnDestroy()
        {
            OnBeforeDestroy();
            UnRegisterAll();
        }

        #endregion


        #region ObjectManager
        public virtual void Recycle(object param = null, object parm2 = null, object parm3 = null)
        {
            this.gameObject.Hide();
            ObjectManager.Instance.ReleaseObject(this.gameObject);
        }
        #endregion

    }

}