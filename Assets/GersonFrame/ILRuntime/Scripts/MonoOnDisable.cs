

using UnityEngine;

namespace GersonFrame.SelfILRuntime
{
    [DisallowMultipleComponent]
    public class MonoOnDisable : AbstractMonoNormalInvoker
    {
        private void OnDisable()
        {
           this.Invoke();
        }
    }

}
