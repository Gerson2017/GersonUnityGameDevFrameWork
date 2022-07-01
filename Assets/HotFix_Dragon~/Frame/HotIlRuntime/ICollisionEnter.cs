

using UnityEngine;

namespace HotGersonFrame.HotIlRuntime
{
    public interface ICollisionEnter : IMono
    {
        void OnCollisionEnter(Collision other);
    }
}
