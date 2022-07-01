using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GersonFrame
{
    /// <summary>
    /// 属于某一架构接口
    /// </summary>
    public interface IBelongToArchitecture
    {
        IArchitecture Architecture { get; }
    }
}
