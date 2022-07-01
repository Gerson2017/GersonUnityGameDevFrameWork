
using System;
using UnityEngine;

namespace GersonFrame.SelfILRuntime
{

    public abstract class AbstractCollisionInvoker : MonoBehaviour
    {

        private event Action<Collision> m_collisionCallBack;


        public void AddCallBack(Action<Collision> callback)
        {
            this.m_collisionCallBack += callback;
        }


        public void RemoveCallback(Action<Collision> callback)
        {
            this.m_collisionCallBack -= callback;
        }
       

        public void ClearCallBack()
        {
            this.m_collisionCallBack = null;
        }


       protected void Invoke(Collision other)
        {
            this.m_collisionCallBack?.Invoke(other);
        }

        private void OnDestroy()
        {
            ClearCallBack();
        }
    }

}
