using GersonFrame.Tool;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GersonFrame
{
    public interface IPool<T>
    {
        T Allocate();
        bool Recycle(T obj);
    }

    public interface IObjectFactory<T>
    {
        T Create();
    }

    public abstract class Pool<T> : IPool<T>
    {
        public Stack<T> mCacheStack = new Stack<T>();
        /// <summary>
        /// 正在使用的池子
        /// </summary>
        public List<T> mUsingStack = new List<T>();

        protected IObjectFactory<T> mFactory;

        /// <summary>
        /// Gets the current count.
        /// </summary>
        /// <value>The current count.</value>
        public int CatchCount
        {
            get { return mCacheStack.Count; }
        }

        /// <summary>
        /// 正在使用的池子
        /// </summary>
        public int UsingCount
        {
            get { return mUsingStack.Count; }
        }


        public T Allocate()
        {
            T obj = mCacheStack.Count > 0 ? mCacheStack.Pop() : mFactory.Create();
            this.mUsingStack.Add(obj);
            return obj;
        }

        public abstract bool Recycle(T obj);

        public abstract void RecycleAll();

    }

    public class CustomObjectFactroy<T> : IObjectFactory<T>
    {
        private Func<T> mFactroyMethod;

        public CustomObjectFactroy(Func<T> factroyMethod)
        {
            mFactroyMethod = factroyMethod;
        }

        public T Create()
        {
            return mFactroyMethod();
        }
    }

    public class SimpleObjectPool<T> : Pool<T>
    {
        Action<T> mResetMethod=null;
        Action<T> mDestoryMethod=null;

        private bool m_destoring = false;

        /// <summary>
        /// 是否正在销毁资源
        /// </summary>
        public bool Destoring
        {
            get { return this.m_destoring; }
        }

        public SimpleObjectPool(Func<T> factroyMethod, Action<T> resetMethod = null, int initCount = 0)
        {
            mFactory = new CustomObjectFactroy<T>(factroyMethod);
            mResetMethod = resetMethod;
            for (var i = 0; i < initCount; i++)
            {
                mCacheStack.Push(mFactory.Create());
            }
        }

        public override bool Recycle(T obj)
        {
            if (mUsingStack.Contains(obj))
                mUsingStack.Remove(obj);
            if (mCacheStack.Contains(obj))
                return false;
            mResetMethod?.Invoke(obj);
            mCacheStack.Push(obj);
            return true;
        }

        /// <summary>
        /// 回收所有
        /// </summary>
        public override void RecycleAll()
        {
            int maxrecycle = 30;
            int count = 0;
            for (int i = 0; i < this.mUsingStack.Count; i++)
            {
                T obj = this.mUsingStack[i];
                mResetMethod?.Invoke(obj);
                mCacheStack.Push(obj);
                count++;
                if (count >= maxrecycle)
                {
                    this.mUsingStack.RemoveRange(0, maxrecycle + 1);
                    TimeInvoke.Instance.AddFrameTask(this.RecycleAll, 1);
                    return;
                }
            }
            this.mUsingStack.Clear();
        }

        /// <summary>
        /// 销毁资源池的内容
        /// </summary>
        /// <returns></returns>
        public void DestoryPoolAssets()
        {
            if (this.mDestoryMethod==null)
            {
                MyDebuger.LogError("mDestoryMethod is null can not destory assets");
                return;
            }
            this.m_destoring = true;
            int maxrecycle = 30;
            int count = 0;
            for (int i = 0; i < this.mUsingStack.Count; i++)
            {
                T obj = this.mUsingStack[i];
                mDestoryMethod?.Invoke(obj);
                count++;
                if (count >= maxrecycle)
                {
                    TimeInvoke.Instance.AddFrameTask(this.DestoryPoolAssets, 1);
                    return;
                }
            }
            this.mUsingStack.Clear();

            for (int i = 0; i < mCacheStack.Count; i++)
            {
                T obj = this.mCacheStack.Pop();
                mDestoryMethod?.Invoke(obj);
                count++;
                if (count >= maxrecycle)
                {
                    TimeInvoke.Instance.AddFrameTask(this.DestoryPoolAssets, 1);
                    return;
                }
            }
            this.m_destoring = false;
            mCacheStack.Clear();
        }
    }
}