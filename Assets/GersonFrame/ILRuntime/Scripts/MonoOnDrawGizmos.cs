

using UnityEngine;

namespace GersonFrame.SelfILRuntime
{
    [DisallowMultipleComponent]
    public class MonoOnDrawGizmos : AbstractMonoNormalInvoker
    {

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
             this.Invoke();
        }
#endif

    }

}
