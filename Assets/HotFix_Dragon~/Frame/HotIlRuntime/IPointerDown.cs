

using UnityEngine.EventSystems;

namespace HotGersonFrame.HotIlRuntime
{
   public interface IPointerDown:IMono
    {
        void OnPointerDown(PointerEventData eventData);
    }
}
