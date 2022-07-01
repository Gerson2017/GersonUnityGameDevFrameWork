using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GersonFrame
{

    public class BaseInternalMsg
    {


        ~BaseInternalMsg()
        {
            UnRegisterAll();
        }

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
                retRecord.mMsgType = -1;
                retRecord.Msgtype = msgtype;
                retRecord.MsgName = null;
                retRecord.OnMsgReceived = onMsgReceived;
                return retRecord;
            }

            /// <summary>
            /// 根据消息类型获取消息对象
            /// </summary>
            /// <param name="msgtype"></param>
            /// <param name="onMsgReceived"></param>
            /// <returns></returns>
            public static MsgRecord Allocate(int msgtype,string msgname, Action<object, object, object> onMsgReceived)
            {
                var retRecord = mMsgRecordPool.Count > 0 ? mMsgRecordPool.Pop() : new MsgRecord();
                retRecord.mMsgType = msgtype;
                retRecord.Msgtype = MsgType.None;
                retRecord.MsgName = msgname;
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
                retRecord.mMsgType = -1;
                retRecord.Msgtype = MsgType.None;
                retRecord.MsgName = msgName;
                retRecord.OnMsgReceived = onMsgReceived;
                return retRecord;
            }

            public void Recycle()
            {
                Msgtype = MsgType.None;
                mMsgType = -1;
                MsgName = null;
                OnMsgReceived = null;
                mMsgRecordPool.Push(this);
            }

            public int mMsgType = -1;
            public MsgType Msgtype;
            public string MsgName = null;
            public Action<object, object, object> OnMsgReceived;

        }

        bool HasRegistered(Action<object, object, object> onMsgReceived)
        {
            for (int i = 0; i < mMsgRecorder.Count; i++)
            {
                if (mMsgRecorder[i].OnMsgReceived == onMsgReceived)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 根据消息类型注册消息
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="onMsgReceived"></param>
        public void RegisterMsg(int msgtype,string  msgName, Action<object, object, object> onMsgReceived)
        {
            if (HasRegistered(onMsgReceived))
                return;
            MsgDispatcher.Register(msgtype,msgName, onMsgReceived);
            mMsgRecorder.Add(MsgRecord.Allocate(msgtype,msgName, onMsgReceived));
        }

        /// <summary>
        /// 根据消息类型注册消息
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="onMsgReceived"></param>
        public void RegisterMsg(MsgType msgName, Action<object, object, object> onMsgReceived)
        {
            if (HasRegistered(onMsgReceived))
                return;
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
            if (HasRegistered(onMsgReceived))
                return;
            MsgDispatcher.Register(msgName, onMsgReceived);
            mMsgRecorder.Add(MsgRecord.Allocate(msgName, onMsgReceived));
        }


        /// <summary>
        /// 根据消息类型发送消息
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="data"></param>
        public void SendMsg(int msgtype,string msgName, object data1 = null, object data2 = null, object data3 = null)
        {
            MsgDispatcher.SendTypeMsg(msgtype,msgName, data1, data2, data3);
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
            MsgDispatcher.Send(msgName, data1,data2,data3);
        }


        /// <summary>
        /// 根据消息msgtype卸载所有消息
        /// </summary>
        /// <param name="msgName"></param>
        public void UnRegisterMsg(int msgtype )
        {
            var selectedRecords = mMsgRecorder.FindAll(record => record.mMsgType == msgtype);
            selectedRecords.ForEach(record =>
            {
                MsgDispatcher.UnRegister(record.mMsgType, record.MsgName, record.OnMsgReceived);
                mMsgRecorder.Remove(record);
                record.Recycle();
            });
            selectedRecords.Clear();
        }

        /// <summary>
        /// 根据消息msgtype卸载所有消息
        /// </summary>
        /// <param name="msgName"></param>
        public void UnRegisterMsg(int msgtype,string msgname)
        {
            var selectedRecords = mMsgRecorder.FindAll(record => record.mMsgType == msgtype && record.MsgName == msgname);
            selectedRecords.ForEach(record =>
            {
                MsgDispatcher.UnRegister(record.mMsgType, record.MsgName, record.OnMsgReceived);
                mMsgRecorder.Remove(record);
                record.Recycle();
            });
            selectedRecords.Clear();
        }

        /// <summary>
        /// 根据消息msgtype  msgname 卸载所有消息
        /// </summary>
        /// <param name="msgName"></param>
        public void UnRegisterMsg(int msgtype,string msgname, Action<object, object, object> onMsgReceived)
        {
            var selectedRecords = mMsgRecorder.FindAll(record => record.mMsgType == msgtype && record.MsgName== msgname && record.OnMsgReceived == onMsgReceived);
            selectedRecords.ForEach(record =>
            {
                MsgDispatcher.UnRegister(record.mMsgType, record.MsgName, record.OnMsgReceived);
                mMsgRecorder.Remove(record);
                record.Recycle();
            });
            selectedRecords.Clear();
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
                if (msgRecord.mMsgType!=-1)
                    MsgDispatcher.UnRegister(msgRecord.mMsgType, msgRecord.MsgName, msgRecord.OnMsgReceived);
                else   if (msgRecord.Msgtype!= MsgType.None)
                    MsgDispatcher.UnRegister(msgRecord.Msgtype, msgRecord.OnMsgReceived);
                else
                    MsgDispatcher.UnRegister(msgRecord.MsgName, msgRecord.OnMsgReceived);
                msgRecord.Recycle();
            }
            mMsgRecorder.Clear();
        }


        #endregion
    }
}