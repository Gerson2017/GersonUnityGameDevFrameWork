using UnityEngine;


namespace GersonFrame.Tool
{


    /// <summary>
    /// 抛物线运动轨迹
    /// ZhangYu 2019-04-28
    /// </summary>
    public class ParabolaPath
    {
        /// <summary> 是否夹紧起点和终点 </summary>
        public bool isClampStartEnd;
        private Vector3 m_start;
        private Vector3 m_end;
        private float m_height;
        private float m_gravity;
        private float m_upTime;
        private float m_downTime;
        private float m_totalTime;
        private Vector3 m_velocityStart;
        private Vector3 m_position;
        private float m_time;


        public ParabolaPath()
        {

        }

        /// <summary> 初始化抛物线运动轨迹 </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <param name="height">高度(相对于两个点的最高位置 高出多少)</param>
        /// <param name="gravity">重力加速度(负数)</param>
        /// <returns></returns>
        public ParabolaPath(Vector3 start, Vector3 end, float height = 10, float gravity = -9.8f)
        {
            Init(start, end, height, gravity);
        }

        /// <summary> 初始化抛物线运动轨迹 </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <param name="height">高度(相对于两个点的最高位置 高出多少)</param>
        /// <param name="gravity">重力加速度(负数)</param>
        /// <returns></returns>
        public void Init(Vector3 start, Vector3 end, float height = 10, float gravity = -9.8f)
        {
            float topY = Mathf.Max(start.y, end.y) + height;
            float d1 = topY - start.y;
            float d2 = topY - end.y;
            float g2 = 2 / -gravity;
            float t1 = Mathf.Sqrt(g2 * d1);
            float t2 = Mathf.Sqrt(g2 * d2);
            float t = t1 + t2;
            float vX = (end.x - start.x) / t;
            float vZ = (end.z - start.z) / t;
            float vY = -gravity * t1;
            m_start = start;
            m_end = end;
            m_height = height;
            m_gravity = gravity;
            m_upTime = t1;
            m_downTime = t2;
            m_totalTime = t;
            m_velocityStart = new Vector3(vX, vY, vZ);
            m_position = m_start;
            m_time = 0;
        }


        public void InitByDuring(Vector3 start, Vector3 end, float height = 10, float during=1)
        {
            float topY = Mathf.Max(start.y, end.y) + height;
            float d1 = topY - start.y;
            float halftime = during / 2;
            float gravity = -2 * d1 / (halftime * halftime);
            float t = during;
            float vX = (end.x - start.x) / t;
            float vZ = (end.z - start.z) / t;
            float vY = -gravity * halftime;
            m_start = start;
            m_end = end;
            m_height = height;
            m_gravity = gravity;
            m_upTime = halftime;
            m_downTime = halftime;
            m_totalTime = t;
            m_velocityStart = new Vector3(vX, vY, vZ);
            m_position = m_start;
            m_time = 0;
        }


        /// <summary> 起点 </summary>
        public Vector3 start { get { return m_start; } }
        /// <summary> 终点 </summary>
        public Vector3 end { get { return m_end; } }
        /// <summary> 目标高度 </summary>
        public float height { get { return m_height; } }
        /// <summary> 重力加速度 </summary>
        public float gravity { get { return m_gravity; } }
        /// <summary> 上升时间 </summary>
        public float upTime { get { return m_upTime; } }
        /// <summary> 下降时间 </summary>
        public float downTime { get { return m_downTime; } }
        /// <summary> 总运动时间 </summary>
        public float totalTime { get { return m_totalTime; } }
        /// <summary> 顶点 </summary>
        public Vector3 top { get { return GetPosition(m_upTime); } }
        /// <summary> 初始速度 </summary>
        public Vector3 velocityStart { get { return m_velocityStart; } }
        /// <summary> 当前位置 </summary>
        public Vector3 position { get { return m_position; } }
        /// <summary> 当前速度 </summary>
        public Vector3 velocity { get { return GetVelocity(m_time); } }

        /// <summary> 当前时间 </summary>
        public float time
        {
            get { return m_time; }
            set
            {
                if (isClampStartEnd) value = Mathf.Clamp(value, 0, m_totalTime);
                m_time = value;
                m_position = GetPosition(value);
            }
        }

        /// <summary> 获取某个时间点的位置 </summary>
        public Vector3 GetPosition(float time)
        {
            if (time == 0) return m_start;
            if (time == m_totalTime) return m_end;
            float dY = 0.5f * m_gravity * time * time;
            return m_start + m_velocityStart * time + new Vector3(0, dY, 0);
        }


        /// <summary> 获取某个时间点的速度 </summary>
        public Vector3 GetVelocity(float time)
        {
            if (time == 0) return m_velocityStart;
            return m_velocityStart + new Vector3(0, m_velocityStart.y + m_gravity * time, 0);
        }

    }
}