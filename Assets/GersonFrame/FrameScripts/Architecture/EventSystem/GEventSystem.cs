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



    public class GEventSystem
    {

        private static EventSystemIOC m_eventIoc = new EventSystemIOC();

        public static void Register(Action callback) 
        {
            m_eventIoc.GetOrAddEvent<EasyEvent>().Register(callback);
        }


        public static void Register<T>(Action<T> callback) where T:IEventType,new()
        {
            m_eventIoc.GetOrAddEvent<EasyEvent<T>>().Register(callback);
        }

    }



    public class EasyEvent: IEventType
    {
        private Action mOnEvent =()=> { };

        public EventType mEventType => EventType.Normal;

        public void Register(Action onEvent)
        {
            mOnEvent += onEvent;
        }

        public void UnRegister(Action onEvent)
        {
            mOnEvent -= onEvent;
        }

        public void Trigger()
        {
            mOnEvent?.Invoke();
        }
    }



    public class EasyEvent<T> :IEventType
    {
        private Action<T> mOnEvent = e => { };

        public EventType mEventType => EventType.Normal;

        public void Register(Action<T> onEvent)
        {
            mOnEvent += onEvent;
        }

        public void UnRegister(Action<T> onEvent)
        {
            mOnEvent -= onEvent;
        }

        public void Trigger(T t)
        {
            mOnEvent?.Invoke(t);
        }
    }


    public class EasyEvent<T,K> 
    {
        private Action<T, K> mOnEvent =(t,k)=> { };

        public void Register(Action<T, K> onEvent)
        {
            mOnEvent += onEvent;
        }

        public void UnRegister(Action<T, K> onEvent)
        {
            mOnEvent -= onEvent;
        }

        public void Trigger(T t,K k)
        {
            mOnEvent?.Invoke(t,k);
        }
    }

    public class EasyEvent<T, K,S>
    {
        private Action<T, K,S> mOnEvent = (t, k,s) => { };

        public void Register(Action<T, K,S> onEvent)
        {
            mOnEvent += onEvent;
        }

        public void UnRegister(Action<T, K,S> onEvent)
        {
            mOnEvent -= onEvent;
        }

        public void Trigger(T t, K k,S s)
        {
            mOnEvent?.Invoke(t, k,s);
        }
    }


    public class EventSystemIOC
    {
        private Dictionary<Type, IEventType> m_typeEventDic= new Dictionary<Type, IEventType>();


        public void AddEvent<T>() where T : IEventType, new()
        {
            m_typeEventDic.Add(typeof(T), new T());
        }


        public void AddEvent<T>(T t) where T : IEventType, new()
        {
            m_typeEventDic.Add(typeof(T), t);
        }


        public T GetEvent<T>() where T : IEventType
        {
            IEventType e;

            if (m_typeEventDic.TryGetValue(typeof(T), out e))
                return (T)e;

            return default;
        }


        public bool RemoveEvent<T>() where T : IEventType
        {
            Type type = typeof(T);
            if (m_typeEventDic.ContainsKey(type))
            {
                m_typeEventDic.Remove(type);
                return true;
            }
            return false;
        }


        public T GetOrAddEvent<T>() where T : IEventType, new()
        {
            var eType = typeof(T);
            if (m_typeEventDic.TryGetValue(eType, out var e))
            {
                return (T)e;
            }

            var t = new T();
            m_typeEventDic.Add(eType, t);
            return t;
        }



        public void RemoveAllEvent()
        {
            m_typeEventDic.Clear();
        }

    }


}


