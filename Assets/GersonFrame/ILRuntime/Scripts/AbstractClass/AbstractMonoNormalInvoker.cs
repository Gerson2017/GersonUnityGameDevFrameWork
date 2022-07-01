
using UnityEngine;

namespace GersonFrame.SelfILRuntime
{

    public abstract class AbstractMonoNormalInvoker : MonoBehaviour
    {

        protected event System.Action m_updateCallBack;

        protected bool m_useMsg = true;
        public void AddCallBack(System.Action callback)
        {
            m_useMsg = false;
            this.m_updateCallBack += callback;
#if UNITY_EDITOR
            if (this.m_updateCallBack.GetInvocationList().Length > 1)
                MyDebuger.LogWarning(gameObject.name + " invokecount "+ this.m_updateCallBack.GetInvocationList().Length +" tyep="+ this.GetType().FullName+ " "+gameObject.name);
#endif
        }


        public void RemoveCallback(System.Action callback)
        {
            this.m_updateCallBack -= callback;
        }
       

        public void ClearCallBack()
        {
            m_useMsg = true;
            this.m_updateCallBack = null;
        }


       protected void Invoke()
        {
            this.m_updateCallBack?.Invoke();
        }

        private void OnDestroy()
        {
            ClearCallBack();
        }

    }

}
