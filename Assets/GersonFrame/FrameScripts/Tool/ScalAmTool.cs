using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GersonFrame.Tool
{
    public class ScalAmTool : MonoBehaviour
    {
        private float m_timer = 0;
        public bool m_AutoScal = true;
        public bool m_ScalBack = false;
        public float m_moveBackInternal = 1f;
        public float m_ScalTime = 0.5f;
        public float m_StartScal = 0;
        public float m_EndScal = 1;

        private Vector3 m_startScal = Vector3.zero;
        private Vector3 m_endScal = Vector3.zero;

        // Start is called before the first frame update
        void Start()
        {
            this.m_startScal = new Vector3(m_StartScal, m_StartScal, m_StartScal);
            this.m_endScal = new Vector3(m_EndScal, m_EndScal, m_EndScal);
            this.transform.localScale = this.m_startScal;
            this.m_timer = 0;
        }

        public void StartScal()
        {
            this.gameObject.Show();
            this.transform.localScale = this.m_startScal;
            this.m_AutoScal = true;
            m_timer = 0;
        }




        /// <summary>
        /// 重置大小
        /// </summary>
        public void ResetScal()
        {
            this.transform.localScale = this.m_startScal;
        }


        // Update is called once per frame
        void Update()
        {
            if (!this.m_AutoScal) return;
            if (this.m_timer < 1)
            {
                this.m_timer += Time.deltaTime / m_ScalTime;
                this.transform.localScale = Vector3.Lerp(this.m_startScal, this.m_endScal, this.m_timer);
            }
            else if (m_ScalBack)
            {
                if (this.m_moveBackInternal > 0)
                {
                    this.m_moveBackInternal -= Time.deltaTime;
                }
                else
                {
                    this.m_timer += Time.deltaTime / m_ScalTime;
                    this.transform.localScale = Vector3.Lerp(this.m_endScal, this.m_startScal, this.m_timer - 1);
                }

            }

        }
    }
}
