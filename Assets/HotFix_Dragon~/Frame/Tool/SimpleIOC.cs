/*

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HotGersonFrame.Tool
{


    public class SimpleIOCInjectAttribute : Attribute
    {



        public SimpleIOCInjectAttribute(string name)
        {

        }


    }

    public interface ISimpleIOC
    {

        /// <summary>
        /// 注册类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void Register<T>();

        /// <summary>
        /// 注册为单例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        void RegisterInstance<T>(T instance);

        /// <summary>
        /// 注册依赖 每次使用Resolve获取的都是不一样的对象
        /// </summary>
        /// <typeparam name="TBase"></typeparam>
        /// <typeparam name="TConcrete"></typeparam>
        void Register<TBase, TConcrete>() where TConcrete : TBase;


        /// <summary>
        /// 获取实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Resolve<T>();

        /// <summary>
        /// 注入
        /// </summary>
        /// <param name="obj"></param>
        void Inject(object obj);

    }


    public class SimpleIOC : ISimpleIOC
    {

        /// <summary>
        /// 注册的类型
        /// </summary>
        HashSet<Type> mRegisteredTtype = new HashSet<Type>();

        /// <summary>
        /// 注册的对象
        /// </summary>
        Dictionary<Type, object> mRegisterInstances = new Dictionary<Type, object>();

        /// <summary>
        /// 注册的依赖关系
        /// </summary>
        Dictionary<Type, Type> mDependencies = new Dictionary<Type, Type>();

        public void Register<T>()
        {
            mRegisteredTtype.Add(typeof(T));
        }

        public void Register<TBase, TConcrete>() where TConcrete : TBase
        {
            mDependencies[typeof(TBase)] = typeof(TConcrete);
        }

        public void RegisterInstance<T>(T instance)
        {
            var type = typeof(T);
            mRegisterInstances[type] = instance;
        }

        public T Resolve<T>()
        {
            var type = typeof(T);

            if (mRegisterInstances.ContainsKey(type))
                return (T)mRegisterInstances[type];

            //判断是否有依赖关系
            if (mDependencies.ContainsKey(type))
                return (T)Activator.CreateInstance(mDependencies[type]);

            if (mRegisteredTtype.Contains(type))
                return Activator.CreateInstance<T>();

            return default;
        }


        /// <summary>
        /// 根据实例对象的类型获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public T Resolve<T>(T instance)
        {
            return Resolve<T>();
        }


        /// <summary>
        /// 注入被标签标记的对象内的值
        /// </summary>
        /// <param name="obj"></param>
        public void Inject(object obj)
        {
            PropertyInfo[] infos = obj.GetType().GetProperties();
            IEnumerable<PropertyInfo> properitys = infos.Where(p => p.GetCustomAttributes(typeof(SimpleIOCInjectAttribute)).Any());

            foreach (var properity in properitys)
            {
                var instance = Resolve(properity.PropertyType);
                if (instance != null)
                    properity.SetValue(obj, instance);
                else
                    MyDebuger.LogErrorFormat("can  not found propertyType {0}", properity.PropertyType);
            }

        }

    }



}
*/