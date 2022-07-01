using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.CLR.Utils;

namespace GersonFrame.Tool
{

    public unsafe class MyDebuger_ILRunTimeBinding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(global::MyDebuger);
            args = new Type[] { typeof(System.Object) };
            method = type.GetMethod("Log", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Log_0);
            args = new Type[] { typeof(System.Object) };
            method = type.GetMethod("LogError", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LogError_1);
            args = new Type[] { typeof(System.Object) };
            method = type.GetMethod("LogWarning", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LogWarning_2);
            args = new Type[] { typeof(System.String), typeof(System.Object[]) };
            method = type.GetMethod("LogErrorFormat", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LogErrorFormat_3);
            args = new Type[] { typeof(System.String), typeof(System.Object[]) };
            method = type.GetMethod("LogWarningFormat", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LogWarningFormat_4);
            args = new Type[] { typeof(global::LogLevel) };
            method = type.GetMethod("InitLogger", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, InitLogger_5);
            args = new Type[] { typeof(System.String), typeof(System.Object[]) };
            method = type.GetMethod("LogFormat", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LogFormat_6);


        }


        static StackObject* Log_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object @message = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            //在真实调用Debug.Log前，我们先获取DLL内的堆栈
            var stacktrace = __domain.DebugService.GetStackTrace(__intp);

            global::MyDebuger.Log(@message + "\n" + stacktrace);

            return __ret;
        }

        static StackObject* LogError_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object @message = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            //在真实调用Debug.Log前，我们先获取DLL内的堆栈
            var stacktrace = __domain.DebugService.GetStackTrace(__intp);

            global::MyDebuger.LogError(@message + "\n" + stacktrace);

            return __ret;
        }

        static StackObject* LogWarning_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object @message = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);



            //在真实调用Debug.Log前，我们先获取DLL内的堆栈
            var stacktrace = __domain.DebugService.GetStackTrace(__intp);

            global::MyDebuger.LogWarning(@message + "\n" + stacktrace);

            return __ret;
        }

        static StackObject* LogErrorFormat_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object[] @args = (System.Object[])typeof(System.Object[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @format = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            //在真实调用Debug.Log前，我们先获取DLL内的堆栈
            var stacktrace = __domain.DebugService.GetStackTrace(__intp);


            global::MyDebuger.LogErrorFormat(@format, @args, stacktrace);

            return __ret;
        }

        static StackObject* LogWarningFormat_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object[] @args = (System.Object[])typeof(System.Object[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @format = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            //在真实调用Debug.Log前，我们先获取DLL内的堆栈
            var stacktrace = __domain.DebugService.GetStackTrace(__intp);
            global::MyDebuger.LogWarningFormat(@format, @args, stacktrace);

            return __ret;
        }

        static StackObject* InitLogger_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::LogLevel @logLevel = (global::LogLevel)typeof(global::LogLevel).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            global::MyDebuger.InitLogger(@logLevel);

            return __ret;
        }

        static StackObject* LogFormat_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object[] @args = (System.Object[])typeof(System.Object[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @format = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            //在真实调用Debug.Log前，我们先获取DLL内的堆栈
            var stacktrace = __domain.DebugService.GetStackTrace(__intp);
            global::MyDebuger.LogWarningFormat(@format, @args, stacktrace);

            return __ret;
        }


    }
}
