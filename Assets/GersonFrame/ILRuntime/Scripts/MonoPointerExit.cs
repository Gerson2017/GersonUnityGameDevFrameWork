
using ILRuntime.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GersonFrame.SelfILRuntime
{
    [DisallowMultipleComponent]

    public class MonoPointerExit : AbstractMonoPointerInvoker, IPointerExitHandler
    {
        public void OnPointerExit(PointerEventData eventData)
        {
            this.Invoke(eventData);
        }
    }
}
