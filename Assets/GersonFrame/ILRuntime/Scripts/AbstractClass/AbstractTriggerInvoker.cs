
using System;
using UnityEngine;

namespace GersonFrame.SelfILRuntime
{

    public abstract class AbstractTriggerInvoker : MonoBehaviour
    {

        private event Action<Collider> m_triggerCallBack;

        protected bool m_useMsg = true;
        public void AddCallBack(Action<Collider> callback)
        {
            m_useMsg = false;
            this.m_triggerCallBack += callback;
        }


        public void RemoveCallback(Action<Collider> callback)
        {
            this.m_triggerCallBack -= callback;
        }
       

        public void ClearCallBack()
        {
            m_useMsg = true;
            this.m_triggerCallBack = null;
        }


       protected void Invoke(Collider collider)
        {
            this.m_triggerCallBack?.Invoke(collider);
        }

        private void OnDestroy()
        {
            ClearCallBack();
        }
    }

}
