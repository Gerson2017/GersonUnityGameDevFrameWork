
using GersonFrame.Tool;
using System;
using System.Collections.Generic;

namespace GersonFrame
{

    public class MsgDispatcher
    {

        static Dictionary<MsgType, Action<object, object, object>> mRegisteredMsgs = new Dictionary<MsgType, Action<object, object, object>>();

        static Dictionary<string, Action<object, object, object>> mRegisteredStrMsgs = new Dictionary<string, Action<object, object, object>>();

        /// <summary>
        /// 将消息按类型分类
        /// </summary>
        static Dictionary<int, Dictionary<string, Action<object, object, object>>> mRegisterTypeMsgs = new Dictionary<int, Dictionary<string, Action<object, object, object>>>();


     

        public static void Register(MsgType msgName, Action<object, object, object> onMsgReceived)
        {
            if (!mRegisteredMsgs.ContainsKey(msgName))
            {
                mRegisteredMsgs.Add(msgName, (a, b, c) => { });
            }
            mRegisteredMsgs[msgName] += onMsgReceived;
        }

        public static void UnRegisterAll(MsgType msgName)
        {
            if (mRegisteredMsgs.ContainsKey(msgName))
                mRegisteredMsgs.Remove(msgName);
            else
                MyDebuger.LogError("UnRegisterAll dont contains " + msgName);
        }

        public static void UnRegister(MsgType msgName, Action<object, object, object> onMsgReceived)
        {
            if (mRegisteredMsgs.ContainsKey(msgName))
            {
                mRegisteredMsgs[msgName] -= onMsgReceived;
            }
        }

        public static void Send(MsgType msgName, object data1 = null, object data2 = null, object data3 = null)
        {
            if (mRegisteredMsgs.ContainsKey(msgName))
            {
                mRegisteredMsgs[msgName](data1, data2, data3);
            }
        }


        //==========================================================================

        public static void Register(string msgName, Action<object, object, object> onMsgReceived)
        {
            if (string.IsNullOrEmpty(msgName))
            {
                MyDebuger.LogError("注册消息请传递消息名 当前消息名称为 空");
                return;
            }
            if (!mRegisteredStrMsgs.ContainsKey(msgName))
                mRegisteredStrMsgs.Add(msgName, (a, b, c) => { });
            mRegisteredStrMsgs[msgName] += onMsgReceived;
        }


        public static void UnRegisterAll(string msgName)
        {
            if (mRegisteredStrMsgs.ContainsKey(msgName))
                mRegisteredStrMsgs.Remove(msgName);
        }

      


        public static void UnRegister(string msgName, Action<object, object, object> onMsgReceived)
        {
            if (string.IsNullOrEmpty(msgName)) return;
            if (mRegisteredStrMsgs.ContainsKey(msgName))
            {
                mRegisteredStrMsgs[msgName] -= onMsgReceived;
            }
        }


        public static void Send(string msgName, object data1 = null, object data2 = null, object data3 = null)
        {
            if (mRegisteredStrMsgs.ContainsKey(msgName))
            {
                mRegisteredStrMsgs[msgName](data1, data2, data3);
            }
        }


       //============================================注册消息==========================================

        /// <summary>
        /// 注册消息类型 对消息进行分类 msgtype 消息的类别
        /// </summary>
        /// <param name="mgsname"></param>
        /// <param name="onMsgReceived"></param>
        /// <param name="msgtype"></param>
        public static void Register(int msgtype ,string mgsname, Action<object, object, object> onMsgReceived)
        {
            if (string.IsNullOrEmpty(mgsname))
            {
                MyDebuger.LogError("注册消息请传递消息名 当前消息名称为 空");
                return;
            }

            if (!mRegisterTypeMsgs.ContainsKey(msgtype))
            {
                mRegisterTypeMsgs[msgtype] = new Dictionary<string, Action<object, object, object>>();
            }
            if (!mRegisterTypeMsgs[msgtype].ContainsKey(mgsname))
            {
                mRegisterTypeMsgs[msgtype].Add(mgsname, (a, b, c) => { });
            }
            mRegisterTypeMsgs[msgtype][mgsname] += onMsgReceived;
        }


        /// <summary>
        /// 卸载所有消息 根据消息分类 msgtype 消息类别
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="msgtype"></param>
        public static void UnRegisterAll( int msgtype ,string msgName)
        {
            if (mRegisterTypeMsgs.ContainsKey(msgtype))
                if (mRegisterTypeMsgs[msgtype].ContainsKey(msgName))
                {
                    mRegisterTypeMsgs[msgtype].Remove(msgName);
                }
        }

        /// <summary>
        /// 取消注册消息 根据消息类别 msgtype 消息类别
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="onMsgReceived"></param>
        /// <param name="msgtype"></param>
        public static void UnRegister(int msgtype,string msgName, Action<object, object, object> onMsgReceived)
        {
            if (mRegisterTypeMsgs.ContainsKey(msgtype))
                if (mRegisterTypeMsgs[msgtype].ContainsKey(msgName))
                {
                    mRegisterTypeMsgs[msgtype][msgName] -= onMsgReceived;
                }
        }


        /// <summary>
        /// 发送消息 根据消息分类 msgtype 消息类别
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="data1"></param>
        /// <param name="data2"></param>
        /// <param name="data3"></param>
        /// <param name="msgtype"></param>
        public static void SendTypeMsg(int msgtype, string msgName, object data1 = null, object data2 = null, object data3 = null)
        {
            if (mRegisterTypeMsgs.ContainsKey(msgtype))
                if (mRegisterTypeMsgs[msgtype].ContainsKey(msgName))
                {
                    mRegisterTypeMsgs[msgtype][msgName](data1, data2, data3);
                }
        }


    }
}