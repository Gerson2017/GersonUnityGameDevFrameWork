
using UnityEngine.EventSystems;

namespace HotGersonFrame.HotIlRuntime
{
   public interface IPointerDrag:IMono
    {
        void OnDrag(PointerEventData eventData);

    }
}
