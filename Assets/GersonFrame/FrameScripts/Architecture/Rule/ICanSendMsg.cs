using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GersonFrame
{
    public interface ICanSendMsg:IBelongToArchitecture
    {

    }


    public static class CanSendMsgExtension
    {
        public static void SendEvt<T>(this ICanSendMsg self) where T:new()
        {
            self.Architecture.SendEvt<T>();
        }


        public static void SendEvt<T>(this ICanSendMsg self,T t) 
        {
            self.Architecture.SendEvt(t);
        }


    }
}

