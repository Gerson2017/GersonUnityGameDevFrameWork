using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GersonFrame.SelfILRuntime
{
    [DisallowMultipleComponent]
    public class MonoTriggerEnter : AbstractTriggerInvoker
    {
        private void OnTriggerEnter(Collider other)
        {
             this.Invoke(other);
           
        }




    }
}
