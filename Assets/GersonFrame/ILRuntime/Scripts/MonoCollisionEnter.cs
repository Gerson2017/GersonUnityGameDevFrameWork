

using UnityEngine;

namespace GersonFrame.SelfILRuntime
{

    [DisallowMultipleComponent]
    public class MonoCollisionEnter:AbstractCollisionInvoker
    {
        private void OnCollisionEnter(Collision other)
        {
            this.Invoke(other);
        }

    }
}
