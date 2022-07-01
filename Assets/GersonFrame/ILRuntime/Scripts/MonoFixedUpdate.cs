



using ILRuntime.Runtime;
using UnityEngine;

namespace GersonFrame.SelfILRuntime
{

    [DisallowMultipleComponent]
    [ILRuntimeJIT(ILRuntimeJITFlags.JITImmediately)]
    public class MonoFixedUpdate : AbstractMonoNormalInvoker
    {

        private void FixedUpdate()
        {
             this.Invoke();
        }



    }
}
