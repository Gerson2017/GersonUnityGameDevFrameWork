using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotGersonFrame.Tool
{
    ///<summary>
    /// 创建人：Gerson
    /// 日 期：2022/5/23 16:46:24
    /// 描 述：1.子节点通知父节点用委托或事件
    ///2.父节点调用子节点可以直接方法调用
    ///3.跨模块通信用事件
    ///4.耦合就是双向引用或循环引用
    ///</summary>
    public class HotSimpleObjectPool<T>
    {

        private readonly Stack<T> m_Stack;
        private readonly Func<T> m_ctor;
        private readonly Action<T> m_OnRecycle;
        private int m_Size;
        private int m_UsedCount;


        public HotSimpleObjectPool(int max = 5, Action<T> actionOnReset = null, Func<T> ctor = null)
        {
            m_Stack = new Stack<T>(max);
            m_Size = max;
            m_OnRecycle = actionOnReset;
            m_ctor = ctor;
        }


        public T Get()
        {
            T item;
            if (m_Stack.Count == 0)
            {
                if (null != m_ctor)
                {
                    item = m_ctor();
                }
                else
                {
                    item = Activator.CreateInstance<T>();
                }
            }
            else
            {
                item = m_Stack.Pop();
            }
            m_UsedCount++;
            return item;
        }

        public void Recycle(T item)
        {
            if (m_OnRecycle != null)
            {
                m_OnRecycle.Invoke(item);
            }
            if (m_Stack.Count < m_Size)
            {
                m_Stack.Push(item);
            }
            m_UsedCount--;
        }


        /*
        public T GetAndAutoRecycle()
        {
            T obj = Get();
            Utils.OnNextFrameCall(()=> { Recycle(obj); });
            return obj;
        }
        */

        public void Purge()
        {
            // TODO
        }


        public override string ToString()
        {
            return string.Format("SimpleObjPool: item=[{0}], inUse=[{1}], restInPool=[{2}/{3}] ", typeof(T), m_UsedCount, m_Stack.Count, m_Size);
        }

    }
}
