
using GersonFrame.Tool;
using HotGersonFrame.HotIlRuntime;
using System.Collections.Generic;
using UnityEngine;


namespace HotGersonFrame
{

    public abstract class BaseHotMoveStateCtrl
    {

        protected Dictionary<string, HotBaseState> m_stateDic = new Dictionary<string, HotBaseState>();

        public Dictionary<string, HotBaseState> m_StaticDic
        {
            get { return m_stateDic; }
        }

        protected HotBaseState m_currentMoveState = null;

        public HotBaseState CurrentMoveState
        {
            get => m_currentMoveState;
        }

        /// <summary>
        /// 当前状态释放为空
        /// </summary>
        public bool CurrentStateIsNull
        {
            get
            {
                return this.m_currentMoveState == null;
            }
        }


        protected string m_enemyName;

        public BaseHotMoveStateCtrl(string enmyName)
        {
            this.m_enemyName = enmyName;
        }

        /// <summary>
        /// 添加状态
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="moveState"></param>
        public void AddState(string stateId, HotBaseState moveState)
        {
            if (m_stateDic.ContainsKey(stateId))
            {
                MyDebuger.LogWarning(string.Format("{0} 状态{1} 已经存在", this.m_enemyName, stateId));
                this.m_stateDic[stateId] = moveState;
            }
            else
                this.m_stateDic.Add(stateId, moveState);
        }

        /// <summary>
        /// 删除状态
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="moveState"></param>
        public void RemoveState(string stateId)
        {
            if (m_stateDic.ContainsKey(stateId))
            {
                m_stateDic.Remove(stateId);
            }
            else
            {
                MyDebuger.LogError(string.Format("状态{0} 不存在", stateId));
            }
        }

        /// <summary>
        /// 初始化移动状态根据字符串反射获取
        /// </summary>
        public abstract void InitMoveStates(BaseHotMono role);


        /// <summary>
        /// 状态切换
        /// </summary>
        /// <param name="stateid"></param>
        /// <param name="param1"></param>
        /// <returns></returns>
        public bool ChangeState(string stateid, object param1 = null, object param2 = null)
        {
            bool changesuccess = false;
            if (stateid != null)
            {
                if (this.m_stateDic.ContainsKey(stateid))
                {
                    HotBaseState state = this.m_stateDic[stateid];
                    bool canchangestate = true;
                    if (this.m_currentMoveState != null)
                        canchangestate = this.m_currentMoveState.CanTransition(stateid);//是否可以从当前状态跳转到某一状态
                    if (canchangestate)
                    {
                        if (this.m_currentMoveState != null)
                            this.m_currentMoveState.onExit();
                        state.OnEnter(param1, param2);
                        this.m_currentMoveState = state;
                        changesuccess = true;
                    }
                }
                else MyDebuger.LogWarning(string.Format("{0} 未找到状态:{1}", this.m_enemyName, stateid));
            }
            return changesuccess;
        }


        public virtual  void Update(float deltatime)
        {

                m_currentMoveState?.Act(deltatime);
        }


        public virtual void LateUpdate()
        {

            m_currentMoveState?.LateAct();
        }


    }
}
