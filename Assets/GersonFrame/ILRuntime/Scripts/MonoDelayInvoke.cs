using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GersonFrame.SelfILRuntime
{
    [DisallowMultipleComponent]
    public class MonoDelayInvoke : MonoBehaviour
    {
        #region Delay
        public void Delay(float seconds, Action onFinished)
        {
            if (gameObject.activeInHierarchy)
                StartCoroutine(DelayCoroutine(seconds, onFinished));
            else
                MyDebuger.LogWarning("Delay startfail " + this.gameObject.name);
        }



        public void Delay(float seconds, Action<object> onFinishe, object param = null)
        {
            if (gameObject.activeInHierarchy)
                StartCoroutine(DelayCoroutine(seconds, onFinishe, param));
            else
                MyDebuger.LogWarning("Delay startfail " + this.gameObject.name);
        }


        public void Delay(float seconds, Action<object, object, object> onFinished, object param = null, object param1 = null, object param2 = null)
        {
            if (gameObject.activeInHierarchy)
                StartCoroutine(DelayCoroutine(seconds, onFinished, param, param1, param2));
            else
                MyDebuger.LogWarning("Delay startfail " + this.gameObject.name);
        }

        public void DelayFrame(int framecount, Action onFinished)
        {
            if (gameObject.activeInHierarchy)
                StartCoroutine(DelayFrameCorotinue(framecount, onFinished));
            else
                MyDebuger.LogWarning("DelayFrame startfail " + this.gameObject.name);
        }

        public void DelayFrame(int framecount, Action<object> onFinished, object param = null)
        {
            if (gameObject.activeInHierarchy)
                StartCoroutine(DelayFrameCorotinue(framecount, onFinished, param));
            else
                MyDebuger.LogWarning("DelayFrame startfail " + this.gameObject.name);
        }


        public void DelayFrame(int framecount, Action<object, object, object> onFinished, object param = null, object param1 = null, object param2 = null)
        {
            if (gameObject.activeInHierarchy)
                StartCoroutine(DelayFrameCorotinue(framecount, onFinished, param, param1, param2));
            else
                MyDebuger.LogWarning("DelayFrame startfail " + this.gameObject.name);
        }

        private IEnumerator DelayCoroutine(float seconds, Action onFinished)
        {
            yield return new WaitForSeconds(seconds);
            onFinished();
        }

        private IEnumerator DelayCoroutine(float seconds, Action<object> onFinished, object param = null)
        {
            yield return new WaitForSeconds(seconds);
            onFinished(param);
        }

        private IEnumerator DelayCoroutine(float seconds, Action<object, object, object> onFinished, object param = null, object param1 = null, object param2 = null)
        {
            yield return new WaitForSeconds(seconds);
            onFinished(param, param1, param2);
        }

        #endregion



        private IEnumerator DelayFrameCorotinue(int framecount, Action<object> onFinished, object param = null)
        {
            for (int i = 0; i < framecount; i++)
            {
                yield return null;
            }
            onFinished(param);
        }
        private IEnumerator DelayFrameCorotinue(int framecount, Action onFinished)
        {
            for (int i = 0; i < framecount; i++)
            {
                yield return null;
            }
            onFinished();
        }
        private IEnumerator DelayFrameCorotinue(int framecount, Action<object, object, object> onFinished, object param = null, object param1 = null, object param2 = null)
        {
            for (int i = 0; i < framecount; i++)
            {
                yield return null;
            }
            onFinished(param, param1, param2);
        }



        private void OnDisable()
        {
            this.StopAllCoroutines();
        }
    }

}