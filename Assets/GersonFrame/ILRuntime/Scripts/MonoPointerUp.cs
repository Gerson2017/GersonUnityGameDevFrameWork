
using ILRuntime.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GersonFrame.SelfILRuntime
{
    [DisallowMultipleComponent]
    [ILRuntimeJIT(ILRuntimeJITFlags.JITImmediately)]
    public class MonoPointerUp : AbstractMonoPointerInvoker, IPointerUpHandler
    {
        [ILRuntimeJIT(ILRuntimeJITFlags.NoJIT)]
        public void OnPointerUp(PointerEventData eventData)
        {
              this.Invoke(eventData);

        }
    }
}
