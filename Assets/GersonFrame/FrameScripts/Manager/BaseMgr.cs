using GersonFrame.Tool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GersonFrame {
    public enum Enviormnet
    {
        Developing,
        Test,
        Product
    }

    public abstract class BaseMgr : MonoBehaviourSimplify
    {
        public Enviormnet m_Enviorment = Enviormnet.Developing;
        private static Enviormnet s_shareEnviorment = Enviormnet.Developing;
        [SerializeField]
        private bool m_IsEnviormentCtr = false;
        private static int count = 0;

        protected virtual void Awake()
        {
            if (m_IsEnviormentCtr)
            {
                s_shareEnviorment = m_Enviorment;
                MyDebuger.Log("Awake=========="+s_shareEnviorment);
                count++;
            }
            if (count>1)
                MyDebuger.LogError("EnviormentCtr Not Only one "+this.gameObject.name);
        }

        protected virtual void Start()
        {
            this.OnMgrStart();
        }


        /// <summary>
        /// 游戏入口
        /// </summary>
        public virtual void OnMgrStart()
        {
            switch (s_shareEnviorment)
            {
                case Enviormnet.Developing:
                    this.RunInDevloping();
                    break;
                case Enviormnet.Product:
                    this.RunInProduct();
                    break;
                case Enviormnet.Test:
                    this.RunInTest();
                    break;
                default:
                    MyDebuger.LogError("未找到游戏运行环境");
                    break;
            }
        }

        /// <summary>
        /// 开发环境运行
        /// </summary>
        protected abstract void RunInDevloping();
        /// <summary>
        /// 测试环境运行
        /// </summary>
        protected abstract void RunInTest();
        /// <summary>
        /// 生产环境运行
        /// </summary>
        protected abstract void RunInProduct();

    }
}


