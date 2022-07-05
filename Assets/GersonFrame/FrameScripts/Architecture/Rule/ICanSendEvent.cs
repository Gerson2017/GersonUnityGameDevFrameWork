using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GersonFrame
{
    public interface ICanSendEvent:IBelongToArchitecture
    {

    }


    public static class CanSendMsgExtension
    {
        public static void SendEvt<T>(this ICanSendEvent self) where T:new()
        {
            self.Architecture.SendEvt<T>();
        }


        public static void SendEvt<T>(this ICanSendEvent self,T t) 
        {
            self.Architecture.SendEvt(t);
        }


    }
}

