using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;



public static class TransformExtensiion
{
    public static void SetLocalPosx(this Transform transform, float x)
    {
        var localposition = transform.localPosition;
        localposition.x = x;
        transform.localPosition = localposition;
    }

    public static void SetLocalPosY(this Transform transform, float y)
    {
        var localposition = transform.localPosition;
        localposition.y = y;
        transform.localPosition = localposition;
    }

    public static void SetLocalPosZ(this Transform transform, float z)
    {
        var localposition = transform.localPosition;
        localposition.z = z;
        transform.localPosition = localposition;
    }

    public static void SetLocalPosXY(this Transform transform, float x, float y)
    {
        var localposition = transform.localPosition;
        localposition.x = x;
        localposition.y = y;
        transform.localPosition = localposition;
    }

    public static void SetLocalPosXZ(this Transform transform, float x, float z)
    {
        var localposition = transform.localPosition;
        localposition.x = x;
        localposition.z = z;
        transform.localPosition = localposition;
    }

    public static void SetLocalPosYZ(this Transform transform, float y, float z)
    {
        var localposition = transform.localPosition;
        localposition.y = y;
        localposition.z = z;
        transform.localPosition = localposition;
    }



    public static void IdentityLocalPosRos(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }


    public static void IdentityLocalRos(this Transform transform)
    {
        transform.localRotation = Quaternion.identity;
    }

    public static void IdentityWorldPosRos(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }


    public static void IdentityWorldRos(this Transform transform)
    {
        transform.rotation = Quaternion.identity;
    }

    public static void IdentityScal(this Transform transform)
    {
        transform.localScale = Vector3.one;
      
    }




    public static void Show(this Transform transform)
    {
        if (!transform.gameObject.activeSelf)
            transform.gameObject.SetActive(true);
    }

    public static void Hide(this Transform transform)
    {
        if (transform.gameObject.activeSelf)
            transform.gameObject.SetActive(false);
       
    }

    /// <summary>
    /// 添加子物体
    /// </summary>
    public static void AddChild(this Transform transform,Transform child)
    {
        child.SetParent(transform);
    }


    /// <summary>
    /// 获取指定距离下相机视口四个角的坐标
    /// </summary>
    /// <param name="cam"></param>
    /// <param name="distance">相对于相机的距离</param>
    /// <returns></returns>
    public static Vector3[] GetCameraFovPositionByDistance(this Camera cam, float distance)
    {
        Vector3[] corners = new Vector3[4];

        float halfFOV = (cam.fieldOfView * 0.5f) * Mathf.Deg2Rad;
        float aspect = cam.aspect;

        float height = distance * Mathf.Tan(halfFOV);
        float width = height * aspect;

        Transform tx = cam.transform;

        // 左上角
        corners[0] = tx.position - (tx.right * width);
        corners[0] += tx.up * height;
        corners[0] += tx.forward * distance;

        // 右上角
        corners[1] = tx.position + (tx.right * width);
        corners[1] += tx.up * height;
        corners[1] += tx.forward * distance;

        // 左下角
        corners[2] = tx.position - (tx.right * width);
        corners[2] -= tx.up * height;
        corners[2] += tx.forward * distance;

        // 右下角
        corners[3] = tx.position + (tx.right * width);
        corners[3] -= tx.up * height;
        corners[3] += tx.forward * distance;

        return corners;
    }

    /// <summary>
    ///*** 获取某对象从根节点到自身的路径
    /// </summary>
    public static string GetPath(this Transform _tra,Transform parent=null)
    {
        if (_tra == null)
        {
            Debug.Log("对象为空" + _tra.name);
            return null;
        }
        StringBuilder tempPath = new StringBuilder(_tra.name);
        Transform tempTra = _tra;
        string g = "/";
        while (tempTra.parent != null && tempTra.parent != parent)
        {
            tempTra = tempTra.parent;
            tempPath.Insert(0, tempTra.name + g);
        }
    // Debug.Log("路径: " + tempPath);
        return tempPath.ToString();
    }


    /// <summary>
    /// 隐藏子物体
    /// </summary>
    /// <param name="ts"></param>
    /// <param name="startHideNum"></param>
    public static void HideChild(this Transform ts,int startHideNum )
    {
        int count = ts.childCount;
        if (startHideNum < count)
        {
            for (int i = startHideNum ; i < count; i++)
            {
                if (i < 0) i = 0;
                ts.GetChild(i).gameObject.Hide();
            }
        }
    } 

}


