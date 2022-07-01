

using UnityEngine;

namespace HotGersonFrame.HotIlRuntime
{
    public interface ICollisionExit : IMono
    {
        void OnCollisionExit(Collision other);
    }
}
