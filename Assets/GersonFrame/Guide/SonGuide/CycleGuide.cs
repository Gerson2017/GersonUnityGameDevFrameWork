using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleGuide : BaseGuide
{
    private float m_currentRadiu;


    /// <summary>
    /// 圆半径
    /// </summary>
    private float m_radius;
    public override void Guide(Canvas canvas, RectTransform target, TranslateType translateType = TranslateType.Direct, float centertime = 1)
    {
        base.Guide(canvas, target,translateType,centertime);
        //计算宽高
        float width = m_targetCorners[3].x - m_targetCorners[0].x;
        float height = m_targetCorners[1].y - m_targetCorners[0].y;
        //计算半径
        this.m_radius = Mathf.Sqrt(width*width+height*height)/2;
        this.m_material.SetFloat("_Slider", this.m_radius);
    }

    public override void Guide(Canvas canvas, RectTransform target, float scal, float time, TranslateType translateType = TranslateType.Direct, float centertime = 1)
    {
        this.Guide(canvas, target, translateType, centertime);
        //设置材质
        this.m_scalTime = time;
        this.m_scaltimer = 0;
        this.m_isScaling = true;
        m_currentRadiu = scal*this.m_radius;
        this.m_material.SetFloat("_Slider", m_currentRadiu);
    }


    protected override void Update()
    {
        base.Update();
        if (this.m_isScaling) this.m_material.SetFloat("_Slider", Mathf.Lerp(this.m_currentRadiu, this.m_radius, this.m_scaltimer));
    }


}
