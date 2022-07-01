using UnityEngine;
using System.Collections.Generic;
using System;
using GersonFrame.ABFrame;
using GersonFrame.Tool;
using DG.Tweening;
using System.Linq;
using GersonFrame.SelfILRuntime;

namespace GersonFrame.UI
{
    public  class UIManager : MonoSingleton<UIManager>
    {
        private Camera _UICamera;
        public Camera UICamera
        {
            get
            {
                if (_UICamera == null)
                {
                    _UICamera = GameObject.Find("Cameras/CameraUI").GetComponent<Camera>();
                }
                return _UICamera;
            }
        }

        public Camera MainCamera
        {
            get
            {
                if (_UICamera == null)
                {
                    _UICamera = GameObject.Find("Cameras/Main Camera").GetComponent<Camera>();
                }
                return _UICamera;
            }
        }


        private UIManager() { }

        private Transform m_canvasTransform;
        public Transform CanvasTs
        {
            get
            {
                if (m_canvasTransform == null)
                    m_canvasTransform = GameObject.Find("Canvas").transform;

                return m_canvasTransform;
            }
        }

        private RectTransform m_RectcanvasTransform;
        public RectTransform RectCanvasTs
        {
            get
            {
                if (m_RectcanvasTransform == null)
                {
                    m_RectcanvasTransform = CanvasTs.GetComponent<RectTransform>();
                }
                return m_RectcanvasTransform;
            }
        }


        private Dictionary<string, BaseHotView> m_innerViewDic = new Dictionary<string, BaseHotView>();
        private const string m_uiPrefabsPath = "Assets/Prefabs/UI/InnerView/";

        [SerializeField]
        /// <summary>
        /// 需要UpDate的view
        /// </summary>
        private Dictionary<string, BaseHotView> m_updateViewDic = new Dictionary<string, BaseHotView>();
        private List<BaseHotView> m_updayeviewlist = new List<BaseHotView>();

        private MyStringBuilder m_prefabNameBuilder = new MyStringBuilder();

        /// <summary>
        /// 根据面板类型 得到实例化的面板
        /// </summary>
        /// <returns></returns>
        private static BaseHotView GetMyView(string viewName, bool ishotFixView = false)
        {
            BaseHotView panel = GetLoadedVIew(viewName);

            if (panel == null)
            {
                Instance.m_prefabNameBuilder.SetStrs(m_uiPrefabsPath, viewName, ".prefab");
                GameObject instPanel = ObjectManager.Instance.InstantiateObject(Instance.m_prefabNameBuilder.ToString(), false, false);
                if (instPanel == null)
                {
                    MyDebuger.LogError("请检查 路径 " + m_uiPrefabsPath + " 是否有该 " + viewName + " 预制体");
                    return null;
                }
                instPanel.transform.SetParent(Instance.CanvasTs, false);
                instPanel.transform.localScale = Vector3.one;
                instPanel.transform.localPosition = Vector3.zero;

                if (ishotFixView)
                    panel = ILRuntimeManager.Instance.GetNewObjectEx<BaseHotView>(DomainType.UI, viewName);
                else
                {
                    Type objType = Type.GetType(viewName, true);
                    panel = (BaseHotView)Activator.CreateInstance(objType);
                }
                if (panel != null)
                {
                    panel.SetViewInfo(instPanel, viewName, ishotFixView);
                    Instance.m_innerViewDic.Add(viewName, panel);
                }
            }
            return panel;
        }


        /// <summary>
        /// 获取已经加载过的view
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        private static BaseHotView GetLoadedVIew(string viewName)
        {
            if (Instance.m_innerViewDic == null)
                Instance.m_innerViewDic = new Dictionary<string, BaseHotView>();

            BaseHotView panel = null;
            panel = Instance.m_innerViewDic.TryGet(viewName);
            return panel;
        }


        public static BaseHotView ShowView(string viewName, bool ishotview = false, object param = null, object param2 = null, object param3 = null)
        {
            BaseHotView view = GetMyView(viewName, ishotview);
            if (view == null)
            {
                MyDebuger.LogError(" not found innerview " + viewName);
                return null;
            }
            view.OnEnter(param, param2, param3);
            if (!Instance.m_updateViewDic.ContainsKey(viewName))
                Instance.m_updateViewDic[viewName] = view;
            if (!Instance.m_updayeviewlist.Contains(view))
                Instance.m_updayeviewlist.Add(view);
            return view;
        }


