using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GersonFrame
{
    public interface ICanGetModel : IBelongToArchitecture
    {

    }


    /// <summary>
    /// 使用静态扩展 赋予使用ICanGetModel 接口的对象获取到 IModel的接口方法 
    /// </summary>
    public static class CanGetModelExtension
    {
        public static T GetModel<T>(this ICanGetModel self) where T :  class,IModel
        {
            return self.Architecture.GetModel<T>();
        }
    }

}
