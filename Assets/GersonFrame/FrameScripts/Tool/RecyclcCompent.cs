using GersonFrame.ABFrame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GersonFrame.Tool
{
    public class RecyclcCompent : MonoBehaviour
    {

        public float m_RecycleTime = 0.7f;
        WaitForSeconds m_waitforsecond;


        private void OnEnable()
        {
            if (m_waitforsecond==null)
                m_waitforsecond= new WaitForSeconds(m_RecycleTime);
            this.StartCoroutine(Disable());
        }


        IEnumerator  Disable()
        {
            yield return m_waitforsecond;
            if (gameObject.activeInHierarchy)
                ObjectManager.Instance.ReleaseObject(this.gameObject);

        }


    }
}
