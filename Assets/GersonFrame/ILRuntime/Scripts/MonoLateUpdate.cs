

using ILRuntime.Runtime;
using UnityEngine;

namespace GersonFrame.SelfILRuntime
{

    [DisallowMultipleComponent]
    [ILRuntimeJIT(ILRuntimeJITFlags.JITImmediately)]
    public class MonoLateUpdate : AbstractMonoNormalInvoker
    {

        private void LateUpdate()
        {
           this.Invoke();
        }



    }
}
