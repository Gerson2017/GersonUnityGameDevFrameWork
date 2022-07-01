using UnityEngine;


namespace GersonFrame.Tool
{
    public class ShakeEffect
    {
        private Transform m_shakeTs;
        private Vector3 m_originalPos = Vector3.zero;
        private Vector3 m_startshakePos = Vector3.zero;


        /// <summary>
        /// 是否在震动
        /// </summary>
        public bool IsShaking { get; private set; }

        /// <summary>
        /// 是否在运行
        /// </summary>
        public bool Running { get; private set; } = true;

        public ShakeEffect(Transform shakets)
        {
            this.m_shakeTs = shakets;
        }


        /// <summary>
        /// 设置抖动参数 factor设置为1时 会一直持续震动
        /// </summary>
        public void StartShake(float factor = -1, float value = -1)
        {
            if (this.IsShaking)
                return;
            this.IsShaking = true;
            this.m_startshakePos = m_shakeTs.position;
            if (factor != -1)
                this.m_shakefactor = factor;
            else
                this.m_shakefactor = 1.5f;
            if (value != -1)
                this.m_shakevalue = value;
            else
                this.m_shakevalue = 0.12f;
        }


        public void StopShake()
        {
            m_shakevalue = 0;
            Running = true;
        }


        public void PauseShake()
        {
            Running = false;
        }

        public void ResumeShake()
        {
            Running = true;
        }


        public void onUpdate()
        {
            if (IsShaking&& Running)
                this.CameraShake();
        }




        //===================================Shake============================================
        private float m_shakevalue = 1;
        private float m_shakefactor = 1.1f;
        void CameraShake()
        {
            this.m_originalPos.x = Random.Range(0, this.m_shakevalue * 2) - this.m_shakevalue;
            this.m_originalPos.y = Random.Range(0, this.m_shakevalue * 2) - this.m_shakevalue;
            this.m_originalPos.z = Random.Range(0, this.m_shakevalue * 2) - this.m_shakevalue;
            this.m_shakevalue = this.m_shakevalue / this.m_shakefactor;
            if (this.m_shakevalue < 0.01f)
            {
                this.IsShaking = false;
                this.m_shakevalue = 0;
                this.m_originalPos.x = 0;
                this.m_originalPos.y = 0;
            }
            else
            {
                this.m_shakeTs.position = this.m_startshakePos + this.m_originalPos;
            }
        }



    }
}