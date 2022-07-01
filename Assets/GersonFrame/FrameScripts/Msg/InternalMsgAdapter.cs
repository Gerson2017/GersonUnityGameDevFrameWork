using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GersonFrame
{

    public class InternalMsgAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType => typeof(BaseInternalMsg);

        public override Type AdaptorType => typeof(Adapter);

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }


        public class Adapter : BaseInternalMsg, CrossBindingAdaptorType
        {
            private ILTypeInstance m_Instance;
            private ILRuntime.Runtime.Enviorment.AppDomain m_AppDomain;

            private IMethod m_ToString;

            public Adapter() { }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appDomain, ILTypeInstance instance)
            {
                m_AppDomain = appDomain;
                m_Instance = instance;
            }

            public ILTypeInstance ILInstance => m_Instance;

            public override string ToString()
            {
                if (m_ToString == null)
                    m_ToString = m_AppDomain.ObjectType.GetMethod("ToString", 0);
                IMethod m = m_Instance.Type.GetVirtualMethod(m_ToString);
                if (m == null || m is ILMethod)
                    return m_Instance.ToString();
                else
                    return m_Instance.Type.FullName;
            }
        }
    }
}
