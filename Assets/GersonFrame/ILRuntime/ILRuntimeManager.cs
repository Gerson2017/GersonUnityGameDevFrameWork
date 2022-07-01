using System.Collections.Generic;
using UnityEngine;
using ILRuntime.Runtime.Enviorment;
using System.IO;
using ILRuntime.Mono.Cecil.Pdb;
using GersonFrame.ABFrame;
using GersonFrame.UI;
using ILRuntime.Runtime.Stack;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using GersonFrame.Tool;

namespace GersonFrame.SelfILRuntime
{


    public class ILRuntimeManager : Singleton<ILRuntimeManager>
    {

        ILRuntimeManager() { }

        public const string DllPath = "Assets/ILRuntime/HotFixDll/HotFix_Dragon.dll";
        public const string PDBPath = "Assets/ILRuntime/HotFixDll/HotFix_Dragon.pdb";
        public const string DomainNamePath = "Assets/ILRuntime/HotFixDll/DomainNameConfig.asset";


        /// <summary>
        /// 程序集名称字典
        /// </summary>
        private Dictionary<DomainType, string> m_DomainInfos = new Dictionary<DomainType, string>();

        /// <summary>
        /// 程序域
        /// </summary>
        public AppDomain AppDomain
        {
            get; private set;
        }

        /// <summary>
        /// 加载程序集 初始化debug 
        /// </summary>
        public void Init()
        {
            if (AppDomain != null) return;
            LoadHotFixAssembly();

#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
            //由于Unity的Profiler接口只允许在主线程使用，为了避免出异常，需要告诉ILRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
            AppDomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
            InitializeILRuntime();
            InitDoMainInfos();
        }


        void InitDoMainInfos()
        {
            DomainName domainName = ResourceManager.Instance.LoadResource<DomainName>(DomainNamePath);
            for (int i = 0; i < domainName.domainInfos.Count; i++)
            {
                DomainInfo info = domainName.domainInfos[i];
                m_DomainInfos[info.domainType] = info.Name;
            }
        }

        void LoadHotFixAssembly()
        {
            //整个程序只有一个ILRuntime的AppDomain
            AppDomain = new AppDomain(ILRuntime.Runtime.ILRuntimeJITFlags.JITOnDemand);
           //AppDomain = new AppDomain();
            //读取热更资源的Dll
            TextAsset dlltxt = ResourceManager.Instance.LoadResource<TextAsset>(DllPath + ".bytes");
            MemoryStream ms = new MemoryStream(dlltxt.bytes);

#if UNITY_EDITOR
            //PBD文件 调试数据 日志报错 发布正式版本时可以注释掉
            TextAsset pdbtxt = ResourceManager.Instance.LoadResource<TextAsset>(PDBPath + ".bytes");
            MemoryStream pd = new MemoryStream(pdbtxt.bytes);
            AppDomain.LoadAssembly(ms, pd, new PdbReaderProvider());
#else
        AppDomain.LoadAssembly(ms, null, new PdbReaderProvider());
#endif
        }


        /// <summary>
        /// 初始化
        /// </summary>
        void InitializeILRuntime()
        {
#if UNITY_EDITOR
            AppDomain.DebugService.StartDebugService(56000);
#endif
            RegistDelegate();
            RegisterClass(AppDomain);

        }


        /// <summary>
        /// 调用静态方法
        /// </summary>
        /// <returns></returns>
        public object InvokeStaticMethod(DomainType domainType, string methodName, params object[] param)
        {
#if UNITY_EDITOR
            if (AppDomain == null)
                Init();
#endif

            if (m_DomainInfos.ContainsKey(domainType))
                return AppDomain.Invoke(m_DomainInfos[domainType], methodName, null, param);
            else
            {
                MyDebuger.LogError("InvokeStaticMethod 热更域不包含程序集类型 " + domainType);
                return null;
            }
        }


        /// <summary>
        /// 调用静态方法
        /// </summary>
        /// <returns></returns>
        public object InvokeStaticMethodEx(DomainType domainType, string className, string methodName, params object[] param)
        {
#if UNITY_EDITOR
            if (AppDomain == null)
                Init();
#endif

