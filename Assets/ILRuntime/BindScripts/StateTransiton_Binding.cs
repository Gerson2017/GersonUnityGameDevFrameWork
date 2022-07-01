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
    unsafe class StateTransiton_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::StateTransiton);

            field = type.GetField("StateId", flag);
            app.RegisterCLRFieldGetter(field, get_StateId_0);
            app.RegisterCLRFieldSetter(field, set_StateId_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_StateId_0, AssignFromStack_StateId_0);
            field = type.GetField("CanTranSitionAll", flag);
            app.RegisterCLRFieldGetter(field, get_CanTranSitionAll_1);
            app.RegisterCLRFieldSetter(field, set_CanTranSitionAll_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_CanTranSitionAll_1, AssignFromStack_CanTranSitionAll_1);
            field = type.GetField("CanTransitonStates", flag);
            app.RegisterCLRFieldGetter(field, get_CanTransitonStates_2);
            app.RegisterCLRFieldSetter(field, set_CanTransitonStates_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_CanTransitonStates_2, AssignFromStack_CanTransitonStates_2);

            app.RegisterCLRCreateDefaultInstance(type, () => new global::StateTransiton());


        }

        static void WriteBackInstance(ILRuntime.Runtime.Enviorment.AppDomain __domain, StackObject* ptr_of_this_method, IList<object> __mStack, ref global::StateTransiton instance_of_this_method)
        {
            ptr_of_this_method = ILIntepreter.GetObjectAndResolveReference(ptr_of_this_method);
            switch(ptr_of_this_method->ObjectType)
            {
                case ObjectTypes.Object:
                    {
                        __mStack[ptr_of_this_method->Value] = instance_of_this_method;
                    }
                    break;
                case ObjectTypes.FieldReference:
                    {
                        var ___obj = __mStack[ptr_of_this_method->Value];
                        if(___obj is ILTypeInstance)
                        {
                            ((ILTypeInstance)___obj)[ptr_of_this_method->ValueLow] = instance_of_this_method;
                        }
                        else
                        {
                            var t = __domain.GetType(___obj.GetType()) as CLRType;
                            t.SetFieldValue(ptr_of_this_method->ValueLow, ref ___obj, instance_of_this_method);
                        }
                    }
                    break;
                case ObjectTypes.StaticFieldReference:
                    {
                        var t = __domain.GetType(ptr_of_this_method->Value);
                        if(t is ILType)
                        {
                            ((ILType)t).StaticInstance[ptr_of_this_method->ValueLow] = instance_of_this_method;
                        }
                        else
                        {
                            ((CLRType)t).SetStaticFieldValue(ptr_of_this_method->ValueLow, instance_of_this_method);
                        }
                    }
                    break;
                 case ObjectTypes.ArrayReference:
                    {
                        var instance_of_arrayReference = __mStack[ptr_of_this_method->Value] as global::StateTransiton[];
                        instance_of_arrayReference[ptr_of_this_method->ValueLow] = instance_of_this_method;
                    }
                    break;
            }
        }


        static object get_StateId_0(ref object o)
        {
            return ((global::StateTransiton)o).StateId;
        }

        static StackObject* CopyToStack_StateId_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::StateTransiton)o).StateId;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_StateId_0(ref object o, object v)
        {
            global::StateTransiton ins =(global::StateTransiton)o;
            ins.StateId = (System.String)v;
            o = ins;
        }

        static StackObject* AssignFromStack_StateId_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @StateId = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            global::StateTransiton ins =(global::StateTransiton)o;
            ins.StateId = @StateId;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_CanTranSitionAll_1(ref object o)
        {
            return ((global::StateTransiton)o).CanTranSitionAll;
        }

        static StackObject* CopyToStack_CanTranSitionAll_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::StateTransiton)o).CanTranSitionAll;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_CanTranSitionAll_1(ref object o, object v)
        {
            global::StateTransiton ins =(global::StateTransiton)o;
            ins.CanTranSitionAll = (System.Boolean)v;
            o = ins;
        }

        static StackObject* AssignFromStack_CanTranSitionAll_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @CanTranSitionAll = ptr_of_this_method->Value == 1;
            global::StateTransiton ins =(global::StateTransiton)o;
            ins.CanTranSitionAll = @CanTranSitionAll;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_CanTransitonStates_2(ref object o)
        {
            return ((global::StateTransiton)o).CanTransitonStates;
        }

        static StackObject* CopyToStack_CanTransitonStates_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::StateTransiton)o).CanTransitonStates;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_CanTransitonStates_2(ref object o, object v)
        {
            global::StateTransiton ins =(global::StateTransiton)o;
            ins.CanTransitonStates = (System.Collections.Generic.List<System.String>)v;
            o = ins;
        }

        static StackObject* AssignFromStack_CanTransitonStates_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.List<System.String> @CanTransitonStates = (System.Collections.Generic.List<System.String>)typeof(System.Collections.Generic.List<System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack), (CLR.Utils.Extensions.TypeFlags)0);
            global::StateTransiton ins =(global::StateTransiton)o;
            ins.CanTransitonStates = @CanTransitonStates;
            o = ins;
            return ptr_of_this_method;
        }



    }
}
