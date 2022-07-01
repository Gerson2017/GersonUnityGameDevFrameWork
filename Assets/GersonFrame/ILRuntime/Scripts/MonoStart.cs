

using UnityEngine;

namespace GersonFrame.SelfILRuntime
{

    [DisallowMultipleComponent]
    public class MonoStart : AbstractMonoNormalInvoker
    {

        private void Start()
        {
             this.Invoke();
        }



    }
}
