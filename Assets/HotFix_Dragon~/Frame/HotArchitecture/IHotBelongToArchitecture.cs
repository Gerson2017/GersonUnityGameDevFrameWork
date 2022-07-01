using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HotGersonFrame
{
    /// <summary>
    /// 属于某一架构接口
    /// </summary>
    public interface IHotBelongToArchitecture
    {
        IHotArchitecture Architecture { get; }
    }
}
