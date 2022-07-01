
using UnityEngine;


namespace GersonFrame.Tool
{


    public class MouseTrack : MonoBehaviour
    {

        /// <summary>

        /// 获取LineRenderer组件

        /// </summary>

        [Header("获得LineRenderer组件")]

        public LineRenderer lineRenderer;

        private Vector3[] mouseTrackPositions = new Vector3[10];

        private Vector3 headPosition;

        private Vector3 lastPosition;

        private int positionCount = 0;

        [Header("设置多远举例记录一个位置")]

        public float distanceOfPositions = 0.01f;

        private bool firstMouseDown = false;

        private bool mouseDown = false;

        // Use this for initialization

        void Start()
        {

        }

        // Update is called once per frame

        void Update()
        {

            if (Input.GetMouseButtonDown(0))
            {

                firstMouseDown = true;

                mouseDown = true;

            }

            if (Input.GetMouseButtonUp(0))
            {

                mouseDown = false;

                MyDebuger.Log("mouseDown:" + mouseDown.ToString());

            }

            OnDrawLine();

            firstMouseDown = false;

        }

        private void OnDrawLine()
        {

            if (firstMouseDown == true)
            {

                positionCount = 0;

                headPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 10));

                lastPosition = headPosition;

            }

            if (mouseDown == true)
            {

                headPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 10));

                if (Vector3.Distance(headPosition, lastPosition) > distanceOfPositions)
                {

                    SavePosition(headPosition);

                    positionCount++;

                }

                lastPosition = headPosition;

            }
            else
            {

                mouseTrackPositions = new Vector3[10];

            }

            SetLineRendererPosition(mouseTrackPositions);

        }

        private void SavePosition(Vector3 pos)
        {

            pos.z = 0;

            if (positionCount <= 9)
            {

                for (int i = positionCount; i < 10; i++)
                {

                    mouseTrackPositions[i] = pos;

                }

            }
            else
            {

                for (int i = 0; i < 9; i++)
                {

                    mouseTrackPositions[i] = mouseTrackPositions[i + 1];

                }

                mouseTrackPositions[9] = pos;

            }

        }

        private void SetLineRendererPosition(Vector3[] positions)
        {

            lineRenderer.SetPositions(positions);

        }

    }
}