using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RectGuide : BaseGuide
{
    /// <summary>
    ///遮罩放大的目标宽度
    /// </summary>
    private float m_scalwidth;
    /// <summary>
    /// 遮罩放大的目标高度
    /// </summary>
    private float m_scalHeight;


    /// <summary>
    /// 宽
    /// </summary>
    private float m_width;
    /// <summary>
    /// 高
    /// </summary>
    private float m_height;

    /// <summary>
    /// 引导
    /// </summary>
    /// <param name="target"></param>
    /// <param name="canvas">用来传递UI相机</param>
    public override void Guide(Canvas canvas, RectTransform target, TranslateType translateType = TranslateType.Direct, float centertime = 1)
    {
        base.Guide(canvas, target, translateType,centertime);
        //计算宽高
        m_width = (m_targetCorners[3].x - m_targetCorners[0].x)/2;
        m_height = (m_targetCorners[1].y - m_targetCorners[0].y)/2;

        //设置宽高
        this.m_material.SetFloat("_SliderX", m_width);
        this.m_material.SetFloat("_SliderY", m_height);
    }


    public override void Guide(Canvas canvas, RectTransform target, float scal, float time, TranslateType translateType = TranslateType.Direct, float centertime = 1)
    {
        this.Guide(canvas, target, translateType, centertime);
        this.m_isScaling = true;
        this.m_scalTime = time;
        this.m_scaltimer = 0;
        this.m_scalwidth = m_width * scal;
        this.m_scalHeight = m_height * scal;
        this.m_material.SetFloat("_SliderX", this.m_scalwidth);
        this.m_material.SetFloat("_SliderY", this.m_scalHeight);
    }

    protected override void Update()
    {
        base.Update();
        if (this.m_isScaling)
        {
           this.m_material.SetFloat("_SliderX", Mathf.Lerp(this.m_scalwidth, this.m_width, this.m_scaltimer));
            this.m_material.SetFloat("_SliderY", Mathf.Lerp(this.m_scalHeight, this.m_height, this.m_scaltimer));
        }
    }

}
