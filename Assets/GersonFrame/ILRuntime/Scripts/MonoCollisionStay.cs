

using UnityEngine;

namespace GersonFrame.SelfILRuntime
{

    [DisallowMultipleComponent]
    public class MonoCollisionStay: AbstractCollisionInvoker
    {
        private void OnCollisionStay(Collision other)
        {
                this .Invoke(other);
        }

    }
}
