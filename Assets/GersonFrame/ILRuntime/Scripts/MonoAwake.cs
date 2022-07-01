

using UnityEngine;

namespace GersonFrame.SelfILRuntime
{

    [DisallowMultipleComponent]
    public class MonoAwake : AbstractMonoNormalInvoker
    {


        private void Awake()
        {
            this.Invoke();
        }

    }
}
