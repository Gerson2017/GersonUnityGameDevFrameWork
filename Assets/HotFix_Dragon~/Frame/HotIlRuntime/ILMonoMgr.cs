using GersonFrame.SelfILRuntime;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HotGersonFrame.HotIlRuntime
{
    public enum MonoType
    {
        IAwake = 0,
        IOnEnable = 2,
        IStart = 4,
        IAnimationFunction = 6,
        ITriggerEnter = 7,
        ITriggerStay = 8,
        ITriggerExit = 9,
        ICollisionEnter = 10,
        ICollisionStay = 11,
        ICollisionExit = 12,
        IFixedUpdate = 13,
        IUpdate = 15,
        ILateUpdate = 17,
        IDrawGizmos = 20,
        IOnDisable = 25,
        IOnDestroy = 30,
        IParticleTrigger = 35,
        IParticleSysStop = 40,
        IPointerDown = 45,
        IPointerUp = 50,
        IPointerExit = 51,
        IPointerDrag = 55
    }

    public class ILMonoMgr : HotSingleton<ILMonoMgr>
    {

        private bool m_hasInit = false;

        /// <summary>
        /// 用到的类型 排好序
        /// </summary>
        private Dictionary<Type, int> m_monoCallBackOrdeDic = new Dictionary<Type, int>();

        /// <summary>
        /// 缓存检查组件的添加
        /// </summary>
        public Dictionary<MonoType, Type> mCheckAddCompendentDic { get; private set; } = new Dictionary<MonoType, Type>();

        /// <summary>
        /// 获取组件执行顺序
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int GetTypeOrder(Type type)
        {
            if (m_monoCallBackOrdeDic.ContainsKey(type))
                return m_monoCallBackOrdeDic[type];
            else
            {
                MyDebuger.LogError("GetTypeOrder not found type " + type);
                return 1000;
            }
        }

        private ILMonoMgr()
        {
            if (m_hasInit) return;
            InitOrder();
            m_hasInit = true;
        }

        void InitOrder()
        {
            SetTypeOrderAndCompendentDic(MonoType.IAwake, typeof(IAwake));
            SetTypeOrderAndCompendentDic(MonoType.IOnEnable, typeof(IOnEnable));
            SetTypeOrderAndCompendentDic(MonoType.IStart, typeof(IStart));
            SetTypeOrderAndCompendentDic(MonoType.IAnimationFunction, typeof(IAnimationFunction));
            SetTypeOrderAndCompendentDic(MonoType.ITriggerEnter, typeof(ITriggerEnter));
            SetTypeOrderAndCompendentDic(MonoType.ITriggerStay, typeof(ITriggerStay));
            SetTypeOrderAndCompendentDic(MonoType.ITriggerExit, typeof(ITriggerExit));
            SetTypeOrderAndCompendentDic(MonoType.ICollisionEnter, typeof(ICollisionEnter));
            SetTypeOrderAndCompendentDic(MonoType.ICollisionStay, typeof(ICollisionStay));
            SetTypeOrderAndCompendentDic(MonoType.ICollisionExit, typeof(ICollisionExit));
            SetTypeOrderAndCompendentDic(MonoType.IFixedUpdate, typeof(IFixedUpdate));
            SetTypeOrderAndCompendentDic(MonoType.IUpdate, typeof(IUpdate));
            SetTypeOrderAndCompendentDic(MonoType.ILateUpdate, typeof(ILateUpdate));
            SetTypeOrderAndCompendentDic(MonoType.IDrawGizmos, typeof(IDrawGizmos));
            SetTypeOrderAndCompendentDic(MonoType.IOnDisable, typeof(IOnDisable));
            SetTypeOrderAndCompendentDic(MonoType.IOnDestroy, typeof(IOnDestroy));
            SetTypeOrderAndCompendentDic(MonoType.IParticleTrigger, typeof(IParticleTrigger));
            SetTypeOrderAndCompendentDic(MonoType.IParticleSysStop, typeof(IParticleSysStop));
            SetTypeOrderAndCompendentDic(MonoType.IPointerDown, typeof(IPointerDown));
            SetTypeOrderAndCompendentDic(MonoType.IPointerUp, typeof(IPointerUp));
            SetTypeOrderAndCompendentDic(MonoType.IPointerExit, typeof(IPointerExit));
            SetTypeOrderAndCompendentDic(MonoType.IPointerDrag, typeof(IPointerDrag));

        }

        void SetTypeOrderAndCompendentDic(MonoType mtype, Type type)
        {
            m_monoCallBackOrdeDic[type] = (int)mtype;
            mCheckAddCompendentDic[mtype] = type;
        }




        private class ILMonoContainer
        {

            private int m_gameObjId;
            private GameObject m_gameobject;


            public bool mCanUseMonoEvt { get; private set; } = true;

            /// <summary>
            /// 设置是否可以是Mono 事件
            /// </summary>
            /// <param name="canuse"></param>
            public void SetCanUseMonoEvt(bool canuse)
            {
                mCanUseMonoEvt = canuse;
            }

            public ILMonoContainer(int goinstanceId, GameObject go)
            {
                this.m_gameObjId = goinstanceId;
                this.m_gameobject = go;
            }

            /// <summary>
            /// 用到的类型 排好序
            /// </summary>
            private List<Type> m_monoCallBackOrderTypes = new List<Type>();






            /// <summary>
            /// Mono实例 支持一对多
            /// </summary>
            public Dictionary<Type, List<object>> mIMonoInstances = new Dictionary<Type, List<object>>();


            /// <summary>
            /// 一般对象实例 一对一
            /// </summary>
            public Dictionary<Type, object> mInstances = new Dictionary<Type, object>();

            /// <summary>
            /// 注册实例
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="instance"></param>
            public void RegisterIMono<T>(T instance) where T : IMono
            {
                if (instance == null)
                    return;
                var key = typeof(T);
                //组件
                if (!mIMonoInstances.ContainsKey(key))
                    mIMonoInstances[key] = new List<object>();

                m_monoCallBackOrderTypes.Add(key);
                if (!mIMonoInstances[key].Contains(instance))
                    mIMonoInstances[key].Add(instance);
            }



            /// <summary>
            /// 注册实例
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="instance"></param>
            public void Register<T>(T instance)
            {
                var key = typeof(T);
                mInstances[key] = instance;
            }


            /// <summary>
            /// 取消Mono实例Msg监听
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="instance"></param>
            public bool UnRegisterIMono<T>(T instance) where T : IMono
            {
                var typekey = typeof(T);

                if (mIMonoInstances.ContainsKey(typekey))
                {
                    if (mIMonoInstances[typekey].Contains(instance))
                    {
                        RemoveMonoCallBack<T>(typekey, instance);
                        mIMonoInstances[typekey].Remove(instance);
                        return true;
                    }

                    return false;
                }
                else
                {
                    MyDebuger.LogWarning(this.m_gameobject.name + "can not found");
                    return false;
                }
            }


            /// <summary>
            /// 注册实例
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="instance"></param>
            public bool UnRegister<T>()
            {
                var key = typeof(T);
                if (mIMonoInstances.ContainsKey(key))
                {
                    mIMonoInstances.Remove(key);
                    return true;
                }
                else
                {
                    MyDebuger.LogWarning(this.m_gameobject.name + "can not found");
                    return false;
                }
            }

            public void UnRegisterAll()
            {
                mIMonoInstances.Clear();
                mInstances.Clear();
            }


            /// <summary>
            /// 是否包含某组件
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public bool Contains<T>()
            {
                var key = typeof(T);
                return mInstances.ContainsKey(key);
            }


            /// <summary>
            /// 获取
            /// </summary>
            public T Get<T>() where T : class
            {
                var key = typeof(T);
                if (key is IMono)
                    MyDebuger.LogError("获取 IMono 类 请使用 GetILMono<T>");
                object retObj;
                if (mInstances.TryGetValue(key, out retObj))
                    return retObj as T;
                else
                    return null;
            }


            /// <summary>
            /// 获取
            /// </summary>
            public List<T> GetILMono<T>() where T : class, IMono
            {
                var key = typeof(T);
                List<object> ilinstances;
                if (mIMonoInstances.TryGetValue(key, out ilinstances))
                    return ilinstances as List<T>;
                else
                    return null;
            }


            public void AddCompentet()
            {
                for (int i = 0; i < m_monoCallBackOrderTypes.Count; i++)
                {
                    CheckAddMonoCompent(m_monoCallBackOrderTypes[i]);
                }
            }


            void CheckAddMonoCompent(Type type)
            {
                if (mIMonoInstances.ContainsKey(type))
                {
                    if (type == Instance.mCheckAddCompendentDic[MonoType.IAwake])
                    {
                        MonoAwake monoAwake = this.m_gameobject.GetCompententOrNew<MonoAwake>();

                        for (int i = 0; i < mIMonoInstances[type].Count; i++)
                        {
                            Action awakecallback = (mIMonoInstances[type][i] as IAwake).Awake;
                            monoAwake.AddCallBack(awakecallback);
                            if (m_gameobject.activeInHierarchy)
                                awakecallback();
                        }

                    }
                    else if (type == Instance.mCheckAddCompendentDic[MonoType.IOnEnable])
                    {
                        MonoEnable monoEnable = this.m_gameobject.GetCompententOrNew<MonoEnable>();
                        for (int i = 0; i < mIMonoInstances[type].Count; i++)
                        {
                            Action enablecallback = (mIMonoInstances[type][i] as IOnEnable).OnEnable;
                            monoEnable.AddCallBack(enablecallback);
                            if (m_gameobject.activeInHierarchy)
                                enablecallback();
                        }

                    }
                    else if (type == Instance.mCheckAddCompendentDic[MonoType.IStart])
                    {
                        MonoStart monoStart = this.m_gameobject.GetCompententOrNew<MonoStart>();
                        for (int i = 0; i < mIMonoInstances[type].Count; i++)
                        {
                            Action startcallback = (mIMonoInstances[type][i] as IStart).Start;
                            monoStart.AddCallBack(startcallback);
                        }
                    }
                    else if (type == Instance.mCheckAddCompendentDic[MonoType.IUpdate])
                    {
                        MonoUpdate monoUpdate = this.m_gameobject.GetCompententOrNew<MonoUpdate>();
                        for (int i = 0; i < mIMonoInstances[type].Count; i++)
                            monoUpdate.AddCallBack((mIMonoInstances[type][i] as IUpdate).Update);

                    }
                    else if (type == Instance.mCheckAddCompendentDic[MonoType.IFixedUpdate])
                    {
                        MonoFixedUpdate monoFixedUpdate = this.m_gameobject.GetCompententOrNew<MonoFixedUpdate>();
                        for (int i = 0; i < mIMonoInstances[type].Count; i++)
                            monoFixedUpdate.AddCallBack((mIMonoInstances[type][i] as IFixedUpdate).FixedUpdate);
                    }
                    else if (type == Instance.mCheckAddCompendentDic[MonoType.ILateUpdate])
                    {
                        MonoLateUpdate monoLateUpdate = this.m_gameobject.GetCompententOrNew<MonoLateUpdate>();
                        for (int i = 0; i < mIMonoInstances[type].Count; i++)
                            monoLateUpdate.AddCallBack((mIMonoInstances[type][i] as ILateUpdate).LateUpdate);
                    }
                    else if (type == Instance.mCheckAddCompendentDic[MonoType.ITriggerEnter])
                    {
                        MonoTriggerEnter monoTriggerEnter = this.m_gameobject.GetCompententOrNew<MonoTriggerEnter>();

                        for (int i = 0; i < mIMonoInstances[type].Count; i++)
                            monoTriggerEnter.AddCallBack((mIMonoInstances[type][i] as ITriggerEnter).OnTriggerEnter);

                    }
                    else if (type == Instance.mCheckAddCompendentDic[MonoType.ITriggerStay])
                    {
                        MonoTriggerStay monoTriggerStay = this.m_gameobject.GetCompententOrNew<MonoTriggerStay>();

                        for (int i = 0; i < mIMonoInstances[type].Count; i++)
                            monoTriggerStay.AddCallBack((mIMonoInstances[type][i] as ITriggerStay).OnTriggerStay);

                    }
                    else if (type == Instance.mCheckAddCompendentDic[MonoType.ITriggerExit])
                    {
                        MonoTriggerExit monoTriggerExit = this.m_gameobject.GetCompententOrNew<MonoTriggerExit>();
                        for (int i = 0; i < mIMonoInstances[type].Count; i++)
                            monoTriggerExit.AddCallBack((mIMonoInstances[type][i] as ITriggerExit).OnTriggerExit);
                    }
                    else if (type == Instance.mCheckAddCompendentDic[MonoType.ICollisionEnter])
                    {
                        MonoCollisionEnter monoCollisionEnte = this.m_gameobject.GetCompententOrNew<MonoCollisionEnter>();

                        for (int i = 0; i < mIMonoInstances[type].Count; i++)
                            monoCollisionEnte.AddCallBack((mIMonoInstances[type][i] as ICollisionEnter).OnCollisionEnter);

                    }
                    else if (type == Instance.mCheckAddCompendentDic[MonoType.ICollisionStay])
                    {
                        MonoCollisionStay monoCollisionStay = this.m_gameobject.GetCompententOrNew<MonoCollisionStay>();

                        for (int i = 0; i < mIMonoInstances[type].Count; i++)
                            monoCollisionStay.AddCallBack((mIMonoInstances[type][i] as ICollisionStay).OnCollisionStay);

                    }
                    else if (type == Instance.mCheckAddCompendentDic[MonoType.ICollisionExit])
                    {
                        MonoCollisionExit monoCollisionExit = this.m_gameobject.GetCompententOrNew<MonoCollisionExit>();


                        for (int i = 0; i < mIMonoInstances[type].Count; i++)
                            monoCollisionExit.AddCallBack((mIMonoInstances[type][i] as ICollisionExit).OnCollisionExit);

                    }
                    else if (type == Instance.mCheckAddCompendentDic[MonoType.IOnDisable])
                    {
                        MonoOnDisable monoOnDisable = this.m_gameobject.GetCompententOrNew<MonoOnDisable>();


                        for (int i = 0; i < mIMonoInstances[type].Count; i++)
                            monoOnDisable.AddCallBack((mIMonoInstances[type][i] as IOnDisable).OnDisable);

                    }
                    else if (type == Instance.mCheckAddCompendentDic[MonoType.IOnDestroy])
                    {
                        MonoOnDestroy monoOnDestroy = this.m_gameobject.GetCompententOrNew<MonoOnDestroy>();


                        for (int i = 0; i < mIMonoInstances[type].Count; i++)
                            monoOnDestroy.AddCallBack((mIMonoInstances[type][i] as IOnDestroy).OnDestroy);

                    }
                    else if (type == Instance.mCheckAddCompendentDic[MonoType.IAnimationFunction])
                    {
                        MonoAnimationFunction monoAnimationFunction = this.m_gameobject.GetCompententOrNew<MonoAnimationFunction>();


                        for (int i = 0; i < mIMonoInstances[type].Count; i++)
                            monoAnimationFunction.AddCallBack((mIMonoInstances[type][i] as IAnimationFunction).OnAnimaitonEvtInvoke);

                    }
                    else if (type == Instance.mCheckAddCompendentDic[MonoType.IParticleSysStop])
                    {
                        MonoParticleSystemStop monoParticleSystemStop = this.m_gameobject.GetCompententOrNew<MonoParticleSystemStop>();


                        for (int i = 0; i < mIMonoInstances[type].Count; i++)
                            monoParticleSystemStop.AddCallBack((mIMonoInstances[type][i] as IParticleSysStop).OnParticleSystemStopped);

                    }
                    else if (type == Instance.mCheckAddCompendentDic[MonoType.IParticleTrigger])
                    {
                        MonoParticleTrigger monoParticleTrigger = this.m_gameobject.GetCompententOrNew<MonoParticleTrigger>();


                        for (int i = 0; i < mIMonoInstances[type].Count; i++)
                            monoParticleTrigger.AddCallBack((mIMonoInstances[type][i] as IParticleTrigger).OnParticleTrigger);

                    }
                    else if (type == Instance.mCheckAddCompendentDic[MonoType.IPointerDown])
                    {
                        MonoPointerDown monoPointerDown = this.m_gameobject.GetCompententOrNew<MonoPointerDown>();


                        for (int i = 0; i < mIMonoInstances[type].Count; i++)
                            monoPointerDown.AddCallBack((mIMonoInstances[type][i] as IPointerDown).OnPointerDown);

                    }
                    else if (type == Instance.mCheckAddCompendentDic[MonoType.IPointerUp])
                    {
                        MonoPointerUp monoPointerUp = this.m_gameobject.GetCompententOrNew<MonoPointerUp>();


                        for (int i = 0; i < mIMonoInstances[type].Count; i++)
                            monoPointerUp.AddCallBack((mIMonoInstances[type][i] as IPointerUp).OnPointerUp);

                    }
                    else if (type == Instance.mCheckAddCompendentDic[MonoType.IPointerExit])
                    {
                        MonoPointerExit monoOnPointerExit = this.m_gameobject.GetCompententOrNew<MonoPointerExit>();

                        for (int i = 0; i < mIMonoInstances[type].Count; i++)
                            monoOnPointerExit.AddCallBack((mIMonoInstances[type][i] as IPointerExit).OnPointerExit);

                    }
                    else if (type == Instance.mCheckAddCompendentDic[MonoType.IPointerDrag])
                    {
                        MonoPointDrag monoOnPointerDrag = this.m_gameobject.GetCompententOrNew<MonoPointDrag>();


                        for (int i = 0; i < mIMonoInstances[type].Count; i++)
                            monoOnPointerDrag.AddCallBack((mIMonoInstances[type][i] as IPointerDrag).OnDrag);

                    }
                    else if (type == Instance.mCheckAddCompendentDic[MonoType.IDrawGizmos])
                    {
                        if (Application.isEditor)
                        {
                            MonoOnDrawGizmos monoOnDrawGizmos = this.m_gameobject.GetCompententOrNew<MonoOnDrawGizmos>();


                            for (int i = 0; i < mIMonoInstances[type].Count; i++)
                                monoOnDrawGizmos.AddCallBack((mIMonoInstances[type][i] as IDrawGizmos).OnDrawGizmos);

                        }
                    }
                    else
                        MyDebuger.LogError(" not found addcompent " + this.m_gameobject.name + type.ToString());

                    mIMonoInstances[type].Clear();
                }
            }


            void RemoveMonoCallBack<T>(Type type, T mono) where T : IMono
            {
                if (type == Instance.mCheckAddCompendentDic[MonoType.IAwake])
                {
                    MonoAwake monoAwake = this.m_gameobject.GetComponent<MonoAwake>();
                    if (monoAwake != null)
                        monoAwake.RemoveCallback((mono as IAwake).Awake);
                }
                else if (type == Instance.mCheckAddCompendentDic[MonoType.IOnEnable])
                {
                    MonoEnable monoEnable = this.m_gameobject.GetComponent<MonoEnable>();
                    if (monoEnable != null)
                        monoEnable.RemoveCallback((mono as IOnEnable).OnEnable);
                }
                else if (type == Instance.mCheckAddCompendentDic[MonoType.IStart])
                {
                    MonoStart monoStart = this.m_gameobject.GetComponent<MonoStart>();
                    if (monoStart != null)
                        monoStart.RemoveCallback((mono as IStart).Start);
                }
                else if (type == Instance.mCheckAddCompendentDic[MonoType.IUpdate])
                {
                    MonoUpdate monoUpdate = this.m_gameobject.GetComponent<MonoUpdate>();
                    if (monoUpdate != null)
                        monoUpdate.RemoveCallback((mono as IUpdate).Update);
                }
                else if (type == Instance.mCheckAddCompendentDic[MonoType.IFixedUpdate])
                {
                    MonoFixedUpdate monoFixedUpdate = this.m_gameobject.GetComponent<MonoFixedUpdate>();
                    if (monoFixedUpdate != null)
                        monoFixedUpdate.RemoveCallback((mono as IFixedUpdate).FixedUpdate);
                }
                else if (type == Instance.mCheckAddCompendentDic[MonoType.ILateUpdate])
                {
                    MonoLateUpdate monoLateUpdate = this.m_gameobject.GetComponent<MonoLateUpdate>();
                    if (monoLateUpdate != null)
                        monoLateUpdate.RemoveCallback((mono as ILateUpdate).LateUpdate);
                }
                else if (type == Instance.mCheckAddCompendentDic[MonoType.ITriggerEnter])
                {
                    MonoTriggerEnter monoTriggerEnter = this.m_gameobject.GetComponent<MonoTriggerEnter>();
                    if (monoTriggerEnter != null)
                        monoTriggerEnter.RemoveCallback((mono as ITriggerEnter).OnTriggerEnter);
                }
                else if (type == Instance.mCheckAddCompendentDic[MonoType.ITriggerStay])
                {
                    MonoTriggerStay monoTriggerStay = this.m_gameobject.GetComponent<MonoTriggerStay>();
                    if (monoTriggerStay != null)
                        monoTriggerStay.RemoveCallback((mono as ITriggerStay).OnTriggerStay);
                }
                else if (type == Instance.mCheckAddCompendentDic[MonoType.ITriggerExit])
                {
                    MonoTriggerExit monoTriggerExit = this.m_gameobject.GetComponent<MonoTriggerExit>();
                    if (monoTriggerExit != null)
                        monoTriggerExit.RemoveCallback((mono as ITriggerExit).OnTriggerExit);
                }
                else if (type == Instance.mCheckAddCompendentDic[MonoType.ICollisionEnter])
                {
                    MonoCollisionEnter monoCollisionEnte = this.m_gameobject.GetComponent<MonoCollisionEnter>();
                    if (monoCollisionEnte != null)
                        monoCollisionEnte.RemoveCallback((mono as ICollisionEnter).OnCollisionEnter);
                }
                else if (type == Instance.mCheckAddCompendentDic[MonoType.ICollisionStay])
                {
                    MonoCollisionStay monoCollisionStay = this.m_gameobject.GetComponent<MonoCollisionStay>();
                    if (monoCollisionStay != null)
                        monoCollisionStay.RemoveCallback((mono as ICollisionStay).OnCollisionStay);
                }
                else if (type == Instance.mCheckAddCompendentDic[MonoType.ICollisionExit])
                {
                    MonoCollisionExit monoCollisionExit = this.m_gameobject.GetComponent<MonoCollisionExit>();
                    if (monoCollisionExit != null)
                        monoCollisionExit.RemoveCallback((mono as ICollisionExit).OnCollisionExit);
                }
                else if (type == Instance.mCheckAddCompendentDic[MonoType.IOnDisable])
                {
                    MonoOnDisable monoOnDisable = this.m_gameobject.GetComponent<MonoOnDisable>();
                    if (monoOnDisable != null)
                        monoOnDisable.RemoveCallback((mono as IOnDisable).OnDisable);
                }
                else if (type == Instance.mCheckAddCompendentDic[MonoType.IOnDestroy])
                {
                    MonoOnDestroy monoOnDestroy = this.m_gameobject.GetComponent<MonoOnDestroy>();
                    if (monoOnDestroy != null)
                        monoOnDestroy.RemoveCallback((mono as IOnDestroy).OnDestroy);
                }
                else if (type == Instance.mCheckAddCompendentDic[MonoType.IAnimationFunction])
                {
                    MonoAnimationFunction monoAnimationFunction = this.m_gameobject.GetComponent<MonoAnimationFunction>();
                    if (monoAnimationFunction != null)
                        monoAnimationFunction.RemoveCallback((mono as IAnimationFunction).OnAnimaitonEvtInvoke);
                }
                else if (type == Instance.mCheckAddCompendentDic[MonoType.IParticleSysStop])
                {
                    MonoParticleSystemStop monoParticleSystemStop = this.m_gameobject.GetComponent<MonoParticleSystemStop>();
                    if (monoParticleSystemStop != null)
                        monoParticleSystemStop.RemoveCallback((mono as IParticleSysStop).OnParticleSystemStopped);
                }
                else if (type == Instance.mCheckAddCompendentDic[MonoType.IParticleTrigger])
                {
                    MonoParticleTrigger monoParticleTrigger = this.m_gameobject.GetComponent<MonoParticleTrigger>();
                    if (monoParticleTrigger != null)
                        monoParticleTrigger.RemoveCallback((mono as IParticleTrigger).OnParticleTrigger);
                }
                else if (type == Instance.mCheckAddCompendentDic[MonoType.IPointerDown])
                {
                    MonoPointerDown monoPointerDown = this.m_gameobject.GetComponent<MonoPointerDown>();
                    if (monoPointerDown != null)
                        monoPointerDown.RemoveCallback((mono as IPointerDown).OnPointerDown);
                }
                else if (type == Instance.mCheckAddCompendentDic[MonoType.IPointerUp])
                {
                    MonoPointerUp monoPointerUp = this.m_gameobject.GetComponent<MonoPointerUp>();
                    if (monoPointerUp != null)
                        monoPointerUp.RemoveCallback((mono as IPointerUp).OnPointerUp);
                }
                else if (type == Instance.mCheckAddCompendentDic[MonoType.IPointerExit])
                {
                    MonoPointerExit monoOnPointerExit = this.m_gameobject.GetComponent<MonoPointerExit>();
                    if (monoOnPointerExit != null)
                        monoOnPointerExit.RemoveCallback((mono as IPointerExit).OnPointerExit);
                }
                else if (type == Instance.mCheckAddCompendentDic[MonoType.IPointerDrag])
                {
                    MonoPointDrag monoOnPointerDrag = this.m_gameobject.GetComponent<MonoPointDrag>();
                    if (monoOnPointerDrag != null)
                        monoOnPointerDrag.RemoveCallback((mono as IPointerDrag).OnDrag);
                }
                else if (type == Instance.mCheckAddCompendentDic[MonoType.IDrawGizmos])
                {
                    if (Application.isEditor)
                    {
                        MonoOnDrawGizmos monoOnDrawGizmos = this.m_gameobject.GetComponent<MonoOnDrawGizmos>();
                        if (monoOnDrawGizmos != null)
                            monoOnDrawGizmos.RemoveCallback((mono as IDrawGizmos).OnDrawGizmos);
                    }
                }
                else
                    MyDebuger.LogError(" not found addcompent " + this.m_gameobject.name + mono.ToString());

            }

        }

        private Dictionary<int, ILMonoContainer> m_IMOnoDic = new Dictionary<int, ILMonoContainer>();


        public void SetCanUseMonoEvt(GameObject go, bool canuse)
        {
            int instanceid = go.GetInstanceID();
            SetCanUseMonoEvt(instanceid, canuse);
        }
        public void SetCanUseMonoEvt(int gameobjectInstanceId, bool canuse)
        {
            ILMonoContainer container;
            if (m_IMOnoDic.TryGetValue(gameobjectInstanceId, out container))
                container.SetCanUseMonoEvt(canuse);
            else
                MyDebuger.LogError("SetCanUseMonoEvt Fail  not found Container " + gameobjectInstanceId);
        }


        public bool ContainsInstance<T>(GameObject go)
        {
            int gameobjectInstacneID = go.GetInstanceID();
            ILMonoContainer container;
            if (m_IMOnoDic.TryGetValue(gameobjectInstacneID, out container))
            {
                return container.Contains<T>();
            }
            return false;

        }

        /// <summary>
        /// 注册要添加的组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <param name="imonoObj"></param>
        public void RegisterIMono<T>(GameObject go, T imonoObj) where T : class, IMono
        {
            int gameobjectInstacneID = go.GetInstanceID();
            if (!m_IMOnoDic.ContainsKey(gameobjectInstacneID))
                m_IMOnoDic.Add(gameobjectInstacneID, new ILMonoContainer(gameobjectInstacneID, go));
            m_IMOnoDic[gameobjectInstacneID].RegisterIMono(imonoObj);

        }


        public void Register<T>(GameObject go, T imonoObj) where T : class
        {
            int gameobjectInstacneID = go.GetInstanceID();
            if (!m_IMOnoDic.ContainsKey(gameobjectInstacneID))
                m_IMOnoDic[gameobjectInstacneID]= new ILMonoContainer(gameobjectInstacneID, go);
            m_IMOnoDic[gameobjectInstacneID].Register(imonoObj);
        }

        /// <summary>
        /// 所有注册结束开始添加组件
        /// </summary>
        /// <param name="go"></param>
        public void RegisterOver(GameObject go)
        {
            int gameobjectInstacneID = go.GetInstanceID();
            if (m_IMOnoDic.ContainsKey(gameobjectInstacneID))
                m_IMOnoDic[gameobjectInstacneID].AddCompentet();
            else
                MyDebuger.LogError(go.name + " not register any MonoEvt");
        }


        /// <summary>
        /// 取消非IMono 类型的注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        public void UnRegister<T>(GameObject go)
        {
            int gameobjectInstacneID = go.GetInstanceID();
            if (m_IMOnoDic.ContainsKey(gameobjectInstacneID))
                m_IMOnoDic[gameobjectInstacneID].UnRegister<T>();
        }

        /// <summary>
        /// 取消注册组件 对应Mono 组件 只会取消监听 不会删除主域里的组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameobjectInstacneID"></param>
        public void UnRegisterIMono<T>(GameObject go, T instance) where T : class, IMono
        {
            int gameobjectInstacneID = go.GetInstanceID();
            if (m_IMOnoDic.ContainsKey(gameobjectInstacneID))
                m_IMOnoDic[gameobjectInstacneID].UnRegisterIMono<T>(instance);
        }


        /// <summary>
        /// 取消所有注册组件 对应Mono 组件 只会取消监听 不会删除主域里的组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameobjectInstacneID"></param>
        public void UnRegisterAll(GameObject go)
        {
            int gameobjectInstacneID = go.GetInstanceID();
            if (m_IMOnoDic.ContainsKey(gameobjectInstacneID))
                m_IMOnoDic[gameobjectInstacneID].UnRegisterAll();
        }


        /// <summary>
        /// 获取指定类型的对象实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <returns></returns>
        public T GetInstance<T>(GameObject go) where T : class
        {
            if (ReferenceEquals(go, null))
            {
                MyDebuger.LogError($"GetInstance {typeof(T)} is null");
                return null;
            }
            int instanceId = go.GetInstanceID();
            ILMonoContainer container;
            if (m_IMOnoDic.TryGetValue(instanceId, out container))
                return container.Get<T>();
            return null;
        }
    }



    public static class ILMgrExtension
    {
        /// <summary>
        /// 注册要添加的IMono组件 usemsg表示是否使用消息机制进行事件触发
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <param name="imonoObj"></param>
        public static void RegisterIMono<T>(this GameObject go, T imonoObj) where T : class, IMono
        {
            if (!ReferenceEquals(go, null))
            {
                if (!Application.isEditor&&typeof(T) is IDrawGizmos)
                    return;
                ILMonoMgr.Instance.RegisterIMono<T>(go, imonoObj);

            }

        }

        /// <summary>
        /// 注册要添加的IMono组件 usemsg表示是否使用消息机制进行事件触发
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <param name="imonoObj"></param>
        public static void UnRegisterIMono<T>(this GameObject go, T imonoObj) where T : class, IMono
        {
            if (!ReferenceEquals(go, null))
                ILMonoMgr.Instance.UnRegisterIMono<T>(go, imonoObj);
        }

        /// <summary>
        /// 注册要添加的IMono组件 usemsg表示是否使用消息机制进行事件触发
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        public static void UnRegister<T>(this GameObject go) where T : class, IMono
        {
            ILMonoMgr.Instance.UnRegister<T>(go);
        }

        /// <summary>
        /// 注册要添加的组件 不执行Mono信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <param name="imonoObj"></param>
        public static void Register<T>(this GameObject go, T imonoObj) where T : class
        {
            ILMonoMgr.Instance.Register<T>(go, imonoObj);
        }

        /// <summary>
        /// 所有注册结束开始添加组件
        /// </summary>
        /// <param name="go"></param>
        public static void RegisterOver(this GameObject go)
        {
            ILMonoMgr.Instance.RegisterOver(go);
        }

        /// <summary>
        /// 获取指定类型的对象实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <returns></returns>
        public static T GetILInstance<T>(this GameObject go) where T : class
        {
            return ILMonoMgr.Instance.GetInstance<T>(go);
        }

        /// <summary>
        /// 获取指定类型的对象实例 若没有 则创建一个 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <returns></returns>
        public static T GetILInstanceOrNew<T>(this GameObject go) where T : BaseHotMono
        {
            if (ReferenceEquals(go, null))
            {
                MyDebuger.LogError($"GetILInstanceOrNew {typeof(T)} is null");
                return null;
            }
            T t = ILMonoMgr.Instance.GetInstance<T>(go);
            if (t == null)
            {
                Type type = typeof(T);
                object[] args = new object[1];
                args[0] = go;
                t = (T)Activator.CreateInstance(type, args);
                if (!ILMonoMgr.Instance.ContainsInstance<T>(go))
                {
                    MyDebuger.LogWarning(go.name + " not contain compentent " + type);
                    ILMonoMgr.Instance.Register(go, t);
                }
            }
            return t;
        }



        /// <summary>
        /// 是否包含某组件
        /// </summary>
        /// <param name="go"></param>
        /// <param name="canuse"></param>
        public static bool ContainsInstance<T>(this GameObject go)
        {
            return ILMonoMgr.Instance.ContainsInstance<T>(go);
        }


        /// <summary>
        /// 设置是否使用Mono组件
        /// </summary>
        /// <param name="go"></param>
        /// <param name="canuse"></param>
        public static void SetCanUseILMono(this GameObject go, bool canuse)
        {
            ILMonoMgr.Instance.SetCanUseMonoEvt(go, canuse);
        }

    }

}
