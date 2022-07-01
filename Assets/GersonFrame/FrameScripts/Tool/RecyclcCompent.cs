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

        private BaseInternalMsg m_innerMsg = new BaseInternalMsg();

        private void OnEnable()
        {
            if (m_waitforsecond==null)
                m_waitforsecond= new WaitForSeconds(m_RecycleTime);
            m_innerMsg.RegisterMsg("TestRegister",this.TestRegister);
            m_innerMsg.RegisterMsg("TestRegister", this.TestRegister);
            this.StartCoroutine(Disable());
        }

        private void TestRegister(object arg1, object arg2, object arg3)
        {
            MyDebuger.Log("TestRegister");
        }

        IEnumerator  Disable()
        {
            yield return m_waitforsecond;
            if (gameObject.activeInHierarchy)
                ObjectManager.Instance.ReleaseObject(this.gameObject);

        }


    }
}
