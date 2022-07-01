using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HotGersonFrame
{

    public interface IHotCanRegisterMsg:IHotBelongToArchitecture
    {

    }

    public static class CanRegisterMsgExtension
    {

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        public static void RegisterMsg(IHotCanRegisterMsg self, string msgType, System.Action<object, object, object> onMsgReceived)
        {
            self.Architecture.RegisterMsg(msgType, onMsgReceived);
        }



        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        public static void RegisterTypeMsg(IHotCanRegisterMsg self, int msgType, string msgName, System.Action<object, object, object> onMsgReceived) 
        {
            self.Architecture.RegisterMsg(msgType, msgName, onMsgReceived);
        }


        /// <summary>
        /// 卸载指定消息的 某个监听
        /// </summary>
        public static void UnRegisterMsg( IHotCanRegisterMsg self, string msgType, System.Action<object, object, object> onMsgReceived)
        {
            self.Architecture.UnRegisterMsg(msgType, onMsgReceived);
        }



        /// <summary>
        /// 卸载指定消息的 某个监听
        /// </summary>
        public static void UnRegisterMsg(IHotCanRegisterMsg self, int msgType, string msgName, System.Action<object, object, object> onMsgReceived)
        {
            self.Architecture.UnRegisterMsg(msgType, msgName, onMsgReceived);
        }


        /// <summary>
        /// 卸载指定消息的 某个监听
        /// </summary>
        public static void UnRegisterMsg(IHotCanRegisterMsg self, int msgType, string msgName)
        {
            self.Architecture.UnRegisterMsg(msgType, msgName);
        }


        /// <summary>
        /// 卸载指定消息的 某个监听
        /// </summary>
        public static void UnRegisterMsg(IHotCanRegisterMsg self, int msgType)
        {
            self.Architecture.UnRegisterMsg(msgType);
        }



        /// <summary>
        /// 卸载指定消息的所有监听
        /// </summary>
        /// <param name="msgType"></param>
        public static void UnRegisterMsg(IHotCanRegisterMsg self, string msgType)
        {
            self.Architecture.UnRegisterMsg(msgType);
        }

        /// <summary>
        /// 卸载所有监听
        /// </summary>
        public static void UnRegisterAll(IHotCanRegisterMsg self)
        {
            self.Architecture.UnRegisterAll();
        }


    }


}
