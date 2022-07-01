using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GersonFrame.ABFrame
{

    public class OffLineData : MonoBehaviour
    {
        public Rigidbody m_RigiBody;
        public Collider m_Collider;
        public Transform[] m_AllPoints;
        public int[] m_AllPointChildCounts;
        public bool[] m_AllPointActives;
        public Vector3[] m_Poss;
        public Vector3[] m_Scales;
        public Quaternion[] m_Rots;


        /// <summary>
        /// 还原属性
        /// </summary>
        public virtual void ResetPrpo()
        {
            int allpointCount = m_AllPoints.Length;
            for (int i = 0; i < allpointCount; i++)
            {
                Transform tempts = m_AllPoints[i];
                if (tempts != null)
                {
                    tempts.localPosition = m_Poss[i];
                    tempts.localRotation = m_Rots[i];
                    tempts.localScale = m_Scales[i];
                    tempts.gameObject.SetActive(this.m_AllPointActives[i]);

                    ///是否有多余出来的节点
                    if (tempts.childCount > m_AllPointChildCounts[i])
                    {
                        int childcount = tempts.childCount;
                        for (int j = m_AllPointChildCounts[i]; j < childcount; j++)
                        {
                            GameObject tempgo = tempts.GetChild(j).gameObject;
                            ///是否是由对象池创建出来的 是的话不用做处理 否则删除
                            if (!ObjectManager.Instance.IsObjectManangerCreate(tempgo))
                            {
                                Destroy(tempgo);
                            }
                        }
                    }

                }
            }
        }

        /// <summary>
        /// 编辑器下绑定初始数据
        /// </summary>
        public virtual void BindData()
        {
            this.m_Collider = this.GetComponentInChildren<Collider>(true);
            this.m_RigiBody = this.GetComponentInChildren<Rigidbody>(true);
            this.m_AllPoints = this.GetComponentsInChildren<Transform>(true);
            int allPointCount = this.m_AllPoints.Length;
            this.m_AllPointChildCounts = new int[allPointCount];
            this.m_AllPointActives = new bool[allPointCount];
            this.m_Poss = new Vector3[allPointCount];
            this.m_Rots = new Quaternion[allPointCount];
            this.m_Scales = new Vector3[allPointCount];
            for (int i = 0; i < allPointCount; i++)
            {
                Transform temp = m_AllPoints[i] as Transform;
                this.m_AllPointChildCounts[i] = temp.childCount;
                this.m_AllPointActives[i] = temp.gameObject.activeSelf;
                this.m_Poss[i] = temp.localPosition;
                this.m_Rots[i] = temp.localRotation;
                this.m_Scales[i] = temp.localScale;
            }
        }
    }
}
