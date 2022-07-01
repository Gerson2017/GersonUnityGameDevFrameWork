using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class  GameObjectExtension  
{
   public static void Show(this GameObject go)
    {
        if (go == null) return;
        if (go.activeSelf) return;
            go?.SetActive(true);
    }

    public static void Hide(this GameObject go)
    {
        if (go == null) return;
        if (!go.activeSelf) return;
        go?.SetActive(false);
    }


    /// <summary>
    /// 设置父物体
    /// </summary>
    /// <param name="go"></param>
    /// <param name="parent"></param>
    public static void SetParent(this GameObject go,GameObject parent)
    {
        go?.transform.SetParent(parent?.transform);
    }


    /// <summary>
    /// 设置父物体
    /// </summary>
    /// <param name="go"></param>
    /// <param name="parent"></param>
    public static void SetParent(this GameObject go,Transform parent)
    {
        go?.transform.SetParent(parent);
    }


    /// <summary>
    /// 设置父物体
    /// </summary>
    /// <param name="go"></param>
    /// <param name="parent"></param>
    public static void SetLocalPos(this GameObject go,Vector3 localpos)
    {
        go.transform.localPosition = localpos;
    }


    public static T GetCompententOrNew<T>(this GameObject go) where T:MonoBehaviour
    {
        T t = go.GetComponent<T>();
        if (t == null) t = go.AddComponent<T>();
        return t;
    }

    
}
