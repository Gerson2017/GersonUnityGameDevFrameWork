using System.Collections.Generic;
using UnityEngine;

namespace GersonFrame.Tool
{


    public enum BeZierLineType
    {
        Normal,
        PingPang,
        Loop,
        PartLoop,//部分节点循环    
        PartPingPang,//部分节点循环
    }


    public class BeZierLine : MonoBehaviourSimplify
    {

#if UNITY_EDITOR
        public Color m_LineColor = Color.white;
#endif

        /// <summary>
        /// 控制所有曲线移动
        /// </summary>
        public static bool LinePause = false;
        /// <summary>
        /// 曲线移动时间间隔
        /// </summary>
        public static float LineFixedDeltaTime = 0.02f;

        public BeZierLineType m_MoveType = BeZierLineType.Normal;
        public bool m_RunOnAwake = false;
        [Header("物体运动方向是否朝着路径方向")]
        public bool m_UseLineDir = true;
        public Transform m_moveTs;
        /// <summary>
        /// 一个周期所需的时间
        /// </summary>
        public float m_during = 2;
        public float m_Speed = 100;
        /// <summary>
        /// 使用时间进行控制
        /// </summary>
        public bool m_UseTimeCtr = true;
        [Header(" 移动回节点起始点的速度")]
        public float m_MoveBackSpeed = 5;

        [Header(" 开始循环节点下标从0开始 最大为节点数减2")]
        public int m_LoopPointIndex;
        public float m_RestTimeOnEnd = 0.2f;
        public float m_RestTimeOnStart = 0.2f;
        private float m_timer = 0;

        private event System.Action m_lineRunOverCallBack;
        /// <summary>
        /// 乒乓不是在往前就是在往后
        /// </summary>
        private bool m_pingPangForward = true;
        private bool m_startRun = false;
        private List<BezierNodeObject> bezierNodeObjects = new List<BezierNodeObject>();
        private BezierData bezierData;
        /// <summary>
        /// 节点之间 的路径节点
        /// </summary>
        public Dictionary<int, List<Vector3>> vector3sDic { get; private set; } = new Dictionary<int, List<Vector3>>();

        /// <summary>
        /// 节点数量
        /// </summary>
        private int accuracy;
        private int item = 1;
        /// <summary>
        /// 当前所处路径节点
        /// </summary>
        private int m_currentPointIndex = 0;




        /// <summary>
        /// 存储临时位置信息
        /// </summary>
        private Vector3 m_temptargetPos;


        /// <summary>
        /// 运动速度
        /// </summary>
        public float Speed { get { return accuracy; } }

        private void Start()
        {
            if (this.m_RunOnAwake) this.StartLineRun();
        }

        /// <summary>
        /// 开启运动
        /// </summary>
        /// <param name="moveTaget"></param>
        /// <param name="during"></param>
        public void StartLineRun(Transform moveTaget = null, System.Action runOverCallback = null)
        {
            this.m_currentPointIndex = 0;
            this.item = 0;
            this.m_timer = 0;
            m_movetimer = 0;
            if (moveTaget != null) this.m_moveTs = moveTaget;
            this.m_lineRunOverCallBack = runOverCallback;
            if (this.m_moveTs != null)
            {
                this.ReflashBeierLinePath();
                this.m_moveTs.position = vector3sDic[0][0];
                m_temptargetPos = m_moveTs.position;
                this.m_startRun = true;
            }
        }


        /// <summary>
        /// 开启运动
        /// </summary>
        /// <param name="targetpos"></param>
        public void StartLineRun(Vector3 targetpos, System.Action runOverCallback = null)
        {
            this.m_currentPointIndex = 0;
            this.item = 0;
            this.m_timer = 0;
            m_movetimer = 0;
            this.m_lineRunOverCallBack = runOverCallback;
            if (this.m_moveTs != null)
            {
                this.ReflashBeierLinePath();
                this.SetNodePos(bezierNodeObjects.Count - 1, targetpos);
                this.m_moveTs.position = vector3sDic[0][0];
                m_temptargetPos = m_moveTs.position;
                this.m_startRun = true;
            }
        }


        /// <summary>
        /// 暂定移动
        /// </summary>
        public void PauseMove()
        {
            this.m_startRun = false;
        }


        /// <summary>
        /// 恢复移动 在启动移动之后调用
        /// </summary>
        public void ResumeMove()
        {
            this.m_startRun = true;
        }


        /// <summary>
        /// 设置是否循环
        /// </summary>
        public void SetBezierType(BeZierLineType beZierLineType)
        {
            this.m_MoveType = beZierLineType;
        }


        /// <summary>
        /// 设置目标节点
        /// </summary>
        /// <param name="targetpos"></param>
        public void SetTargetPos(Vector3 targetpos)
        {
            this.SetNodePos(bezierNodeObjects.Count - 1, targetpos);
        }


