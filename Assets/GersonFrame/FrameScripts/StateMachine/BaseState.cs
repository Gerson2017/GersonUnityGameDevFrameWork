

namespace GersonFrame
{
    public abstract class BaseState
    {
        protected StateTransiton m_StateId;

        public string StateID => m_StateId.StateId;

        //为防止报错 这里先用StateController 代替 将来会是具体类
      protected  BaseState(MonoBehaviourSimplify statecontroller, StateTransiton stateId)
        {
            this.m_role = statecontroller; 
            this.m_StateId = stateId;
        }

        protected MonoBehaviourSimplify m_role = null;

        /// <summary>
        /// 是否可以跳转到下一状态
        /// </summary>
        /// <param name="transition"></param>
        /// <returns></returns>
        public  bool CanTransition(string transition)
        {
            if (m_StateId.CanTranSitionAll) return true;
            return m_StateId.CanTransitonStates.Contains(transition);
        }

        public abstract void OnEnter( object param1, object param2=null, object param3 = null);

        public abstract void Act();

        public abstract void LateAct();

        public abstract void onExit();

        /// <summary>
        /// 转换状态
        /// </summary>
        public abstract void UpdateTransition();

    }
}