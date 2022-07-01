using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotGersonFrame
{

    /// <summary>
    /// 定义可以使用Utility的接口
    /// </summary>
    public interface IHotCanGetUtility : IHotBelongToArchitecture
    {
    }

    /// <summary>
    /// 通过静态扩展 赋予使用ICanGetUtility 接口的对象获取到 IUtility
    /// </summary>
    public static class CanGetUtilityExtension
    {
        public static T GetUtility<T>(this IHotCanGetUtility self) where T : class, IHotUtility
        {
            return self.Architecture.GetUtility<T>();
        }
    }

}
