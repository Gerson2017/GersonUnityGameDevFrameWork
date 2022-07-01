using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HotGersonFrame
{

    public interface IHotModel : IHotBelongToArchitecture,IHotCanSetArchitecture,IHotCanGetUtility,IHotCanSendMsg
    {
        void InitModel();
    }


    public abstract class HotAbstractModel : IHotModel
    {

        private IHotArchitecture m_architecture;
         IHotArchitecture IHotBelongToArchitecture.Architecture => m_architecture;

        /// <summary>
        /// 使用抽象方法进行初始化  使用显示实现 规避子类调用Init方法
        /// </summary>
       public void InitModel()
        {
            OnInit();
        }

        public void SetArchitecture(IHotArchitecture architecture)
        {
            m_architecture = architecture;
        }

        protected abstract void OnInit();


    }

    public interface ICounterModel : IHotModel
    {
        HotBindableProperty<int> Count { get; }
    }


    public class CounterModel : HotAbstractModel, ICounterModel
    {
        public HotBindableProperty<int> Count => new HotBindableProperty<int>() { Value = 0 };

        protected override void OnInit()
        {
        }
    }

}
