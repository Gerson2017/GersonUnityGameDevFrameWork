using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotFix_Dragon
{

    public class HotClassObjectPool<T> where T : class, new()
    {
        protected Stack<T> m_Pool = new Stack<T>();
        /// <summary>
        /// 最大对象个数 <=0 表示不限制个数
        /// </summary>
        protected int m_maxCount = 10;
        /// <summary>
        /// 没有回收的对象个数
        /// </summary>
        protected int m_noReceiveCount = 0;

        public List<T> m_UsingPool = new List<T>();
        public HotClassObjectPool(int maxcount)
        {
            m_maxCount = maxcount;
            for (int i = 0; i < m_maxCount; i++)
            {
                m_Pool.Push(new T());
            }
        }

        /// <summary>
        /// 从池子里取对象 
        /// </summary>
        /// <param name="createIfPoolEmpty">如果为空是否new一个</param>
        /// <returns></returns>
        public T Spwan(bool createIfPoolEmpty)
        {
            if (m_Pool.Count > 0)
            {
                T t = m_Pool.Pop();
                if (t == null)
                {
                    if (createIfPoolEmpty)
                        t = new T();
                }
                m_noReceiveCount++;
                m_UsingPool.Add(t);
                return t;
            }
            else
            {
                if (createIfPoolEmpty)
                {
                    T t = new T();
                    m_noReceiveCount++;
                    m_UsingPool.Add(t);
                    return t;
                }
            }
            return null;
        }



        /// <summary>
        /// 回收类对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Recycle(T obj)
        {
            if (obj == null) return false;

            m_noReceiveCount--;
            if (this.m_Pool.Count >= m_maxCount && m_maxCount > 0)
            {
                obj = null;
                return false;
            }
            m_UsingPool.Remove(obj);
            this.m_Pool.Push(obj);
            return true;
        }


        public void RecycleAll()
        {
            for (int i = m_UsingPool.Count - 1; i >= 0; i--)
            {
                this.Recycle(m_UsingPool[i]);
            }
        }

    }
}
