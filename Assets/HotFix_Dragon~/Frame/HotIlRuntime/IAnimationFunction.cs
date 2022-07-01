
using UnityEngine;

namespace HotGersonFrame.HotIlRuntime
{
    public interface IAnimationFunction:IMono
    {
        void OnAnimaitonEvtInvoke(string stringarg, float floatarg, int intarg);
    }
}
