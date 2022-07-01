using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 namespace GersonFrame.Tool
{

public class BezierNodeObject : MonoBehaviour
{
    private Transform m_bezierOffset1;
    public Transform BezierOffset1
    {
        get
        {
            if (this.m_bezierOffset1 == null)
            {
                this.m_bezierOffset1 = transform.GetChild(0);
            }
            return this.m_bezierOffset1;
        }
    }
    private BezierNode bezierNode1;

    private Transform m_bezierOffset2;
    public Transform BezierOffset2
    {
        get
        {
            if (this.m_bezierOffset2 == null)
            {
                if (transform.childCount>1)
                {
                    this.m_bezierOffset2 = transform.GetChild(1);
                }

            }
            return this.m_bezierOffset2;
        }
    }
    private BezierNode bezierNode2;

    public BezierNode GetBezierNode1()
    {
        if (BezierOffset1 == null) return default(BezierNode);
        bezierNode1.nodeOffset = BezierOffset1.position;
        bezierNode1.nodePos = transform.position;
        return bezierNode1;
    }

    public BezierNode GetBezierNode2()
    {
        if (BezierOffset2 == null) return default(BezierNode);
        bezierNode2.nodeOffset = BezierOffset2.position;
        bezierNode2.nodePos = transform.position;
        return bezierNode2;
    }


  public  void SetIsMiddleNode(bool ismiddle)
    {
        if (transform.childCount<1)
        {
            GameObject ts=  new GameObject("offset1");
            ts.transform.SetParent(this.transform);
            ts.transform.localPosition = new Vector3(0,0,3);
        }
        if (ismiddle)
        {
            if (transform.childCount<2)
            {
                GameObject ts = new GameObject("offset2");
                ts.transform.SetParent(this.transform);
                ts.transform.localPosition = new Vector3(0, 0, -3);
            }
        }
        else
        {
            if (transform.childCount>1)
            {
                for (int i = transform.childCount-1; i>0; i--)
                {
                    DestroyImmediate(transform.GetChild(i).gameObject);
                }
            }
        }
    }

    /// <summary>
    /// 设置斜率
    /// </summary>
    public void SetNodeOffsetInfo(Vector3 localpos,int nodeinex=0)
    {
        if (nodeinex==0)
        {
            BezierOffset1.localPosition = localpos;
        }
        else
        {
            BezierOffset2.localPosition = localpos;
        }
      
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            Gizmos.DrawWireSphere(transform.position, 1);
            Gizmos.DrawWireCube(BezierOffset1.position, Vector3.one);
            Debug.DrawLine(BezierOffset1.position, transform.position - (BezierOffset1.position - transform.position), Color.yellow);

            if (BezierOffset2!=null)
            {
                Gizmos.DrawWireCube(BezierOffset2.position, Vector3.one);
                Debug.DrawLine(BezierOffset2.position, transform.position - (BezierOffset2.position - transform.position), Color.yellow);
            }

        }
    }
#endif

}
}
