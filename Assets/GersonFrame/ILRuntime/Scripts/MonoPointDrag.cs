
using UnityEngine;
using UnityEngine.EventSystems;

namespace GersonFrame.SelfILRuntime
{
    [DisallowMultipleComponent]
    public class MonoPointDrag : AbstractMonoPointerInvoker, IDragHandler
    {
        public void OnDrag(PointerEventData eventData)
        {
            this.Invoke(eventData);
        }


    }
}
