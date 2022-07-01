using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GersonFrame.UI
{


    public class HotfixVIewAdapter : CrossBindingAdaptor
    {

        static CrossBindingMethodInfo mUpdate = new CrossBindingMethodInfo("Update");
        static CrossBindingMethodInfo mAddUIListener = new CrossBindingMethodInfo("AddUIListener");
        static CrossBindingMethodInfo mRegisterAllMsg = new CrossBindingMethodInfo("RegisterMsgListener");
        static CrossBindingMethodInfo mUnRegisterAllMsg = new CrossBindingMethodInfo("UnRegisterMsgListener");
        static CrossBindingMethodInfo mRegisterNetMsg = new CrossBindingMethodInfo("RegisterNetMsg");
        static CrossBindingMethodInfo mUnRegisterNetMsg = new CrossBindingMethodInfo("UnRegisterNetMsg");

        static CrossBindingMethodInfo mInitView = new CrossBindingMethodInfo("InitView");
        static CrossBindingMethodInfo mOnPause = new CrossBindingMethodInfo("OnPause");
        static CrossBindingMethodInfo mOnResume = new CrossBindingMethodInfo("OnResume");
        static CrossBindingMethodInfo mOnExit = new CrossBindingMethodInfo("OnExit");
        static CrossBindingMethodInfo mOnDestroy = new CrossBindingMethodInfo("OnDestroy");
        static CrossBindingMethodInfo mShowScalAm = new CrossBindingMethodInfo("ShowScalAm");
        static CrossBindingMethodInfo mHideScalAm = new CrossBindingMethodInfo("HideScalAm");
        static CrossBindingMethodInfo mShowEnd = new CrossBindingMethodInfo("ShowEnd");
        static CrossBindingMethodInfo mHideEnd = new CrossBindingMethodInfo("HideEnd");
        static CrossBindingMethodInfo<object, object, object> mOnEnter = new CrossBindingMethodInfo<object, object, object>("OnEnter");

        public override Type BaseCLRType => typeof(BaseHotView);

        public override Type AdaptorType => typeof(Adaptor);


        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adaptor(appdomain, instance);
        }


        public class Adaptor : BaseHotView, CrossBindingAdaptorType
        {
            private ILTypeInstance m_Instance;
            private ILRuntime.Runtime.Enviorment.AppDomain m_AppDomain;

            private IMethod m_ToString;


            public Adaptor() { }

            public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appDomain, ILTypeInstance instance)
            {
                m_AppDomain = appDomain;
                m_Instance = instance;
            }

            public ILTypeInstance ILInstance
            {
                get
                {
                    return m_Instance;
                }
            }

            protected override void InitView()
            {
                 mInitView.Invoke(this.m_Instance);
            }

            public override void OnEnter(object param = null, object param2 = null, object param3 = null)
            {
                if (mOnEnter.CheckShouldInvokeBase(this.m_Instance))
                    base.OnEnter(param, param2, param3);
                else
                    mOnEnter.Invoke(this.m_Instance, param, param2, param3);
            }


            public override void OnPause()
            {
                if (mOnPause.CheckShouldInvokeBase(this.m_Instance))
                    base.OnPause();
                else
                    mOnPause.Invoke(this.m_Instance);
            }

            public override void OnResume()
            {
                if (mOnResume.CheckShouldInvokeBase(this.m_Instance))
                    base.OnResume();
                else
                    mOnResume.Invoke(this.m_Instance);
            }

            public override void OnExit()
            {
                if (mOnExit.CheckShouldInvokeBase(this.m_Instance))
                    base.OnExit();
                else
                    mOnExit.Invoke(this.m_Instance);
            }

            public override void OnDestroy()
            {
                if (mOnDestroy.CheckShouldInvokeBase(this.m_Instance))
                    base.OnDestroy();
                else
                    mOnDestroy.Invoke(this.m_Instance);

            }

            public override void Update()
            {
                if (mUpdate.CheckShouldInvokeBase(this.m_Instance))
                    base.Update();
                else
                    mUpdate.Invoke(this.m_Instance);
            }


            public override void ShowScalAm()
            {
                if (mShowScalAm.CheckShouldInvokeBase(this.m_Instance))
                    base.ShowScalAm();
                else
                    mShowScalAm.Invoke(this.m_Instance);
            }


            public override void HideScalAm()
            {
                if (mHideScalAm.CheckShouldInvokeBase(this.m_Instance))
                    base.HideScalAm();
                else
                    mHideScalAm.Invoke(this.m_Instance);
            }


            protected override void ShowEnd()
            {
                if (mShowEnd.CheckShouldInvokeBase(this.m_Instance))
                    base.ShowEnd();
                else
                    mShowEnd.Invoke(this.m_Instance);
            }


            protected override void HideEnd()
            {
                if (mHideEnd.CheckShouldInvokeBase(this.m_Instance))
                    base.HideEnd();
                else
                    mHideEnd.Invoke(this.m_Instance);
            }



            protected override void RegisterMsgListener()
            {
                mRegisterAllMsg.Invoke(this.m_Instance);
            }

            protected override void UnRegisterMsgListener()
            {
                mUnRegisterAllMsg.Invoke(this.m_Instance);
            }

            protected override void RegisterNetMsg()
            {
                mRegisterNetMsg.Invoke(this.m_Instance);
            }

            protected override void UnRegisterNetMsg()
            {
                mUnRegisterNetMsg.Invoke(this.m_Instance);
            }

            protected override void AddUIListener()
            {
                mAddUIListener.Invoke(this.m_Instance);
            }


            public override string ToString()
            {
                if (m_ToString == null)
                {
                    m_ToString = m_AppDomain.ObjectType.GetMethod("ToString", 0);
                }
                IMethod m = m_Instance.Type.GetVirtualMethod(m_ToString);
                if (m == null || m is ILMethod)
                    return m_Instance.ToString();
                else
                    return m_Instance.Type.FullName;
            }


        }
    }
}
