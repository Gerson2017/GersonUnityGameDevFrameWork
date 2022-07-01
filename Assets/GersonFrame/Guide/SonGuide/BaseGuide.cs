using GersonFrame.Tool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 中心点移动
/// </summary>
public enum TranslateType
{
    Direct,//直接切换
    Slowly,//缓慢移动
}

[RequireComponent(typeof(Image))]
public class BaseGuide : MonoBehaviour
{
    #region Scal变化相关
    protected float m_scaltimer = 0;
    protected float m_scalTime = 1;
    protected bool m_isScaling = false;
    #endregion

    #region 中心点移动相关
    protected float m_centertimer = 0;
    protected float m_centerTime = 1;
    protected bool m_isMoving= false;
    #endregion

    private Vector3 m_startCenter;

    protected Material m_material;
    /// <summary>
    /// 中心
    /// </summary>
    protected Vector3 m_center;
    /// <summary>
    /// 中心
    /// </summary>
    public Vector3 Center
    {
        get {
            if (this.m_material!=null)
                return this.m_material.GetVector("_Center");
            return Vector3.zero;
        }
    }

    /// <summary>
    /// 镂空目标
    /// </summary>
    protected RectTransform m_target;
    /// <summary>
    /// 目标四个边界点
    /// </summary>
    protected Vector3[] m_targetCorners = new Vector3[4];



    /// <summary>
    /// 引导
    /// </summary>
    /// <param name="target"></param>
    /// <param name="canvas">用来传递UI相机</param>
    public virtual void Guide(Canvas canvas, RectTransform target,TranslateType translateType=TranslateType.Direct,float centertime=1)
    {
        this.m_material = transform.GetComponent<Image>().material;
        this.m_target = target;
        if (target!=null)
        {
            //获取四个边界点的世界坐标
            this.m_target.GetWorldCorners(m_targetCorners);
            //转换成屏幕坐标
            for (int i = 0; i < m_targetCorners.Length; i++)
            {
                m_targetCorners[i] = WorldToScreenPoint(canvas, m_targetCorners[i]);
            }
            //计算中心点坐标
            m_center.x = m_targetCorners[0].x + (m_targetCorners[3].x - m_targetCorners[0].x) / 2;
            m_center.y = m_targetCorners[0].y + (m_targetCorners[1].y - m_targetCorners[0].y) / 2;
            switch (translateType)
            {
                case TranslateType.Direct:
                    this.m_material.SetVector("_Center", this.m_center);
                    break;
                case TranslateType.Slowly:
                    this.m_startCenter = this.m_material.GetVector("_Center");
                    this.m_isMoving = true;
                    this.m_centerTime = centertime;
                    this.m_centertimer = 0;
                    break;
                default:
                    MyDebuger.LogError("can not found translate type " + translateType);
                    break;
            }
        }
        else
        {
            this.m_center = Vector3.zero;
            m_targetCorners[0] = new Vector3(-2000,-2000,0);
            m_targetCorners[1] = new Vector3(-2000, 2000, 0);
            m_targetCorners[2] = new Vector3(2000, -2000, 0);
            m_targetCorners[3] = new Vector3(2000, 2000, 0);

        }

    
    }

    /// <summary>
    /// 引导动画时间
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="target"></param>
    /// <param name="time"></param>
    public virtual void Guide(Canvas canvas, RectTransform target,float scal, float time, TranslateType translateType = TranslateType.Direct, float centertime = 1)
    {

    }

    /// <summary>
    /// 世界坐标转换成屏幕坐标
    /// </summary>
    public Vector2 WorldToScreenPoint(Canvas canvas, Vector3 worldPos)
    {
        //转换成屏幕坐标
        Vector2 screenpos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, worldPos);
        Vector2 localpoint;
        //转换成中心点在00点的局部坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenpos, canvas.worldCamera, out localpoint);
        return localpoint;
    }


    protected virtual void Update()
    {
        if (this.m_isScaling)
        {
            this.m_scaltimer += Time.deltaTime * 1 / m_scalTime;
            if (this.m_scaltimer >= 1)
            {
                this.m_scaltimer = 0;
                this.m_isScaling = false;
            }
        }
        if (this.m_isMoving)
        {
            this.m_centertimer += Time.deltaTime * 1 / this.m_centerTime;
            this.m_material.SetVector("_Center", Vector3.Lerp(m_startCenter, m_center, m_centertimer));
            if (this.m_centertimer >= 1)
            {
                this.m_centertimer = 0;
                this.m_isMoving = false;
            }
        }
    }
}
