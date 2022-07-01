using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GersonFrame
{

    public interface ICanRegisterMsg:IBelongToArchitecture
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
        public static void RegisterMsg(ICanRegisterMsg self, string msgType, System.Action<object, object, object> onMsgReceived) 
        {
            self.Architecture.RegisterMsg(msgType,onMsgReceived);
        }


        /// <summary>
        /// 卸载指定消息的 某个监听
        /// </summary>
        public static void UnRegisterMsg( ICanRegisterMsg self, string msgType, System.Action<object, object, object> onMsgReceived)
        {
            self.Architecture.UnRegisterMsg(msgType, onMsgReceived);
        }

        /// <summary>
        /// 卸载指定消息的所有监听
        /// </summary>
        /// <param name="msgType"></param>
        public static void UnRegisterMsg(ICanRegisterMsg self, string msgType)
        {
            self.Architecture.UnRegisterMsg(msgType);
        }

        /// <summary>
        /// 卸载所有监听
        /// </summary>
        public static void UnRegisterAll(ICanRegisterMsg self)
        {
            self.Architecture.UnRegisterAll();
        }


    }


}
