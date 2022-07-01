

using ILRuntime.Runtime;
using UnityEngine;

namespace GersonFrame.SelfILRuntime
{

    [ILRuntimeJIT(ILRuntimeJITFlags.JITImmediately)]
    [DisallowMultipleComponent]
    public class MonoUpdate : AbstractMonoNormalInvoker
    {
        private void Update()
        {
                this.Invoke();
        }

    

    }
}
