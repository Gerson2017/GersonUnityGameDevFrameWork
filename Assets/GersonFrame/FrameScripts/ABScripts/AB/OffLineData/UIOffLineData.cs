using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GersonFrame.ABFrame
{

    public class UIOffLineData : OffLineData
    {
        public Vector2[] m_AnchorMax;
        public Vector2[] m_AnchorMin;
        public Vector2[] m_Pivot;
        public Vector2[] m_SizeDelta;
        public Vector3[] m_AnchoredPos;
        public ParticleSystem[] m_Particles;


        public override void ResetPrpo()
        {
            int allPointCount = m_AllPoints.Length;
            for (int i = 0; i < allPointCount; i++)
            {
                RectTransform tempts = m_AllPoints[i] as RectTransform;
                if (tempts != null)
                {
                    tempts.localPosition = m_Poss[i];
                    tempts.localRotation = m_Rots[i];
                    tempts.localScale = m_Scales[i];
                    tempts.gameObject.SetActive(this.m_AllPointActives[i]);
                    tempts.anchorMax = m_AnchorMax[i];
                    tempts.anchorMin = m_AnchorMin[i];
                    tempts.pivot = m_Pivot[i];
                    tempts.sizeDelta = m_SizeDelta[i];
                    tempts.anchoredPosition3D = m_AnchoredPos[i];
                }

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

            int particlecount = this.m_Particles.Length;
            for (int i = 0; i < particlecount; i++)
            {
                m_Particles[i].Clear(true);
                m_Particles[i].Play();
            }
        }

        public override void BindData()
        {
            Transform[] allts = gameObject.GetComponentsInChildren<Transform>(true);
            int alltsCount = allts.Length;
            for (int i = 0; i < alltsCount; i++)
            {
                if (!(allts[i] is RectTransform))
                {
                    allts[i].gameObject.AddComponent<RectTransform>();
                }
            }
            m_AllPoints = gameObject.GetComponentsInChildren<RectTransform>(true);
            m_Particles = gameObject.GetComponentsInChildren<ParticleSystem>(true);
            int allpointcount = m_AllPoints.Length;
            this.m_AllPointChildCounts = new int[allpointcount];
            this.m_AllPointActives = new bool[allpointcount];
            this.m_Poss = new Vector3[allpointcount];
            this.m_Rots = new Quaternion[allpointcount];
            this.m_Scales = new Vector3[allpointcount];

            this.m_AnchorMax = new Vector2[allpointcount];
            this.m_AnchorMin = new Vector2[allpointcount];
            this.m_Pivot = new Vector2[allpointcount];
            this.m_SizeDelta = new Vector2[allpointcount];
            this.m_AnchoredPos = new Vector3[allpointcount];

            for (int i = 0; i < allpointcount; i++)
            {
                RectTransform temp = m_AllPoints[i] as RectTransform;
                this.m_AllPointChildCounts[i] = temp.childCount;
                this.m_AllPointActives[i] = temp.gameObject.activeSelf;
                this.m_Poss[i] = temp.localPosition;
                this.m_Rots[i] = temp.localRotation;
                this.m_Scales[i] = temp.localScale;

                this.m_AnchorMax[i] = temp.anchorMax;
                this.m_AnchorMin[i] = temp.anchorMin;
                this.m_Pivot[i] = temp.pivot;
                this.m_SizeDelta[i] = temp.sizeDelta;
                this.m_AnchoredPos[i] = temp.anchoredPosition3D;
            }
        }

    }
}
