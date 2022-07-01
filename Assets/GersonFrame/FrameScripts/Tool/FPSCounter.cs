using UnityEngine;
using System.Collections;
using GersonFrame.ABFrame;

namespace GersonFrame.Tool
{

    public class FPSCounter : MonoBehaviour
    {
        [Range(30, 120)]
        public int m_TagetFrameRate = 30;


        private float currentTime = 0;
        private float lateTime = 0;

        private float framesNum = 0;
        private float fpsTime = 0;

        GUIStyle style = new GUIStyle();


        private bool m_showFps = false;
        

        private void Start()
        {
            style.fontSize = 50;
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = m_TagetFrameRate;

            if (Application.isEditor)
                m_showFps = true;
          
        }





        // Update is called once per frame
        void Update()
        {
            if (Input.touchCount==6)
                m_showFps = !m_showFps;
            if (!m_showFps) return;
            currentTime += Time.deltaTime;

            framesNum++;

            if (currentTime - lateTime >= 1.0f)
            {
                fpsTime = framesNum / (currentTime - lateTime);

                lateTime = currentTime;

                framesNum = 0;
            }
        }




        void OnGUI()
        {
            if (!m_showFps) return;
            GUI.Label(new Rect(0, 0, 300, 300), "fps:" + fpsTime.ToString(), style);
        }


    }
}
