using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HotGersonFrame
{
    public interface IHotCanSendMsg:IHotBelongToArchitecture
    {

    }


    public static class CanSendMsgExtension
    {
        public static void SendMsg(this IHotCanSendMsg self,string msgType, object arg1=null, object arg2=null, object arg3=null) 
        {
            self.Architecture.SendMsg(msgType,arg1,arg2,arg3);
        }
    }
}

