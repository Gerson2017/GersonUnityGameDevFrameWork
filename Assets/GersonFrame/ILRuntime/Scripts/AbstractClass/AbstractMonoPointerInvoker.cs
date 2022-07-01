
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GersonFrame.SelfILRuntime
{

    public abstract class AbstractMonoPointerInvoker : MonoBehaviour
    {

        private event Action<PointerEventData> m_updateCallBack;

        protected bool m_useMsg = true;
        public void AddCallBack(Action<PointerEventData> callback)
        {
            m_useMsg = false;
            this.m_updateCallBack += callback;
        }


        public void RemoveCallback(Action<PointerEventData> callback)
        {
            this.m_updateCallBack -= callback;
        }
       

        public void ClearCallBack()
        {
            m_useMsg = true;
            this.m_updateCallBack = null;
        }


       protected void Invoke(PointerEventData eventData)
        {
            this.m_updateCallBack?.Invoke(eventData);
        }
        private void OnDestroy()
        {
            ClearCallBack();
        }

    }

}
