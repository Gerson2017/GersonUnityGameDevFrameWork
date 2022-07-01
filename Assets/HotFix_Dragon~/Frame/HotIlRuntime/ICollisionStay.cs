
using UnityEngine;

namespace HotGersonFrame.HotIlRuntime
{
    public interface ICollisionStay : IMono
    {
        void OnCollisionStay(Collision other);

    }
}
