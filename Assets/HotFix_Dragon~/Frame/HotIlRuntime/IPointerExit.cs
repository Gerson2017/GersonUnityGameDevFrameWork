
using UnityEngine.EventSystems;

namespace HotGersonFrame.HotIlRuntime
{
    ///<summary>
    /// 创建人：Gerson
    /// 日 期：2022/2/23 15:37:19
    /// 描 述：1.子节点通知父节点用委托或事件
    ///2.父节点调用子节点可以直接方法调用
    ///3.跨模块通信用事件
    ///4.耦合就是双向引用或循环引用
    ///</summary>
    public interface IPointerExit : IMono
    {
        void OnPointerExit(PointerEventData eventData);
    }
}
