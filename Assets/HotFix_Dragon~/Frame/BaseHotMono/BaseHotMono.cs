using GersonFrame;
using UnityEngine;


namespace HotGersonFrame.HotIlRuntime
{
    public  abstract class BaseHotMono:BaseInternalMsg,IOnDestroy
    {
        public GameObject gameObject { get; protected set; }
        public Transform transform { get; protected set; }
        private int m_instanceId;

         public  BaseHotMono(GameObject go)
        {
            m_instanceId = go.GetInstanceID();
            this.gameObject = go;
            this.transform = go.transform;
            go.RegisterIMono<IOnDestroy>(this);
        }

       public virtual void OnDestroy()
        {
            this.UnRegisterAll();
            ILMonoMgr.Instance.UnRegisterAll(gameObject);
            this.gameObject = null;
            this.transform = null;
        }

        protected abstract void RegisterMonoEvt(GameObject go);

    }

    public static class BaseHotMonoExtension
    {
        public static T GetComponent<T>(this BaseHotMono hotMono) 
        {
            return hotMono.gameObject.GetComponent<T>();
        }

        public static T AddComponent<T>(this BaseHotMono hotMono) where T:UnityEngine.Component
        {
            return hotMono.gameObject.AddComponent<T>();
        }

    }

}
