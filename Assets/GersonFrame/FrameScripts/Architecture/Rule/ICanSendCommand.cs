using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GersonFrame
{
    public interface ICanSendCommand:IBelongToArchitecture
    {
    }

    /// <summary>
    /// 通过静态扩展 ICanSendCommand 接口的对象使用 发送Command mingl
    /// </summary>
    public static class CanSendCommandExtension
    {
        public static void SendCommand<T>(this ICanSendCommand self, object arg1 = null, object arg2 = null, object arg3 = null) where T : ICommand, new()
        {
            self.SendCommand<T>(arg1,arg2,arg3);
        }

        public static void SendCommand<T>(this ICanSendCommand self, T command, object arg1 = null, object arg2 = null, object arg3 = null) where T : ICommand, new()
        {
            self.SendCommand<T>(command, arg1, arg2, arg3);
        }
    }
}

