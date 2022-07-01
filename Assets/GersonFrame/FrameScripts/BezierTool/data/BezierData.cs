using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GersonFrame.Tool
{



    [System.Serializable]
    public struct BezierNode
    {
        [Header("节点位置")]
        public Vector3 nodePos;
        [Header("节点平滑系数")]
        public Vector3 nodeOffset;
        public Vector3 getReverseNodeOffset()
        {
            return nodePos - (nodeOffset - nodePos);
        }
    }
    public class BezierData : ScriptableObject
    {
        [Header("数据集名称")]
        public string DataName;
        [Header("数据节点集")]
        public List<BezierNode> bezierNodes;
        [Header("精度系数，越大越平滑，性能消耗越高"), Range(10, 100)]
        public int accuracy = 10;

        /// <summary>
        /// 计算并返回指定一段曲线的坐标位置数组
        /// </summary>
        /// <param name="region">区间下标数值</param>
        /// <returns></returns>
        public Vector3[] GetBezierDatas(int region)
        {
            if (region < bezierNodes.Count)
            {
                Vector3[] datas = new Vector3[accuracy];
                int index = region;
                if (region > 0)
                    index = region * 2;
                for (int i = 0; i < accuracy; i++)
                {
                    BezierMath.Bezier_3ref(
                        ref datas[i],
                        bezierNodes[index].nodePos,
                        bezierNodes[index].nodeOffset,
                        bezierNodes[index + 1].nodeOffset,
                        bezierNodes[index + 1].nodePos,
                        i / (accuracy - 1.0f)
                    );
                }
                return datas;
            }
            return null;
        }

        public void GetBezuerDatas(int region, List<Vector3> datas)
        {
            if (region < bezierNodes.Count)
            {
                datas.Clear();
                for (int i = 0; i < accuracy; i++)
                {
                    datas[i] = BezierMath.Bezier_3(
                        bezierNodes[region].nodePos,
                        bezierNodes[region].getReverseNodeOffset(),
                        bezierNodes[region + 1].nodeOffset,
                        bezierNodes[region + 1].nodePos,
                        i / (accuracy - 1.0f)
                    );
                }
            }
        }

        /// <summary>
        /// 获取指定曲线区间上的坐标信息
        /// </summary>
        /// <param name="region"></param>
        /// <param name="t"></param>
        /// <param name="data"></param>
        public void GetBezuerData(int region, float t, ref Vector3 data)
        {
            if (region < bezierNodes.Count)
            {
                BezierMath.Bezier_3ref(
                    ref data,
                    bezierNodes[region].nodePos,
                    bezierNodes[region].getReverseNodeOffset(),
                    bezierNodes[region + 1].nodeOffset,
                    bezierNodes[region + 1].nodePos,
                    t
                );
            }
        }

        public void SetBezierNode(List<BezierNodeObject> bezierNodeObjects)
        {
            if (bezierNodes == null) bezierNodes = new List<BezierNode>();
            bezierNodes.Clear();

            foreach (var item in bezierNodeObjects)
            {
                if (item.BezierOffset1 != null)
                {
                    bezierNodes.Add(item.GetBezierNode1());
                }
                if (item.BezierOffset2 != null)
                {
                    bezierNodes.Add(item.GetBezierNode2());
                }
            }
        }
        public static BezierData CreateWithNodeObjects(List<BezierNodeObject> bezierNodeObjects)
        {
            BezierData bezierData = new BezierData();
            bezierData.SetBezierNode(bezierNodeObjects);
            return bezierData;
        }
    }
}