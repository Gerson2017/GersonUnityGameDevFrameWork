//using System;
//using System.Collections.Generic;
//using System.Reflection;

//namespace ILRuntime.Runtime.Generated
//{
//    class CLRBindings
//    {

////will auto register in unity
//#if UNITY_5_3_OR_NEWER
//        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
//#endif
//        static private void RegisterBindingAction()
//        {
//            ILRuntime.Runtime.CLRBinding.CLRBindingUtils.RegisterBindingAction(Initialize);
//        }

//        internal static ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector3> s_UnityEngine_Vector3_Binding_Binder = null;
//        internal static ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Quaternion> s_UnityEngine_Quaternion_Binding_Binder = null;
//        internal static ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector2> s_UnityEngine_Vector2_Binding_Binder = null;

//        /// <summary>
//        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
//        /// </summary>
//        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
//        {
//            System_Object_Binding.Register(app);
//            System_String_Binding.Register(app);
//            MyDebuger_Binding.Register(app);
//            UnityEngine_Object_Binding.Register(app);
//            System_Collections_Generic_List_1_ILTypeInstance_Binding.Register(app);
//            System_Collections_Generic_List_1_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
//            System_IDisposable_Binding.Register(app);
//            GersonFrame_BaseInternalMsg_Binding.Register(app);
//            Crc32_Binding.Register(app);
//            System_Collections_Generic_Dictionary_2_Type_Object_Binding.Register(app);
//            StateTransiton_Binding.Register(app);
//            System_Collections_Generic_List_1_String_Binding.Register(app);
//            System_Collections_Generic_Dictionary_2_String_ILTypeInstance_Binding.Register(app);
//            GersonFrame_MonoSingleton_1_UIManager_Binding.Register(app);
//            GersonFrame_UI_UIManager_Binding.Register(app);
//            UnityEngine_MonoBehaviour_Binding.Register(app);
//            UnityEngine_Vector2_Binding.Register(app);
//            UnityEngine_Mathf_Binding.Register(app);
//            UnityEngine_GameObject_Binding.Register(app);
//            UnityEngine_Component_Binding.Register(app);
//            UnityEngine_RectTransform_Binding.Register(app);
//            UnityEngine_EventSystems_PointerEventData_Binding.Register(app);
//            UnityEngine_UI_CanvasScaler_Binding.Register(app);
//            UnityEngine_Screen_Binding.Register(app);
//            UnityEngine_Canvas_Binding.Register(app);
//            UnityEngine_Transform_Binding.Register(app);
//            UnityEngine_RectTransformUtility_Binding.Register(app);
//            System_Collections_Generic_Dictionary_2_Type_Int32_Binding.Register(app);
//            System_Type_Binding.Register(app);
//            System_Collections_Generic_Dictionary_2_Int32_Type_Binding.Register(app);
//            System_Collections_Generic_Dictionary_2_Int32_ILTypeInstance_Binding.Register(app);
//            System_Int32_Binding.Register(app);
//            System_Collections_Generic_Dictionary_2_Type_List_1_Object_Binding.Register(app);
//            System_Collections_Generic_List_1_Object_Binding.Register(app);
//            System_Collections_Generic_List_1_Type_Binding.Register(app);
//            GameObjectExtension_Binding.Register(app);
//            GersonFrame_SelfILRuntime_AbstractMonoNormalInvoker_Binding.Register(app);
//            System_Action_Binding.Register(app);
//            GersonFrame_SelfILRuntime_AbstractTriggerInvoker_Binding.Register(app);
//            GersonFrame_SelfILRuntime_AbstractCollisionInvoker_Binding.Register(app);
//            GersonFrame_SelfILRuntime_AbstractMonoValueInvoker_Binding.Register(app);
//            GersonFrame_SelfILRuntime_AbstractMonoPointerInvoker_Binding.Register(app);
//            UnityEngine_Application_Binding.Register(app);
//            System_Action_1_Int32_Binding.Register(app);
//            System_Threading_Interlocked_Binding.Register(app);
//            System_Array_Binding.Register(app);
//            System_Reflection_ConstructorInfo_Binding.Register(app);
//            System_Exception_Binding.Register(app);

//            ILRuntime.CLR.TypeSystem.CLRType __clrType = null;
//            __clrType = (ILRuntime.CLR.TypeSystem.CLRType)app.GetType (typeof(UnityEngine.Vector3));
//            s_UnityEngine_Vector3_Binding_Binder = __clrType.ValueTypeBinder as ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector3>;
//            __clrType = (ILRuntime.CLR.TypeSystem.CLRType)app.GetType (typeof(UnityEngine.Quaternion));
//            s_UnityEngine_Quaternion_Binding_Binder = __clrType.ValueTypeBinder as ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Quaternion>;
//            __clrType = (ILRuntime.CLR.TypeSystem.CLRType)app.GetType (typeof(UnityEngine.Vector2));
//            s_UnityEngine_Vector2_Binding_Binder = __clrType.ValueTypeBinder as ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector2>;
//        }

//        /// <summary>
//        /// Release the CLR binding, please invoke this BEFORE ILRuntime Appdomain destroy
//        /// </summary>
//        public static void Shutdown(ILRuntime.Runtime.Enviorment.AppDomain app)
//        {
//            s_UnityEngine_Vector3_Binding_Binder = null;
//            s_UnityEngine_Quaternion_Binding_Binder = null;
//            s_UnityEngine_Vector2_Binding_Binder = null;
//        }
//    }
//}
