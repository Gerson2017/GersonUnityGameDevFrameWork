

using UnityEngine;

namespace GersonFrame.SelfILRuntime
{

    [DisallowMultipleComponent]
    public class MonoTriggerExit: AbstractTriggerInvoker
    {
        private void OnTriggerExit(Collider other)
        {
             this.Invoke(other);
        }

    }
}
