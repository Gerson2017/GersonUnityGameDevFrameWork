using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///不会被消销毁的Mono单例
/// </summary>
/// <typeparam name="T"></typeparam>
public class NoDestoryMonoSingleton <T>: MonoBehaviour where T:NoDestoryMonoSingleton<T>
{

    public static T mInstance { get; private set; }

    private void Awake()
    {
        if (mInstance==null)
        {
            mInstance = this as T;
            GameObject.DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
