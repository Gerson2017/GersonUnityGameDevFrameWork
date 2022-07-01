

using UnityEngine;

namespace HotGersonFrame.HotIlRuntime
{
    public interface ITriggerExit : IMono
    {
        void OnTriggerExit(Collider other);
    }
}
