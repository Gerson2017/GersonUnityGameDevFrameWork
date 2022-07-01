using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GersonFrame
{

    public interface IModel : IBelongToArchitecture,ICanSetArchitecture,ICanGetUtility,ICanSendMsg
    {
        void Init();
    }


    public abstract class AbstractModel : IModel
    {

        private IArchitecture m_architecture;
         IArchitecture IBelongToArchitecture.Architecture => m_architecture;

        /// <summary>
        /// 使用抽象方法进行初始化  使用显示实现 规避子类调用Init方法
        /// </summary>
        void IModel.Init()
        {
            OnInit();
        }

        public void SetArchitecture(IArchitecture architecture)
        {
            m_architecture = architecture;
        }

        protected abstract void OnInit();


    }

    public interface ICounterModel : IModel
    {
        BindableProperty<int> Count { get; }
    }


    public class CounterModel : AbstractModel /* 新增 */, ICounterModel
    {
        public BindableProperty<int> Count => new BindableProperty<int>() { Value = 0 };

        protected override void OnInit()
        {
        }
    }

}
