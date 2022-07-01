
using UnityEngine;
using UnityEngine.EventSystems;

namespace GersonFrame.SelfILRuntime
{
    [DisallowMultipleComponent]
    public class MonoPointerDown : AbstractMonoPointerInvoker, IPointerDownHandler
    {

        public void OnPointerDown(PointerEventData eventData)
        {
              this.Invoke(eventData);

        }
    }
}
