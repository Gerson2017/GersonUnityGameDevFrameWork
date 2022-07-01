

using UnityEngine.EventSystems;

namespace HotGersonFrame.HotIlRuntime
{
    public interface IPointerUp : IMono
    {
        void OnPointerUp(PointerEventData eventData);
    }
}