            if (m_DomainInfos.ContainsKey(domainType))
            {
                MyDebuger.Log(m_DomainInfos[domainType] + "." + className);
                return AppDomain.Invoke(m_DomainInfos[domainType] + "." + className, methodName, null, param);
            }

            else
            {
                MyDebuger.LogErrorFormat("InvokeStaticMethod 热更域不包含程序集类型 {0} classname {1}", domainType, className);
                return null;
            }
        }

        /// <summary>
        /// 实例化对象
        /// </summary>
        /// <param name="domainType"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T GetNewObject<T>(DomainType domainType, params object[] param)
        {
            if (m_DomainInfos.ContainsKey(domainType))
            {
                T obj = AppDomain.Instantiate<T>(m_DomainInfos[domainType], param);
                return obj;
            }
            else
            {
                MyDebuger.LogError("GetNewObject 热更域不包含程序集类型 " + domainType);
                return default(T);
            }
        }

        /// <summary>
        /// 实例化对象
        /// </summary>
        /// <param name="domainType"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public object GetNewObject(DomainType domainType, params object[] param)
        {
            if (m_DomainInfos.ContainsKey(domainType))
            {
                object obj = AppDomain.Instantiate(m_DomainInfos[domainType], param);
                return obj;
            }
            else
            {
                MyDebuger.LogError("GetNewObject 热更域不包含程序集类型 " + domainType);
                return null;
            }
        }


        /// <summary>
        /// 实例化对象
        /// </summary>
        /// <param name="domainType"></param>
        /// <param name="typeName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public object GetNewObjectEx(DomainType domainType, string typeName, params object[] param)
        {
            if (m_DomainInfos.ContainsKey(domainType))
            {
                object obj = AppDomain.Instantiate(m_DomainInfos[domainType] + "." + typeName, param);
                return obj;
            }
            else
            {
                MyDebuger.LogError("GetNewObject 热更域不包含程序集类型 " + domainType + "." + typeName);
                return null;
            }
        }


        /// <summary>
        /// 实例化对象
        /// </summary>
        /// <param name="domainType"></param>
        /// <param name="typeName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T GetNewObjectEx<T>(DomainType domainType, string typeName, params object[] param)
        {
            if (m_DomainInfos.ContainsKey(domainType))
            {
                T obj = AppDomain.Instantiate<T>(m_DomainInfos[domainType] + "." + typeName, param);
                return obj;
            }
            else
            {
                MyDebuger.LogError("GetNewObject 热更域不包含程序集类型 " + domainType + "." + typeName);
                return default(T);
            }
        }


        /// <summary>
        /// 调用对象方法
        /// </summary>
        /// <returns></returns>
        public object InvokeObjectMethod(DomainType domainType, string methodName, object objinstance, params object[] param)
        {
            if (m_DomainInfos.ContainsKey(domainType))
            {
                object obj = AppDomain.Invoke(m_DomainInfos[domainType], methodName, objinstance, param);
                return obj;
            }
            else
            {
                MyDebuger.LogError("InvokeObjectMethod 热更域不包含程序集类型 " + domainType + " 方法名 " + methodName);
                return null;
            }
        }

        /// <summary>
        /// 调用热更dll 方法
        /// </summary>
        /// <param name="nameSpace">命名空间</param>
        /// <param name="typeName">类名</param>
        /// <param name="methodName">方法名</param>
        /// <param name="objinstance">对象</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public object InvokeObjectMethodEx(DomainType nameSpace, string typeName, string methodName, object objinstance, params object[] param)
        {
            if (m_DomainInfos.ContainsKey(nameSpace))
            {
                object obj = AppDomain.Invoke(m_DomainInfos[nameSpace] + "." + typeName, methodName, objinstance, param);
                return obj;
            }
            else
            {
                MyDebuger.LogError("InvokeObjectMethod 热更域不包含程序集类型 " + nameSpace + ":" + typeName + " 方法名 " + methodName);
                return null;
            }
        }


