

using UnityEngine;

namespace GersonFrame.SelfILRuntime
{


    [DisallowMultipleComponent]
    public class MonoEnable : AbstractMonoNormalInvoker
    {

        private void OnEnable()
        {
             this.Invoke();
        }



    }
}
