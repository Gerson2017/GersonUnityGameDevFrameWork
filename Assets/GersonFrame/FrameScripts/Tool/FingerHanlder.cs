
using UnityEngine;
using UnityEngine.UI;

namespace GersonFrame.Tool
{

    public class FingerHanlder : MonoBehaviour
    {

        public RectTransform m_centerTs;

        //计算摇杆块的半径
        public float mRadius
        {
            get
            {
                if (m_raudio==0)
                    m_raudio= (transform as RectTransform).sizeDelta.x * 0.5f;
                
                return m_raudio;
            }
        }
        private float m_raudio = 0;

        /// <summary>
        /// 无效半径
        /// </summary>
        public float m_unUseRadius = 0;

        private bool m_ismouseDown;
        Canvas m_canvas;
        RectTransform m_cansrecttts;
        RectTransform m_myrectts;
        Vector2 uipos = Vector2.one;


        public System.Action mOnFingerDownAc;
        public System.Action mOnFingerUpAc;

        public System.Action<Vector2> mOnFingerMoveAc;

        protected void Start()
        {

            m_canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
            m_cansrecttts = m_canvas.transform as RectTransform;
            m_myrectts = (transform as RectTransform);
            this.m_ismouseDown = false;
            this.GetComponent<Image>().enabled = false;

        }


        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                this.m_ismouseDown = true;
                var pos = Input.mousePosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(m_cansrecttts, pos, m_canvas.worldCamera, out uipos);
                m_myrectts.localPosition = uipos;
                this.GetComponent<Image>().enabled = true;
                this.m_centerTs.GetComponent<Image>().enabled = true;
                mOnFingerDownAc?.Invoke();
            }

            if (Input.GetMouseButtonUp(0))
            {
                this.m_centerTs.localPosition = Vector3.zero;
                this.m_ismouseDown = false;
                this.GetComponent<Image>().enabled = false;
                this.m_centerTs.GetComponent<Image>().enabled = false;
                mOnFingerUpAc?.Invoke();
            }

            if (this.m_ismouseDown)
            {
                var pos = Input.mousePosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(m_myrectts, pos, m_canvas.worldCamera, out uipos);
                float dis = Vector2.Distance(Vector2.zero, uipos);
                Vector2 dir = uipos.normalized;
                if (dis > mRadius) 
                    dis = mRadius;
                this.m_centerTs.localPosition = dir * dis;
                if (dis>m_unUseRadius)
                    mOnFingerMoveAc?.Invoke(uipos);
                
              
            }
        }



    }
}