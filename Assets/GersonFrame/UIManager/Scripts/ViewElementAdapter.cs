using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GersonFrame.UI
{


    public class ViewElementAdapter : CrossBindingAdaptor
    {

        static CrossBindingMethodInfo<GameObject> mInitElement = new CrossBindingMethodInfo<GameObject>("InitElement");
        
        public override Type BaseCLRType => typeof(ViewElementBase);

        public override Type AdaptorType => typeof(Adaptor);


        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adaptor(appdomain, instance);
        }


        public class Adaptor : ViewElementBase, CrossBindingAdaptorType
        {
            private ILTypeInstance m_Instance;
            private ILRuntime.Runtime.Enviorment.AppDomain m_AppDomain;

            private IMethod m_ToString;


            public Adaptor() { }
            public Adaptor(GameObject go) { }

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


            public override void InitElement(GameObject go)
            {
                if (mInitElement.CheckShouldInvokeBase(this.m_Instance))
                    base.InitElement(go);
                else
                    mInitElement.Invoke(this.m_Instance, go);
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
