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
        public static void SendMsg(this ICanSendMsg self,string msgType, object arg1=null, object arg2=null, object arg3=null) 
        {
            self.Architecture.SendMsg(msgType,arg1,arg2,arg3);
        }
    }
}