        /// <summary>
        /// 设置路径节点位置
        /// </summary>
        /// <param name="nodeIndex"></param>
        /// <param name="pos"></param>
        public void SetNodePos(int nodeIndex, Vector3 pos)
        {
            if (bezierNodeObjects.Count == 0)
                bezierNodeObjects.AddRange(this.transform.GetComponentsInChildren<BezierNodeObject>());
            if (bezierNodeObjects.Count > nodeIndex && nodeIndex > -1)
            {
                bezierNodeObjects[nodeIndex].transform.position = pos;
                this.ReflashBeierLinePath();
            }
            else
                MyDebuger.LogError("下标超出边界 请检查下标 " + nodeIndex + " 最大下标 " + bezierNodeObjects.Count);
        }

        /// <summary>
        /// 设置节点斜率
        /// </summary>
        public void SetNodeSlop(int nodeIndex, Vector3 localpos)
        {
            if (bezierNodeObjects.Count == 0)
                bezierNodeObjects.AddRange(this.transform.GetComponentsInChildren<BezierNodeObject>());
            if (bezierNodeObjects.Count > nodeIndex && nodeIndex > -1)
            {
                bezierNodeObjects[nodeIndex].SetNodeOffsetInfo(localpos);
            }
            else
                MyDebuger.LogError("下标超出边界 请检查下标 " + nodeIndex + " 最大下标 " + bezierNodeObjects.Count);
        }


        /// <summary>
        /// 停止运动
        /// </summary>
        public void StopRunLine()
        {
            this.m_startRun = false;
            this.vector3sDic.Clear();
            this.bezierNodeObjects.Clear();
        }


        public void ReflashBeierLinePath()
        {
            item = 1;
            this.vector3sDic.Clear();
            if (bezierNodeObjects.Count == 0)
                bezierNodeObjects.AddRange(this.transform.GetComponentsInChildren<BezierNodeObject>());

            bezierData = ScriptableObject.CreateInstance<BezierData>();
            if (!this.m_UseTimeCtr)
            {
                if (this.m_Speed <= 1) this.m_Speed = 1;
                this.m_during = Vector3.Distance(bezierNodeObjects[0].transform.position, bezierNodeObjects[bezierNodeObjects.Count - 1].transform.position) / this.m_Speed;
            }
            accuracy = Mathf.FloorToInt(this.m_during / Time.fixedDeltaTime);
            accuracy = accuracy / bezierNodeObjects.Count;
            if (accuracy < bezierNodeObjects.Count) accuracy = bezierNodeObjects.Count;
            bezierData.accuracy = accuracy;
            bezierData.SetBezierNode(bezierNodeObjects);
            for (int i = 0; i < bezierNodeObjects.Count - 1; i++)
            {
                if (vector3sDic.ContainsKey(i))
                {
                    vector3sDic[i].Clear();
                    vector3sDic[i].AddRange(bezierData.GetBezierDatas(i));
                }
                else
                {
                    List<Vector3> points = new List<Vector3>();
                    points.AddRange(bezierData.GetBezierDatas(i));
                    vector3sDic[i] = points;
                }
            }
        }

