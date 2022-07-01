using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GersonFrame
{
    public interface ICommand : IBelongToArchitecture, ICanSetArchitecture,ICanGetModel,ICanGetSystem,ICanGetUtility
    {
        void Execute(object arg1,object arg2,object arg3);
    }


    public abstract class AbstractCommand : ICommand
    {
        private IArchitecture m_architecture;
        public IArchitecture Architecture => m_architecture;

        /// <summary>
        /// 使用显示实现接口 阉割子类调用 子类采用实现抽闲方法的方式调用
        /// </summary>
         void ICommand.Execute(object arg1, object arg2, object arg3)
        {
            OnExecute(arg1,arg2,arg3);
        }

        public void SetArchitecture(IArchitecture architecture)
        {
            m_architecture = architecture;
        }

        public abstract void OnExecute(object arg1, object arg2, object arg3);


    }
}


