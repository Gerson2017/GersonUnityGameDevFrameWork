using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GersonFrame
{

    public abstract class BaseEnemyMoveState
    {
        public string m_StateId { get; private set; }
        protected MonoBehaviourSimplify m_enemy;
        protected Transform m_ts;
        protected float m_mSpeed = 0;


        public void ReflashMoveSpeed(float movespeed)
        {
            this.m_mSpeed = movespeed;
        }

        public  BaseEnemyMoveState(MonoBehaviourSimplify enemy, string stateId)
        {
            this.m_enemy = enemy;
            this.m_ts = enemy.transform;
            this.m_StateId = stateId;
        }


        /// <summary>
        /// 是否可以跳转到下一状态
        /// </summary>
        /// <param name="transition"></param>
        /// <returns></returns>
        public virtual bool CanTransition(string transition)
        {
            return true;
        }

        public abstract void OnEnter(object param1, object param2 = null, object param3 = null);

        public abstract void Act();

        public abstract void LateAct();
        public abstract void onExit();

        /// <summary>
        /// 转换状态
        /// </summary>
        public abstract void UpdateTransition();
    }
}
