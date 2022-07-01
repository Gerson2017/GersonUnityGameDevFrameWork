using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GersonFrame
{

    public interface ICanGetSystem : IBelongToArchitecture
    {

    }


    /// <summary>
    /// 通过静态扩展 ICanGetSystem 接口的对象获取到 ISystem
    /// </summary>
    public static class CanGetSystem
    {
        public static T GetSystem<T>(this ICanGetSystem self)where T:class,ISystem
        {
            return self.GetSystem<T>();
        }
    }

}
