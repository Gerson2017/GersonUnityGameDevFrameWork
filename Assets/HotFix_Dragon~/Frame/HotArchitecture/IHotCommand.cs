using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HotGersonFrame
{
    public interface IHotCommand : IHotBelongToArchitecture, IHotCanSetArchitecture,IHotCanGetModel,IHotCanGetSystem,IHotCanGetUtility
    {
        /// <summary>
        /// 外部调用 command内部不管该方法
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        void Execute(object arg1,object arg2,object arg3);
    }


    public abstract class HotAbstractCommand : IHotCommand
    {
        private IHotArchitecture m_architecture;
        public IHotArchitecture Architecture => m_architecture;

        /// <summary>
        /// 使用显示实现接口 阉割子类调用 子类采用实现抽闲方法的方式调用
        /// </summary>
        public   void Execute(object arg1, object arg2, object arg3)
        {
            OnExecute(arg1,arg2,arg3);
        }


        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        protected void SendMsg(string msgType, object arg1 = null, object arg2 = null, object arg3 = null)
        {
            Architecture.SendMsg( msgType,  arg1,arg2,  arg3);
        }

        public void SetArchitecture(IHotArchitecture architecture)
        {
            m_architecture = architecture;
        }

        public abstract void OnExecute(object arg1, object arg2, object arg3);
    }


}


