using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class GersonFrame_SelfILRuntime_AbstractTriggerInvoker_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(GersonFrame.SelfILRuntime.AbstractTriggerInvoker);
            args = new Type[]{typeof(System.Action<UnityEngine.Collider>)};
            method = type.GetMethod("AddCallBack", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddCallBack_0);


        }


        static StackObject* AddCallBack_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<UnityEngine.Collider> @callback = (System.Action<UnityEngine.Collider>)typeof(System.Action<UnityEngine.Collider>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)8);
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            GersonFrame.SelfILRuntime.AbstractTriggerInvoker instance_of_this_method = (GersonFrame.SelfILRuntime.AbstractTriggerInvoker)typeof(GersonFrame.SelfILRuntime.AbstractTriggerInvoker).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddCallBack(@callback);

            return __ret;
        }



    }
}
