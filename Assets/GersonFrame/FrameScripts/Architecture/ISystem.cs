using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GersonFrame
{
    public interface ISystem : IBelongToArchitecture,ICanSetArchitecture,ICanGetModel,ICanGetUtility,ICanSendMsg,ICanRegisterMsg
    {
        void Init();
    }


    public abstract class AbstractSystem : ISystem
    {
        private IArchitecture m_architecture;
        public IArchitecture Architecture => m_architecture;

        /// <summary>
        /// 使用显示实现 规避子类调用Init方法 使用抽象方法进行初始化
        /// </summary>
         void ISystem.Init()
        {
            OnInit();
        }

        public void SetArchitecture(IArchitecture architecture)
        {
            m_architecture = architecture;
        }

        public abstract void OnInit();
    }

}
