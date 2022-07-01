
using UnityEngine;

namespace GersonFrame.SelfILRuntime
{
    [DisallowMultipleComponent]
    public class MonoAnimationFunction : AbstractMonoValueInvoker
    {


        public void AmClipFunctuion(string stringarg)
        {
            this.Invoke(stringarg, 0f, 0);
        }


        public void AmEvtFunction(string stringarg)
        {

                this.Invoke(stringarg,0f,0);
        }


        public void AmEvtFunction(float floatarg)
        {
          this.Invoke(null, floatarg, 0);
        }


        public void AmEvtFunction(int intarg)
        {
              this.Invoke(null, 0f, intarg);
        }


    }
}

