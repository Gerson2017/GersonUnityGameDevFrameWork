

using HotGersonFrame.HotIlRuntime;
using System;

namespace HotGersonFrame
{
    public abstract class HotBaseState
    {
        protected StateTransiton m_StateTransion;
        public string StateId => m_StateTransion.StateId;

        public uint CrcStateID  => Crc32.GetCrc32(StateId);

        public Action m_EnterAction = null;
        public Action m_ExitAction = null;

        //为防止报错 这里先用StateController 代替 将来会是具体类
        protected HotBaseState(BaseHotMono statecontroller, StateTransiton stateId)
        {
            this.m_role = statecontroller;
            this.m_StateTransion = stateId;
        }

        protected BaseHotMono m_role = null;

        /// <summary>
        /// 是否可以跳转到下一状态
        /// </summary>
        /// <param name="transition"></param>
        /// <returns></returns>
        public  bool CanTransition(string transition)
        {
            if (m_StateTransion.CanTranSitionAll) return true;
            return m_StateTransion.CanTransitonStates.Contains(transition);
        }

        public abstract void OnEnter(object param1, object param2 = null, object param3 = null);

        public abstract void Act(float deltatime);

        public abstract void LateAct();

        public abstract void onExit();

        /// <summary>
        /// 转换状态
        /// </summary>
        public abstract void UpdateTransition(float deltatime);

    }
}