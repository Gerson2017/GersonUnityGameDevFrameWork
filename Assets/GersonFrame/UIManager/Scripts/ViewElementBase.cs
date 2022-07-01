using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GersonFrame.UI
{

    /// <summary>
    /// 一个物体结尾为_Txt 需要自动生成Text组件
    /// 一个物体结尾为_Btn 需要自动生成Button组件
    /// 一个物体结尾为_RawImg 需要自动生成RawImage组件
    /// 一个物体结尾为_Img 需要自动生成Image组件
    /// 一个物体结尾为_Ts 需要自动生成Transform组件
    /// 一个物体结尾为_RectTs 需要自动生成RectTransform组件
    /// 一个物体结尾为_ScRect 需要自动生成ScrollRect组件
    /// 一个物体结尾为_Go 需要自动生成GameObject组件
    /// 一个物体结尾为_Ps 需要自动生成ParticleSystem组件
    /// 一个物体结尾为_Am 需要自动生成Animator组件
    /// 一个物体结尾为_Sp 需要自动生成SpriteRender组件
    /// 一个物体结尾为_Rend 需要自动生成Renderer组件 
    /// 一个物体结尾为_Col 需要自动生成Collider组件 
    /// 一个物体结尾为_Mesh 需要自动生成Mesh组件 
    /// 一个物体结尾为_Skin 需要自动生成SkinnedMeshRenderer组件 
    /// 一个物体结尾为_Slider需要自动生成Slider组件 
    /// 一个物体结尾为_Rg需要自动生成Rigidbody组件 
    /// </summary>
    public class ViewElementBase
    {
      public ViewElementBase() { }
        public ViewElementBase(GameObject go)
        {
            Init(go);
        }

        public void Init(GameObject go)
        {
            bool isactive = go.activeInHierarchy;
            if (!isactive) go.Show();
            InitElement(go);
            if (!isactive) go.Hide();
        }

        public virtual void InitElement(GameObject go)
        {

        }

    }
}