        /// <summary>
        /// 注册委托适配器
        /// </summary>
        void RegistDelegate()
        {
            //在使用自定义委托之前 要先将系统自带委托注册
            AppDomain.DelegateManager.RegisterMethodDelegate<string>();
            AppDomain.DelegateManager.RegisterMethodDelegate<int>();
            AppDomain.DelegateManager.RegisterMethodDelegate<int, bool>();
            AppDomain.DelegateManager.RegisterMethodDelegate<object>();
            AppDomain.DelegateManager.RegisterMethodDelegate<object, object, object>();
            AppDomain.DelegateManager.RegisterFunctionDelegate<int, int>();
            AppDomain.DelegateManager.RegisterFunctionDelegate<bool>();

            AppDomain.DelegateManager.RegisterFunctionDelegate<System.Int32>();
            AppDomain.DelegateManager.RegisterMethodDelegate<System.Single>();
            AppDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Collider>();
            AppDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.GameObject>();
            AppDomain.DelegateManager.RegisterFunctionDelegate<System.Reflection.ConstructorInfo, System.Boolean>();
            AppDomain.DelegateManager.RegisterMethodDelegate<System.Boolean, System.Boolean>();
            AppDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Collision>();
            AppDomain.DelegateManager.RegisterMethodDelegate< UnityEngine.EventSystems.PointerEventData>();
            AppDomain.DelegateManager.RegisterMethodDelegate< System.String, System.Single, System.Int32>();

            AppDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Playables.PlayableDirector>();
            AppDomain.DelegateManager.RegisterMethodDelegate<GersonFrame.InternalMsgAdapter.Adapter>();
            AppDomain.DelegateManager.RegisterMethodDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance>();
            AppDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.ParticleSystem>();
            AppDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.ParticleSystem.MainModule>();
            AppDomain.DelegateManager.RegisterFunctionDelegate<GersonFrame.InternalMsgAdapter.Adapter, System.Boolean>();
            AppDomain.DelegateManager.RegisterMethodDelegate<GersonFrame.CoroutineController>();
            AppDomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Collider, System.Boolean>();
            AppDomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Vector2, System.Boolean>();
            AppDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Transform>();

            AppDomain.DelegateManager.RegisterFunctionDelegate<System.Int32, System.Int32, System.Boolean>();


            AppDomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<System.Reflection.ConstructorInfo>>((act) =>
            {
                return new System.Predicate<System.Reflection.ConstructorInfo>((obj) =>
                {
                    return ((System.Func<System.Reflection.ConstructorInfo, System.Boolean>)act)(obj);
                });
            });

            AppDomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.TweenCallback>((act) =>
            {
                return new DG.Tweening.TweenCallback(() =>
                {
                    ((System.Action)act)();
                });
            });


            AppDomain.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>();
            AppDomain.DelegateManager.RegisterMethodDelegate<System.Boolean, System.String>();
            AppDomain.DelegateManager.RegisterMethodDelegate<System.Boolean>();
            AppDomain.DelegateManager.RegisterMethodDelegate<System.Int32, UnityEngine.RectTransform>();

            LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(AppDomain);


            //==============================Unity自带事件注册===============================================
            AppDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) =>
            {
                return new UnityEngine.Events.UnityAction(() =>
                {
                    ((System.Action)act)();
                });
            });

            //toggle
            AppDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<bool>>((action) =>
            {
                return new UnityEngine.Events.UnityAction<bool>((a) =>
                {
                    ((System.Action<bool>)action)(a);
                });
            });



            AppDomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<ILRuntime.Runtime.Intepreter.ILTypeInstance>>((act) =>
            {
                return new System.Comparison<ILRuntime.Runtime.Intepreter.ILTypeInstance>((x, y) =>
                {
                    return ((System.Func<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>)act)(x, y);
                });
            });


            AppDomain.DelegateManager.RegisterMethodDelegate<System.Boolean, System.String>();
            AppDomain.DelegateManager.RegisterMethodDelegate<System.Boolean>();


            AppDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.EventSystems.BaseEventData>();

            AppDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<UnityEngine.EventSystems.BaseEventData>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<UnityEngine.EventSystems.BaseEventData>((arg0) =>
                {
                    ((System.Action<UnityEngine.EventSystems.BaseEventData>)act)(arg0);
                });
            });

            AppDomain.DelegateManager.RegisterFunctionDelegate<System.Single>();
            AppDomain.DelegateManager.RegisterMethodDelegate<System.Single>();

            AppDomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<System.Single>>((act) =>
            {
                return new DG.Tweening.Core.DOGetter<System.Single>(() =>
                {
                    return ((System.Func<System.Single>)act)();
                });
            });
            AppDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Single>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<System.Single>((arg0) =>
                {
                    ((System.Action<System.Single>)act)(arg0);
                });
            });

            AppDomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOSetter<System.Single>>((act) =>
            {
                return new DG.Tweening.Core.DOSetter<System.Single>((pNewValue) =>
                {
                    ((System.Action<System.Single>)act)(pNewValue);
                });
            });

            AppDomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<System.Int32>>((act) =>
            {
                return new DG.Tweening.Core.DOGetter<System.Int32>(() =>
                {
                    return ((System.Func<System.Int32>)act)();
                });
            });
            AppDomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOSetter<System.Int32>>((act) =>
            {
                return new DG.Tweening.Core.DOSetter<System.Int32>((pNewValue) =>
                {
                    ((System.Action<System.Int32>)act)(pNewValue);
                });
            });
            AppDomain.DelegateManager.RegisterFunctionDelegate<System.Int64>();
            AppDomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOSetter<System.Int64>>((act) =>
            {
                return new DG.Tweening.Core.DOSetter<System.Int64>((pNewValue) =>
                {
                    ((System.Action<System.Int64>)act)(pNewValue);
                });
            });

            AppDomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<System.Int64>>((act) =>
            {
                return new DG.Tweening.Core.DOGetter<System.Int64>(() =>
                {
                    return ((System.Func<System.Int64>)act)();
                });
            });

            AppDomain.DelegateManager.RegisterMethodDelegate<System.Int64>();

            AppDomain.DelegateManager.RegisterFunctionDelegate<System.Int32, System.Int32, System.Int32>();

            AppDomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<System.Int32>>((act) =>
            {
                return new System.Comparison<System.Int32>((x, y) =>
                {
                    return ((System.Func<System.Int32, System.Int32, System.Int32>)act)(x, y);
                });
            });

            AppDomain.DelegateManager.RegisterFunctionDelegate<System.String>();
            AppDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.String>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<System.String>((arg0) =>
                {
                    ((System.Action<System.String>)act)(arg0);
                });
            });

            AppDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.SystemLanguage>();
            AppDomain.DelegateManager.RegisterDelegateConvertor<Localization.LocalizationManager.LanguageChange>((act) =>
            {
                return new Localization.LocalizationManager.LanguageChange((newLanguage) =>
                {
                    ((System.Action<UnityEngine.SystemLanguage>)act)(newLanguage);
                });
            });
            AppDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Vector2>();
            AppDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<UnityEngine.Vector2>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<UnityEngine.Vector2>((arg0) =>
                {
                    ((System.Action<UnityEngine.Vector2>)act)(arg0);
                });
            });


            ///proto相关

           AppDomain.DelegateManager.RegisterFunctionDelegate<global::Adapt_IMessage.Adaptor>();
            AppDomain.DelegateManager.RegisterFunctionDelegate<global::Adapt_IMessage.Adaptor, global::Adapt_IMessage.Adaptor, System.Int32>();
            AppDomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<global::Adapt_IMessage.Adaptor>>((act) =>
            {
                return new System.Comparison<global::Adapt_IMessage.Adaptor>((x, y) =>
                {
                    return ((System.Func<global::Adapt_IMessage.Adaptor, global::Adapt_IMessage.Adaptor, System.Int32>)act)(x, y);
                });
            });




        }


        public unsafe static void RegisterClass(AppDomain domain)
        {
            //这里需要注册所有热更DLL中用到的跨域继承Adapter，否则无法正确抓取引用
            //  domain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
            domain.RegisterCrossBindingAdaptor(new Adapt_IMessage());
            domain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
            domain.RegisterCrossBindingAdaptor(new HotfixVIewAdapter());
            domain.RegisterCrossBindingAdaptor(new ViewElementAdapter());
            domain.RegisterCrossBindingAdaptor(new InternalMsgAdapter());
            SetupCLRRedirection(domain);
            MyDebuger_ILRunTimeBinding.Register(domain);
            //这里做一些ILRuntime的注册，这里我们注册值类型Binder，注释和解注下面的代码来对比性能差别 降低值类型的装箱和拆箱 减少GC
            domain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());
            domain.RegisterValueTypeBinder(typeof(Quaternion), new QuaternionBinder());
            domain.RegisterValueTypeBinder(typeof(Vector2), new Vector2Binder());
            //CLR 绑定注册
       		ILRuntime.Runtime.Generated.CLRBindings.Initialize(domain);  Debug.LogWarning("+++++++++++++++++++绑定完成++++++++++");
        }


        static unsafe void SetupCLRRedirection(AppDomain domain)
        {
            //这里面的通常应该写在InitializeILRuntime，这里为了演示写这里
            var arr = typeof(GameObject).GetMethods();
            foreach (var i in arr)
            {
                if (i.Name == "AddComponent" && i.GetGenericArguments().Length == 1)
                {
                    domain.RegisterCLRMethodRedirection(i, AddComponent);
                }
                else if (i.Name == "GetComponent" && i.GetGenericArguments().Length == 1)
                {
                    domain.RegisterCLRMethodRedirection(i, GetComponent);
                }
            }



        }


        unsafe static StackObject* AddComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            //CLR重定向的说明请看相关文档和教程，这里不多做解释
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;

            var ptr = __esp - 1;
            //成员方法的第一个参数为this
            GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
            if (instance == null)
                throw new System.NullReferenceException();
            __intp.Free(ptr);

            var genericArgument = __method.GenericArguments;
            //AddComponent应该有且只有1个泛型参数
            if (genericArgument != null && genericArgument.Length == 1)
            {
                var type = genericArgument[0];
                object res;
                if (type is CLRType)
                {
                    //Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                    res = instance.AddComponent(type.TypeForCLR);
                }
                else
                {
                    //热更DLL内的类型比较麻烦。首先我们得自己手动创建实例
                    var ilInstance = new ILTypeInstance(type as ILType, false);//手动创建实例是因为默认方式会new MonoBehaviour，这在Unity里不允许
                                                                               //接下来创建Adapter实例
                    var clrInstance = instance.AddComponent<MonoBehaviourAdapter.Adaptor>();
                    //unity创建的实例并没有热更DLL里面的实例，所以需要手动赋值
                    clrInstance.ILInstance = ilInstance;
                    clrInstance.AppDomain = __domain;
                    //这个实例默认创建的CLRInstance不是通过AddComponent出来的有效实例，所以得手动替换
                    ilInstance.CLRInstance = clrInstance;

                    res = clrInstance.ILInstance;//交给ILRuntime的实例应该为ILInstance

                    clrInstance.Awake();//因为Unity调用这个方法时还没准备好所以这里补调一次
                }

                return ILIntepreter.PushObject(ptr, __mStack, res);
            }

            return __esp;
        }

        unsafe static StackObject* GetComponent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            //CLR重定向的说明请看相关文档和教程，这里不多做解释
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;

            var ptr = __esp - 1;
            //成员方法的第一个参数为this
            GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
            if (instance == null)
                throw new System.NullReferenceException();
            __intp.Free(ptr);

            var genericArgument = __method.GenericArguments;
            //AddComponent应该有且只有1个泛型参数
            if (genericArgument != null && genericArgument.Length == 1)
            {
                var type = genericArgument[0];
                object res = null;
                if (type is CLRType)
                {
                    //Unity主工程的类不需要任何特殊处理，直接调用Unity接口
                    res = instance.GetComponent(type.TypeForCLR);
                }
                else
                {
                    //因为所有DLL里面的MonoBehaviour实际都是这个Component，所以我们只能全取出来遍历查找
                    var clrInstances = instance.GetComponents<MonoBehaviourAdapter.Adaptor>();
                    for (int i = 0; i < clrInstances.Length; i++)
                    {
                        var clrInstance = clrInstances[i];
                        if (clrInstance.ILInstance != null)//ILInstance为null, 表示是无效的MonoBehaviour，要略过
                        {
                            if (clrInstance.ILInstance.Type == type)
                            {
                                res = clrInstance.ILInstance;//交给ILRuntime的实例应该为ILInstance
                                break;
                            }
                        }
                    }
                }

                return ILIntepreter.PushObject(ptr, __mStack, res);
            }

            return __esp;
        }



    }
}
