using UnityEngine;
using DG.Tweening;
using GersonFrame.ABFrame;

namespace GersonFrame.UI
{

    public enum ViewState
    {
        Showing,
        Hide,
        Destory
    }

    public abstract  class BaseHotView:BaseInternalMsg
    {
        protected float m_showAmTime = 0.3f;
        public GameObject gameobject { get; private set; }

        private Transform m_transform;
        public Transform transform
        {
            get
            {
                if (m_transform==null)
                    m_transform = gameobject.transform;
                return m_transform;
            }
        }
        public bool IsHotView { get; private set; }

        public string ViewName { get; private set; }

        private Transform m_aMts = null;

        /// <summary>
        /// 界面是否是显示中
        /// </summary>
        public ViewState mViewIState { get; protected set; }



        /// <summary>
        ///更换动画播放物体
        /// </summary>
        /// <param name="transform"></param>
        public void ChangeAmTs(Transform transform)
        {
            m_aMts = transform;
        }


        internal void SetViewInfo(GameObject viewgo,string viewName, bool ishotview=false)
        {
            ViewName = viewName;
            this.gameobject = viewgo;
            this.IsHotView = ishotview;
            InitView();
        }

        protected abstract void InitView();

        public virtual void OnEnter(object param = null, object param2 = null, object param3 = null)
        {
            this.mViewIState =  ViewState.Showing;
            this.gameobject.SetActive(true);
            this.ShowScalAm();
        }

        public virtual void OnPause(){ }
        public virtual void OnResume(){ }

        public virtual void OnExit()
        {
            if (this.mViewIState == ViewState.Destory) return;
            this.mViewIState = ViewState.Hide;
            this.HideScalAm();
        }

        public virtual void OnDestroy() 
        {
            this.mViewIState = ViewState.Destory;
            gameobject.transform.DOKill();
            Transform root = gameobject.transform.Find("root");
            if (root!=null)
                root.DOKill();

            this.UnRegisterAll();
            this.UnRegisterMsgListener();
            this.UnRegisterNetMsg();
            ObjectManager.Instance.ReleaseObject(gameobject, 0);
        }

        ///=============================UIListener==================================

        /// <summary>
        /// 注册UI事件监听
        /// </summary>
        protected abstract void AddUIListener();


        //=======================EVT=========================================

        /// <summary>
        /// Update 每帧调用
        /// </summary>
        public virtual void Update()
        {

        }

        /// <summary>
        /// 注册Msg消息监听
        /// </summary>
        protected abstract void RegisterMsgListener();

        /// <summary>
        /// 取消Msg消息监听
        /// </summary>
        protected abstract void UnRegisterMsgListener();


        /// <summary>
        /// 注册网络事件监听
        /// </summary>
        protected abstract void RegisterNetMsg();


        /// <summary>
        /// 取消网络事件注册时间监听
        /// </summary>
        protected abstract void UnRegisterNetMsg();



        //=============================AM========================================

        public virtual void ShowScalAm()
        {
            if (m_aMts == null)
                m_aMts = gameobject.transform;
            UIManager.PanelInAnim(0.5f, m_aMts, ShowEnd);
        }

        public virtual void HideScalAm()
        {
            if (m_aMts == null) 
                m_aMts = gameobject.transform;
            UIManager.PanelOutAnim(0.3f, m_aMts, HideEnd);
        }

        protected virtual void ShowEnd(){ }

        protected virtual void HideEnd()
        {
            this.gameobject.Hide();
            if (m_aMts == null)
                m_aMts = gameobject.transform;
            m_aMts.localScale = Vector3.one;
        }

    }
}



