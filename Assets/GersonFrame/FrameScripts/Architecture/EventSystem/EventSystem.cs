using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GersonFrame
{

    public enum EventType
    {
        Normal,
    }


    public interface IEventType
    {
        EventType mEventType { get;}
    }




    public class EventSystem
    {
        private static Dictionary<Type, IEventType> mTypeEventDic = new Dictionary<Type, IEventType>();

        public static T Get<T>() where T : IEventType
        {
            return mGlobalEvents.GetEvent<T>();
        }


        public static void Register<T>() where T : IEventType, new()
        {
            mGlobalEvents.AddEvent<T>();
        }

        private Dictionary<Type, IEventType> mTypeEvents = new Dictionary<Type, IEventType>();

        public void AddEvent<T>() where T : IEventType, new()
        {
            mTypeEvents.Add(typeof(T), new T());
        }

        public T GetEvent<T>() where T : IEasyEvent
        {
            IEasyEvent e;

            if (mTypeEvents.TryGetValue(typeof(T), out e))
            {
                return (T)e;
            }

            return default;
        }

        public T GetOrAddEvent<T>() where T : IEasyEvent, new()
        {
            var eType = typeof(T);
            if (mTypeEvents.TryGetValue(eType, out var e))
            {
                return (T)e;
            }

            var t = new T();
            mTypeEvents.Add(eType, t);
            return t;
        }

    }




}


