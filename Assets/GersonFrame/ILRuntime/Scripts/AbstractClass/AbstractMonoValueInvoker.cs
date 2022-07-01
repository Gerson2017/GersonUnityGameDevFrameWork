
using System;
using UnityEngine;

namespace GersonFrame.SelfILRuntime
{

    public abstract class AbstractMonoValueInvoker : MonoBehaviour
    {

        private event Action<string, float, int> m_strCallBack;
        protected bool m_useMsg = true;
        public void AddCallBack(Action<string,float,int> callback)
        {
            m_useMsg = false;
            this.m_strCallBack += callback;
        }

        public void RemoveCallback(Action<string, float, int> callback)
        {
            this.m_strCallBack -= callback;
        }


        public void ClearCallBack()
        {
            m_useMsg = true;
            this.m_strCallBack = null;
        }

       protected void Invoke(string str, float f, int n)
        {
            this.m_strCallBack?.Invoke(str,f,n);
        }

        private void OnDestroy()
        {
            ClearCallBack();
        }
    }

}
