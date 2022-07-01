

using UnityEngine;

namespace GersonFrame.SelfILRuntime
{

    [DisallowMultipleComponent]
    public class MonoCollisionExit : AbstractCollisionInvoker
    {
        private void OnCollisionExit(Collision other)
        {
                this .Invoke(other);
        }

    }
}
