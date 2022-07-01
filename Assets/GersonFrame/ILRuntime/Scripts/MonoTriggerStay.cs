

using UnityEngine;

namespace GersonFrame.SelfILRuntime
{

    [DisallowMultipleComponent]
    public class MonoTriggerStay: AbstractTriggerInvoker
    {
        private void OnTriggerStay(Collider other)
        {
              this.Invoke(other);
        }

    }
}
