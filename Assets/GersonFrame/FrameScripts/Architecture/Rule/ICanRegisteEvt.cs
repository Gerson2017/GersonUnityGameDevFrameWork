using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GersonFrame
{

    public interface ICanRegisteEvt:IBelongToArchitecture
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
        public static void RegisterMsg<T>(this ICanRegisteEvt self, Action<T> OnEvent) 
        {
            self.Architecture.RegistEvt(OnEvent);
        }


        /// <summary>
        /// 卸载指定消息的 某个监听
        /// </summary>
        public static void UnRegisterMsg<T>(this  ICanRegisteEvt self, Action<T> OnEvent)
        {
            self.Architecture.UnRegisterEvt(OnEvent);
        }

    

        /// <summary>
        /// 卸载所有监听
        /// </summary>
        public static void UnRegisterAll(this ICanRegisteEvt self)
        {
            self.Architecture.UnRegisterAllEvt();
        }


    }


}
