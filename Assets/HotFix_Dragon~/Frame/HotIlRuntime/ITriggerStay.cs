
using UnityEngine;

namespace HotGersonFrame.HotIlRuntime
{
    public interface ITriggerStay : IMono
    {
        void OnTriggerStay(Collider other);
    }
}
