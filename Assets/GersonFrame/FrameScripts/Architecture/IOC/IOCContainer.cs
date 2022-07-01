using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GersonFrame
{

    public class IOCContainer
    {
        /// <summary>
        /// 实例
        /// </summary>
        public Dictionary<Type, object> mInstances = new Dictionary<Type, object>();

        /// <summary>
        /// 注册实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        public void Register<T>(T instance)
        {
            var key = typeof(T);
            if (mInstances.ContainsKey(key))
                mInstances[key] = instance;
            else
                mInstances.Add(key, instance);
        }


        /// <summary>
        /// 获取
        /// </summary>
        public T Get<T>() where T:class
        {
            var key = typeof(T);
            object retObj;
            if (mInstances.TryGetValue(key, out retObj))
                return retObj as T;
            else
                return null;
        }


    }
}