        /// <summary>
        /// 显示界面  只能在主域使用
        /// </summary>
        public static BaseHotView ShowView<T>(object param = null, object param2 = null, object param3 = null) where T : BaseHotView
        {
            Type t = typeof(T);
#if UNITY_EDITOR
            if (t.Name.Contains("Adaptor"))
            {
                MyDebuger.LogError("热更界面请在域中请使用 ShowHotView<T> " + t.FullName);
                return null;
            }
#endif

            return ShowView(t.Name, false, param, param2, param3);
        }


        public static BaseHotView GetView(string viewName)
        {
            BaseHotView view = Instance.m_innerViewDic.TryGet(viewName);
            return view;
        }

        public static T GetView<T>(string viewName) where T : BaseHotView
        {
            Type t = typeof(T);
#if UNITY_EDITOR
            if (t.Name.Contains("Adaptor"))
            {
                MyDebuger.LogError("热更界面请在域中请使用 GetHotView<T> " + t.FullName);
                return null;
            }
#endif
            BaseHotView view = GetView(t.Name);
            return view as T;
        }


        /// <summary>
        /// 暂定指定界面
        /// </summary>
        /// <param name="viewName"></param>
        public static void PauseView(string viewName)
        {
            BaseHotView panel = GetLoadedVIew(viewName);
            if (panel == null)
                MyDebuger.LogErrorFormat("{0} 未使用过 无法调用Pause功能", viewName);
            else
                panel.OnPause();
        }

        /// <summary>
        /// 恢复指定界面
        /// </summary>
        /// <param name="viewName"></param>
        public static void ResumeView(string viewName)
        {
            BaseHotView panel = GetLoadedVIew(viewName);
            if (panel == null)
                MyDebuger.LogErrorFormat("{0} 未使用过 无法调用Resume功能", viewName);
            else
                panel.OnResume();
        }

        public static void HideView(string viewName, bool destory = false)
        {
            BaseHotView view = Instance.m_innerViewDic.TryGet(viewName);
            if (view == null)
            {
                MyDebuger.LogWarning("关闭失败 请检查是否是热更域界面 " + viewName);
                return;
            }
            view.OnExit();
            if (destory)
            {
                Instance.m_innerViewDic.Remove(view.ViewName);
                view.OnDestroy();
            }
            if (Instance.m_updateViewDic.ContainsKey(viewName))
                Instance.m_updateViewDic.Remove(viewName);
            if (Instance.m_updayeviewlist.Contains(view))
                Instance.m_updayeviewlist.Remove(view);
        }


        /// <summary>
        /// 隐藏界面  只能在主域使用
        /// </summary>
        public static void HideView<T>(bool destory = false) where T : BaseHotView
        {
            string viewname = ClassTool.Name<T>();
            if (viewname.Contains("Adapter"))
            {
                MyDebuger.LogWarning("热更域请使用字符串隐藏界面");
                return;
            }
            HideView(viewname, destory);
        }


        public static void ResetAllShowingView()
        {
            DOTween.KillAll();
            DOTween.Clear(true);
            List<BaseHotView> views = Instance.m_innerViewDic.Values.ToList();
            for (int i = 0; i < views.Count; i++)
                views[i].OnDestroy();
            Instance.m_updateViewDic.Clear();
            Instance.m_updayeviewlist.Clear();
            Instance.m_innerViewDic.Clear();
        }


        private  void Update()
        {
            for (int i = 0; i < m_updayeviewlist.Count; i++)
                m_updayeviewlist[i].Update();
        }




        public static void PanelInAnim(float time, Transform panel, TweenCallback InFinish = null)
        {
            panel.gameObject.SetActive(true);
            CanvasGroup canvasGroup;
            if (!panel.TryGetComponent(out canvasGroup))
                canvasGroup = panel.gameObject.AddComponent<CanvasGroup>();
            panel.transform.localScale = Vector3.one;
            canvasGroup.alpha = 0.3f;
            canvasGroup.DOFade(1, time);
            panel.DOScale(Vector3.one * 0.5f, time).From().OnComplete(() =>
            {
                InFinish?.Invoke();
            });
        }


        public static void PanelOutAnim(float time, Transform panel, TweenCallback OutFinish)
        {
            CanvasGroup canvasGroup;
            if (!panel.TryGetComponent(out canvasGroup))
                canvasGroup = panel.gameObject.AddComponent<CanvasGroup>();
            canvasGroup.DOFade(0, time);
            panel.DOScale(Vector3.one * 0.5f, time).OnComplete(() =>
            {
                OutFinish?.Invoke();
                panel.transform.localScale = Vector3.one;
                canvasGroup.alpha = 1;
                panel.Hide();
            });
        }


    }
}