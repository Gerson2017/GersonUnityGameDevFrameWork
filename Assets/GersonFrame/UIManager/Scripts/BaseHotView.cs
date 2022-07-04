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



    public abstract  class BaseHotView
    {
        protected float m_showAmTime = 0.3f;
        public GameObject gameobject { get; private set; }

        public Transform transform => gameobject.transform;


        public string ViewName { get; private set; }

        private Transform animationTs = null;

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
            animationTs = transform;
        }


        internal void SetViewInfo(GameObject viewgo,string viewName)
        {
            ViewName = viewName;
            this.gameobject = viewgo;
            InitView();
        }

        protected abstract void InitView();

        public virtual void OnEnter(object param = null, object param2 = null, object param3 = null)
        {
            this.mViewIState =  ViewState.Showing;
            this.gameobject.SetActive(true);
            this.ShowScalAm();
        }

        public virtual void OnExit()
        {
            if (this.mViewIState == ViewState.Destory) return;
            this.mViewIState = ViewState.Hide;
            this.HideScalAm();
        }

        public virtual void OnDestroy() 
        {
            this.mViewIState = ViewState.Destory;
            transform.DOKill();
            if (animationTs != null&&animationTs!=transform)
                animationTs.DOKill();

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
            if (animationTs == null)
                animationTs = gameobject.transform;
            UIManager.PanelInAnim(0.5f, animationTs, ShowEnd);
        }

        public virtual void HideScalAm()
        {
            if (animationTs == null) 
                animationTs = gameobject.transform;
            UIManager.PanelOutAnim(0.3f, animationTs, HideEnd);
        }

        protected virtual void ShowEnd(){ }

        protected virtual void HideEnd()
        {
            this.gameobject.Hide();
            if (animationTs == null)
                animationTs = gameobject.transform;
            animationTs.localScale = Vector3.one;
        }

    }
}