        private float m_movetimer = 0;
        private void FixedUpdate()
        {
            if (LinePause) return;
            if (!this.m_startRun) return;
            if (m_moveTs == null) return;
            this.m_movetimer += LineFixedDeltaTime;
            if (this.m_movetimer < Time.fixedDeltaTime)
                return;
            this.m_movetimer -= Time.fixedDeltaTime;
            switch (this.m_MoveType)
            {
                case BeZierLineType.Normal:
                    this.NormalMove();
                    break;
                case BeZierLineType.PingPang:
                    this.PingPangMove();
                    break;
                case BeZierLineType.Loop:
                    LoopMove();
                    break;
                case BeZierLineType.PartLoop:
                    PartLoopMove();
                    break;
                case BeZierLineType.PartPingPang:
                    PartPingPangMove();
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// 正向移动
        /// </summary>
        void ForwardMove()
        {
            if (this.m_currentPointIndex < this.vector3sDic.Keys.Count)
            {
                List<Vector3> roadPoints = vector3sDic[this.m_currentPointIndex];
                if (item < roadPoints.Count - 1)
                {
                    if (this.m_UseLineDir)
                        m_moveTs.LookAt(roadPoints[item + 1], Vector3.up);
                    m_moveTs.position = roadPoints[item];
                    item++;
                }
                else
                {
                    m_moveTs.position = roadPoints[roadPoints.Count - 1];
                    this.m_currentPointIndex++;
                    item = 0;
                }
            }
        }


        /// <summary>
        /// 反向以移动
        /// </summary>
        void BackMove()
        {
            if (this.m_currentPointIndex >= 0)
            {
                List<Vector3> roadPoints = vector3sDic[this.m_currentPointIndex];
                if (item > 0)
                {
                    if (this.m_UseLineDir)
                        m_moveTs.LookAt(roadPoints[item - 1], Vector3.up);
                    m_moveTs.position = roadPoints[item];
                    item--;
                }
                else
                {
                    m_moveTs.position = roadPoints[0];
                    this.m_currentPointIndex--;
                    if (this.m_currentPointIndex >= 0)
                    {
                        item = vector3sDic[this.m_currentPointIndex].Count - 1;
                    }
                }
            }
        }


        /// <summary>
        /// 正常移动
        /// </summary>
        private void NormalMove()
        {
            if (m_currentPointIndex <= vector3sDic.Keys.Count - 1)
            {
                ForwardMove();
                if (m_currentPointIndex >= vector3sDic.Keys.Count)
                {
                    this.m_lineRunOverCallBack?.Invoke();
                    m_startRun = false;
                }
            }
        }


        /// <summary>
        /// 往返循环移动
        /// </summary>
        private void PingPangMove()
        {
            if (this.m_timer > 0)
            {
                this.m_timer -= Time.fixedDeltaTime;
                return;
            }
            if (!MoveBackTempTarget())
                return;
            if (this.m_pingPangForward)
            {
                if (m_currentPointIndex < 0)
                {
                    m_currentPointIndex = 0;
                    item = 0;
                }
                if (m_currentPointIndex <= vector3sDic.Keys.Count - 1)
                {
                    ForwardMove();
                    if (m_currentPointIndex >= vector3sDic.Keys.Count)
                    {
                        this.m_timer = this.m_RestTimeOnEnd;
                        this.m_pingPangForward = false;

                    }
                }
            }
            else
            {
                if (m_currentPointIndex >= vector3sDic.Keys.Count)
                {
                    m_currentPointIndex = vector3sDic.Keys.Count - 1;
                    item = vector3sDic[m_currentPointIndex].Count - 1;
                }

                if (m_currentPointIndex >= 0)
                {
                    BackMove();
                    if (m_currentPointIndex < 0)
                    {
                        this.m_timer = this.m_RestTimeOnStart;
                        this.m_pingPangForward = true;
                    }
                }
            }
            m_temptargetPos = m_moveTs.position;
        }

        private void LoopMove()
        {
            if (this.m_timer > 0)
            {
                this.m_timer -= Time.fixedDeltaTime;
                return;
            }
            if (!MoveBackTempTarget())
                return;
            if (m_currentPointIndex <= vector3sDic.Keys.Count - 1)
            {
                ForwardMove();
                if (m_currentPointIndex >= vector3sDic.Keys.Count)
                {
                    m_currentPointIndex = 0;
                    item = 0;
                    this.m_timer = this.m_RestTimeOnEnd;
                }
            }
            this.m_temptargetPos = m_moveTs.position;
        }

        /// <summary>
        /// 分段循环运动
        /// </summary>
        void PartLoopMove()
        {
            if (this.m_timer > 0)
            {
                this.m_timer -= Time.fixedDeltaTime;
                return;
            }
            if (!MoveBackTempTarget())
                return;
            if (this.m_currentPointIndex < this.m_LoopPointIndex)
            {
                this.ForwardMove();
            }
            else
            {
                List<Vector3> roadPoints = vector3sDic[this.m_currentPointIndex];
                if (item < roadPoints.Count - 1)
                {
                    if (this.m_UseLineDir)
                        m_moveTs.LookAt(roadPoints[item + 1], Vector3.up);

                    m_moveTs.position = roadPoints[item];
                    item++;
                }
                else
                {
                    m_moveTs.position = roadPoints[roadPoints.Count - 1];
                    item = 0;
                    this.m_timer = this.m_RestTimeOnEnd;
                }
            }
            this.m_temptargetPos = this.m_moveTs.position;
        }


        /// <summary>
        /// 分端乒乓运动
        /// </summary>
        void PartPingPangMove()
        {
            if (this.m_timer > 0)
            {
                this.m_timer -= Time.fixedDeltaTime;
                return;
            }
            if (!MoveBackTempTarget())
                return;
            if (this.m_currentPointIndex < this.m_LoopPointIndex)
            {
                this.ForwardMove();
            }
            else
            {
                List<Vector3> roadPoints = vector3sDic[this.m_currentPointIndex];
                if (this.m_pingPangForward)
                {
                    if (item < roadPoints.Count - 1)
                    {
                        if (this.m_UseLineDir)
                            m_moveTs.LookAt(roadPoints[item + 1], Vector3.up);
                        m_moveTs.position = roadPoints[item];
                        item++;
                    }
                    else
                    {
                        m_moveTs.position = roadPoints[roadPoints.Count - 1];
                        this.m_pingPangForward = false;
                        item = roadPoints.Count - 1;
                        this.m_timer = this.m_RestTimeOnEnd;
                    }
                }
                else
                {
                    if (item > 0)
                    {
                        if (this.m_UseLineDir)
                            m_moveTs.LookAt(roadPoints[item - 1], Vector3.up);
                        m_moveTs.position = roadPoints[item];
                        item--;
                    }
                    else
                    {
                        item = 0;
                        m_moveTs.position = roadPoints[item];
                        this.m_pingPangForward = true;
                        this.m_timer = this.m_RestTimeOnStart;
                    }
                }
            }
            this.m_temptargetPos = this.m_moveTs.position;
        }


        /// <summary>
        /// 移动回临时目标点
        /// </summary>
        /// <returns></returns>
        bool MoveBackTempTarget()
        {
            if (m_temptargetPos != m_moveTs.position)
            {
                m_moveTs.position = Vector3.MoveTowards(m_moveTs.position, m_temptargetPos, m_MoveBackSpeed * Time.deltaTime);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 设置整体时长
        /// </summary>
        /// <param name="during"></param>
        public void SetDuring(float during)
        {
            this.m_during = during;
        }

        /// <summary>
        /// 设置移动速度
        /// </summary>
        /// <param name="movespeed"></param>
        public void SetMoveSpeed(float movespeed)
        {
            this.m_Speed = movespeed;
        }



#if UNITY_EDITOR
        private List<Vector3> m_editorvector3s = new List<Vector3>();
        private List<BezierNodeObject> m_editorbezierNodeObjects = new List<BezierNodeObject>();
        private BezierData m_editorbezierData;
        private void OnDrawGizmos()
        {
            Gizmos.color = m_LineColor;
            if (vector3sDic.Count > 0)
            {
                foreach (var item in vector3sDic)
                {
                    for (int i = 0; i < item.Value.Count - 1; i++)
                    {
                        Gizmos.DrawLine(item.Value[i], item.Value[i + 1]);
                    }
                }
            }
            else
            {
                m_editorbezierNodeObjects.Clear();
                m_editorbezierNodeObjects.AddRange(this.transform.GetComponentsInChildren<BezierNodeObject>());
                if (m_editorbezierNodeObjects.Count < 1) return;

                this.ReflashEditorLinePos();
                for (int i = 0; i < m_editorvector3s.Count - 1; i++)
                {
                    Gizmos.DrawLine(m_editorvector3s[i], m_editorvector3s[i + 1]);
                }

                if (this.m_MoveType == BeZierLineType.PartLoop || this.m_MoveType == BeZierLineType.PartPingPang)
                {
                    if (m_editorbezierNodeObjects.Count < 2)
                    {
                        MyDebuger.LogError("曲线节点少于2 请检查曲线设置");
                    }
                    else
                    {
                        if (this.m_LoopPointIndex >= m_editorbezierNodeObjects.Count - 1)
                        {
                            this.m_LoopPointIndex = this.m_editorbezierNodeObjects.Count - 2;
                        }
                        if (this.m_LoopPointIndex < 0) this.m_LoopPointIndex = 0;
                    }
                }
            }
        }


        void ReflashEditorLinePos()
        {
            m_editorbezierData = ScriptableObject.CreateInstance<BezierData>();
            if (!this.m_UseTimeCtr)
            {
                if (this.m_Speed <= 1) this.m_Speed = 1;
                this.m_during = Vector3.Distance(m_editorbezierNodeObjects[0].transform.position, m_editorbezierNodeObjects[m_editorbezierNodeObjects.Count - 1].transform.position) / this.m_Speed;
            }
            accuracy = Mathf.FloorToInt(this.m_during / Time.fixedDeltaTime);
            accuracy = accuracy / m_editorbezierNodeObjects.Count;
            if (accuracy < m_editorbezierNodeObjects.Count) accuracy = m_editorbezierNodeObjects.Count;
            m_editorbezierData.accuracy = accuracy;
            m_editorbezierData.SetBezierNode(m_editorbezierNodeObjects);
            m_editorvector3s.Clear();
            for (int i = 0; i < m_editorbezierNodeObjects.Count; i++)
            {
                if (i > 0 && i < m_editorbezierNodeObjects.Count - 1)
                    m_editorbezierNodeObjects[i].SetIsMiddleNode(true);
                else
                    m_editorbezierNodeObjects[i].SetIsMiddleNode(false);
                if (i < m_editorbezierNodeObjects.Count - 1)
                    m_editorvector3s.AddRange(m_editorbezierData.GetBezierDatas(i));
            }
        }

#endif
        protected override void OnBeforeDestroy()
        {
        }


    }

}
