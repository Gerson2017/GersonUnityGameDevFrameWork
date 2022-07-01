using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace GersonFrame
{


    public interface IArchitecture
    {

        /// <summary>
        /// 注册系统
        /// </summary>
        void RegisterSystem<T>(T instance) where T : ISystem; 

        /// <summary>
        /// 注册 Model
        /// </summary>
        void RegisterModel<T>(T instance) where T : IModel;

        /// <summary>
        /// 注册 Utility
        /// </summary>
        void RegisterUtility<T>(T instance);

        /// <summary>
        /// 获取 System
        /// </summary>
        T GetSystem<T>() where T : class, ISystem; 


        /// <summary>
        /// 获取 Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetModel<T>() where T : class, IModel;

        /// <summary>
        /// 获取工具
        /// </summary>
        T GetUtility<T>() where T : class;


        /// <summary>
        /// 发送命令
        /// </summary>
        void SendCommand<T>(object arg1,object arg2,object arg3) where T : ICommand, new();

        /// <summary>
        /// 发送命令
        /// </summary>
        void SendCommand<T>(T command, object arg1, object arg2, object arg3) where T : ICommand;

        /// <summary>
        /// 发送事件
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
         void SendMsg(string msgType, object arg1 = null, object arg2 = null, object arg3 = null);

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
         void RegisterMsg(string msgType, System.Action<object, object, object> onMsgReceived);

        /// <summary>
        /// 卸载指定消息的 某个监听
        /// </summary>
         void UnRegisterMsg(string msgType, System.Action<object, object, object> onMsgReceived);

        /// <summary>
        /// 卸载指定消息的所有监听
        /// </summary>
        /// <param name="msgType"></param>
         void UnRegisterMsg(string msgType);

        /// <summary>
        /// 卸载所有监听
        /// </summary>
         void UnRegisterAll();


    }



    /// <summary>
    /// 架构
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Architecture<T> : IArchitecture where T : Architecture<T>, new()
    {
        /// <summary>
        /// 是否已经初始化完成
        /// </summary>
        private bool mInited = false;


        /// <summary>
        /// 用于初始化的 Systems 的缓存
        /// </summary>
        private List<ISystem> mSystems = new List<ISystem>(); // 新增                                           
        public void RegisterSystem<TSystem>(TSystem instance) where TSystem : ISystem // 新增
        {
            // 需要给 Model 赋值一下
            instance.SetArchitecture(this);
            mContainer.Register(instance);

            // 如果初始化过了
            if (mInited)
            {
                instance.Init();
            }
            else
            {
                // 添加到 Model 缓存中，用于初始化
                mSystems.Add(instance);
            }
        }

        /// <summary>
        /// 用于初始化的 Models 的缓存
        /// </summary>
        private List<IModel> mModels = new List<IModel>();


     // 提供一个注册 Model 的 API
        public void RegisterModel<TModel>(TModel instance) where TModel : IModel
        {
            // 需要给 Model 赋值一下
            instance.SetArchitecture(this);
            mContainer.Register(instance);
            // 如果初始化过了
            if (mInited)
                instance.Init();
            else
                // 添加到 Model 缓存中，用于初始化
                mModels.Add(instance);
        }

        #region 类似单例模式 但是仅在内部课访问

        private static T mArchitecture = null;

        public static IArchitecture Interface
        {
            get
            {
                if (mArchitecture == null)
                    MakeSureArchitecture();
                return mArchitecture;
            }
        }


        // 确保 Container 是有实例的
        static void MakeSureArchitecture()
        {
            if (mArchitecture == null)
            {
                mArchitecture = new T();
                mArchitecture.Init();

                // 初始化 Model 先初始化 确保system可以获取到model
                foreach (var architectureModel in mArchitecture.mModels)
                    architectureModel.Init();
                mArchitecture.mModels.Clear();
                // 初始化 System
                foreach (var system in mArchitecture.mSystems)
                    system.Init();
                mArchitecture.mSystems.Clear();

                mArchitecture.mInited = true;
            }
        }

        #endregion

        private IOCContainer mContainer = new IOCContainer();

        // 留给子类注册模块
        protected abstract void Init();


        public void RegisterUtility<TUtility>(TUtility instance)
        {
            mContainer.Register(instance);
        }

        public TUtility GetUtility<TUtility>() where TUtility : class
        {
            return mContainer.Get<TUtility>();
        }
        public TModel GetModel<TModel>() where TModel : class, IModel
        {
            return mContainer.Get<TModel>();
        }

        public TSystem GetSystem<TSystem>() where TSystem : class, ISystem
        {
            return mContainer.Get<TSystem>();
        }


        public void SendCommand<TCommand>(object arg1=null, object arg2=null, object arg3=null) where TCommand : ICommand, new()
        {
            var command = new TCommand();
            command.SetArchitecture(this);
            command.Execute(arg1,arg2,arg3);
        }

        public void SendCommand<TCommand>(TCommand command, object arg1 = null, object arg2 = null, object arg3 = null) where TCommand : ICommand
        {
            command.Execute(arg1, arg2, arg3);
        }

        //=====================Msg================

        protected BaseInternalMsg m_msg = new BaseInternalMsg();

        public void SendMsg(string msgType, object arg1 = null, object arg2 = null, object arg3 = null)
        {
            m_msg.SendMsg(msgType, arg1, arg2, arg3);
        }

        public void RegisterMsg(string msgType,System.Action<object, object, object> onMsgReceived)
        {
            m_msg.RegisterMsg(msgType,onMsgReceived);
        }

        /// <summary>
        /// 卸载指定消息的 某个监听
        /// </summary>
        public void UnRegisterMsg(string msgType,System.Action<object, object, object> onMsgReceived)
        {
            m_msg.UnRegisterMsg(msgType, onMsgReceived);
        }

        /// <summary>
        /// 卸载指定消息的所有监听
        /// </summary>
        /// <param name="msgType"></param>
        public void UnRegisterMsg(string msgType)
        {
            m_msg.UnRegisterMsg(msgType);
        }

        /// <summary>
        /// 卸载所有监听
        /// </summary>
        public void UnRegisterAll()
        {
            m_msg.UnRegisterAll();
        }
    }

}