

using UnityEngine;

namespace HotGersonFrame.HotIlRuntime
{
    public interface ITriggerEnter : IMono
    {
        void OnTriggerEnter(Collider other);

    }
}
