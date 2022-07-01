

namespace HotGersonFrame
{
    public interface IHotSystem : IHotBelongToArchitecture,IHotCanSetArchitecture,IHotCanGetModel,IHotCanGetUtility,IHotCanSendMsg,IHotCanRegisterMsg
    {
        void Init();
    }


    public abstract class HotAbstractSystem : IHotSystem
    {
        private IHotArchitecture m_architecture;
        public IHotArchitecture Architecture => m_architecture;

        /// <summary>
        /// 使用显示实现 规避子类调用Init方法 使用抽象方法进行初始化
        /// </summary>
         public void Init()
        {
            OnInit();
        }

        public void SetArchitecture(IHotArchitecture architecture)
        {
            m_architecture = architecture;
        }

        public abstract void OnInit();
    }

}
