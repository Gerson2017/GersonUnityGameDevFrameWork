
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotFix_Dragon
{
    public interface IViewBase
    {
        void AddUIListener();
        void RegisterMsgListener();
        void RegisterNetMsg();
        void UnRegisterMsgListener();
        void UnRegisterNetMsg();
    }
}
