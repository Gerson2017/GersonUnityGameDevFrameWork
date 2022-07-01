using GersonFrame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HotGersonFrame
{


    public interface IHotArchitecture
    {

        /// <summary>
        /// 注册系统
        /// </summary>
        void RegisterSystem<T>(T instance) where T : IHotSystem;

        /// <summary>
        /// 注册 Model
        /// </summary>
        void RegisterModel<T>(T instance) where T : IHotModel;

        /// <summary>
        /// 注册 Utility
        /// </summary>
        void RegisterUtility<T>(T instance) where T : IHotUtility;

        /// <summary>
        /// 获取 System
        /// </summary>
        T GetSystem<T>() where T : class, IHotSystem;


        /// <summary>
        /// 获取 Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetModel<T>() where T : class, IHotModel;

        /// <summary>
        /// 获取工具
        /// </summary>
        T GetUtility<T>() where T : class, IHotUtility;


        /// <summary>
        /// 发送命令
        /// </summary>
        void SendCommand<T>(object arg1, object arg2, object arg3) where T : IHotCommand, new();

        /// <summary>
        /// 发送命令
        /// </summary>
        void SendCommand<T>(T command, object arg1, object arg2, object arg3) where T : IHotCommand;

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
        /// 注册事件
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        void RegisterMsg(int msgtype, string msgname, System.Action<object, object, object> onMsgReceived);


        /// 卸载指定消息的 某个监听
        /// </summary>
        void UnRegisterMsg(int msgtype, string msgname, System.Action<object, object, object> onMsgReceived);

        /// <summary>
        /// 卸载指定消息的 某个监听
        /// </summary>
        void UnRegisterMsg(string msgType, System.Action<object, object, object> onMsgReceived);


        /// <summary>
        /// 卸载指定消息的所有监听
        /// </summary>
        /// <param name="msgType"></param>
        void UnRegisterMsg(string msgName);


        /// <summary>
        /// 卸载指定消息的所有监听
        /// </summary>
        /// <param name="msgType"></param>
        void UnRegisterMsg(int msgType);


        /// <summary>
        /// 卸载指定消息的所有监听
        /// </summary>
        /// <param name="msgType"></param>
        void UnRegisterMsg(int msgType,string msgName);

        /// <summary>
        /// 卸载所有监听
        /// </summary>
        void UnRegisterAll();


    }



    /// <summary>
    /// 架构
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class HotAbArchitecture : IHotArchitecture
    {

        #region 构造方法中初始化各模块

        protected HotAbArchitecture()
        {
            //在init中注册各模块
            Init();
            MakeSureArchitecture();
        }

        // 确保 Container 是有实例的
        void MakeSureArchitecture()
        {
            // 初始化 Model 先初始化 确保system可以获取到model
            foreach (var architectureModel in this.mModels)
                architectureModel.InitModel();
            this.mModels.Clear();
            // 初始化 System
            foreach (var system in this.mSystems)
                system.Init();
            this.mSystems.Clear();
            this.mInited = true;
        }

        #endregion

        /// <summary>
        /// 是否已经初始化完成
        /// </summary>
        private bool mInited = false;
        /// <summary>
        /// 模块容器
        /// </summary>
        private HotIOCContainer mContainer = new HotIOCContainer();

        /// <summary>
        /// 用于初始化的 Systems 的缓存
        /// </summary>
        private List<IHotSystem> mSystems = new List<IHotSystem>();
        public void RegisterSystem<T>(T instance) where T : IHotSystem // 新增
        {
            // 需要给 Model 赋值一下
            instance.SetArchitecture(this);
            mContainer.Register<T>(instance);
            // 如果初始化过了
            if (mInited)
                instance.Init();
            else
                // 添加到 Model 缓存中，用于初始化
                mSystems.Add(instance);
        }

        /// <summary>
        /// 用于初始化的 Models 的缓存
        /// </summary>
        private List<IHotModel> mModels = new List<IHotModel>();

        // 提供一个注册 Model 的 API
        public void RegisterModel<T>(T instance) where T : IHotModel
        {
            // 需要给 Model 赋值一下
            instance.SetArchitecture(this);
            mContainer.Register<T>(instance);
            // 如果初始化过了
            if (mInited)
                instance.InitModel();
            else
                // 添加到 Model 缓存中，用于初始化
                mModels.Add(instance);
        }

        // 留给子类注册模块
        protected abstract void Init();

        public void RegisterUtility<T>(T instance) where T : IHotUtility
        {
            mContainer.Register<T>(instance);
        }

        public T GetUtility<T>() where T : class, IHotUtility
        {
            return mContainer.Get<T>();
        }

        public T GetModel<T>() where T : class, IHotModel
        {
            return mContainer.Get<T>();
        }

        public T GetSystem<T>() where T : class, IHotSystem
        {
            return mContainer.Get<T>();
        }

        public void SendCommand<T>(object arg1 = null, object arg2 = null, object arg3 = null) where T : IHotCommand, new()
        {
            var command = new T();
            command.SetArchitecture(this);
            command.Execute(arg1, arg2, arg3);
        }

        public void SendCommand<T>(T command, object arg1 = null, object arg2 = null, object arg3 = null) where T : IHotCommand
        {
            command.Execute(arg1, arg2, arg3);
        }

        //=====================Msg================

        protected BaseInternalMsg m_msg = new BaseInternalMsg();

        public void SendMsg(string msgType, object arg1 = null, object arg2 = null, object arg3 = null)
        {
            m_msg.SendMsg(msgType, arg1, arg2, arg3);
        }

        public void RegisterMsg(string msgType, System.Action<object, object, object> onMsgReceived)
        {
            m_msg.RegisterMsg(msgType, onMsgReceived);
        }

        /// <summary>
        /// 卸载指定消息的 某个监听
        /// </summary>
        public void UnRegisterMsg(string msgType, System.Action<object, object, object> onMsgReceived)
        {
            m_msg.UnRegisterMsg(msgType, onMsgReceived);
        }

        /// <summary>
        /// 卸载指定消息的所有监听
        /// </summary>
        /// <param name="msgType"></param>
        public void UnRegisterMsg(string msgName)
        {
            m_msg.UnRegisterMsg(msgName);
        }

        /// <summary>
        /// 卸载所有监听
        /// </summary>
        public void UnRegisterAll()
        {
            m_msg.UnRegisterAll();
        }

        public void RegisterMsg(int msgtype, string msgname, Action<object, object, object> onMsgReceived)
        {
            this.m_msg.RegisterMsg(msgtype,msgname,onMsgReceived);
        }

        public void UnRegisterMsg(int msgtype, string msgname, Action<object, object, object> onMsgReceived)
        {
            this.m_msg.UnRegisterMsg(msgtype, msgname, onMsgReceived);
        }

        public void UnRegisterMsg(int msgType)
        {
            this.m_msg.UnRegisterMsg(msgType);
        }

        public void UnRegisterMsg(int msgType, string msgName)
        {
            this.m_msg.UnRegisterMsg(msgType, msgName);
        }
    }

}