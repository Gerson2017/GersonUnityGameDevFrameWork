

using UnityEngine;

namespace GersonFrame.SelfILRuntime
{

    [DisallowMultipleComponent]
    public class MonoOnDestroy : AbstractMonoNormalInvoker
    {

        private void OnDestroy()
        {
              this.Invoke();
        }




    }
}
