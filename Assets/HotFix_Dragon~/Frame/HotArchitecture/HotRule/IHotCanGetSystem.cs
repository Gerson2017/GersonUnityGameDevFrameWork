using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HotGersonFrame
{

    public interface IHotCanGetSystem : IHotBelongToArchitecture
    {

    }


    /// <summary>
    /// 通过静态扩展 ICanGetSystem 接口的对象获取到 ISystem
    /// </summary>
    public static class CanGetSystem
    {
        public static T GetSystem<T>(this IHotCanGetSystem self)where T:class,IHotSystem
        {
            return self.Architecture.GetSystem<T>();
        }
    }

}
