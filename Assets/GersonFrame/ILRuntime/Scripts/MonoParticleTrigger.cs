
using UnityEngine;

namespace GersonFrame.SelfILRuntime
{
    [DisallowMultipleComponent]
    public class MonoParticleTrigger : AbstractMonoNormalInvoker
    {
        private void OnParticleTrigger()
        {
           this.Invoke();
        }
    }
}
