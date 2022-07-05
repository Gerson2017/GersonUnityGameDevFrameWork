using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GersonFrame
{


    public interface ITypeEvent
    {
    }

    public interface IUITypeEvent:ITypeEvent
    {

    }



    public class GEventSystem
    {

        private static EventSystemIOC m_eventIoc = new EventSystemIOC();

        public static void Register(Action callback) 
        {
            m_eventIoc.GetOrAddEvent<EasyEvent>().Register(callback);
        }


        public static void Register<T>(Action<T> callback) where T:ITypeEvent,new()
        {
            m_eventIoc.GetOrAddEvent<EasyEvent<T>>().Register(callback);
        }

    }



    public class EasyEvent: ITypeEvent
    {
        private Action mOnEvent =()=> { };


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



    public class EasyEvent<T> :ITypeEvent
    {
        private Action<T> mOnEvent = e => { };


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
        private Dictionary<Type, ITypeEvent> m_typeEventDic= new Dictionary<Type, ITypeEvent>();



        public T GetEvent<T>() where T : ITypeEvent
        {
            ITypeEvent e;

            if (m_typeEventDic.TryGetValue(typeof(T), out e))
                return (T)e;

            return default;
        }


        public bool RemoveEvent<T>() where T : ITypeEvent
        {
            Type type = typeof(T);
            if (m_typeEventDic.ContainsKey(type))
            {
                m_typeEventDic.Remove(type);
                return true;
            }
            return false;
        }


        public T GetOrAddEvent<T>() where T : ITypeEvent, new()
        {
            var eType = typeof(T);
            if (m_typeEventDic.TryGetValue(eType, out var e))
                return (T)e;

            var t = new T();
            m_typeEventDic.Add(eType, t);
            return t;
        }


        public void RemoveAllEvent()
        {
            m_typeEventDic.Clear();
        }



        public void RemoveEventByType<T>()
        {
            List<Type> removetypoes = new List<Type>();
            foreach (var item in m_typeEventDic)
            {
                if (item.Value is T)
                    removetypoes.Add(item.Key);
            }

            for (int i = 0; i < removetypoes.Count; i++)
            {
                Type t= removetypoes[i];
                if (m_typeEventDic.ContainsKey(t))
                    m_typeEventDic.Remove(t);
            }

        }


    }


}


