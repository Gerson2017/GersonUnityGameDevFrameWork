using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GersonFrame
{
    /// <summary>
    /// 子类实现的时候 要显示实现  IArchitecture IBelongToArchitecture.GetArchitecture()  阉割掉获取GetArchitecture 方法
    /// </summary>
    public interface IController : IBelongToArchitecture,ICanSendCommand,ICanGetSystem,ICanGetModel
    {
    }
}
