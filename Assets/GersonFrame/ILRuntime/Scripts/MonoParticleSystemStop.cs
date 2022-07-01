using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GersonFrame.SelfILRuntime
{
    [DisallowMultipleComponent]
    public class MonoParticleSystemStop : AbstractMonoNormalInvoker
    {
        private void OnParticleSystemStopped()
        {
            
                this.Invoke();
        }
    }
}
