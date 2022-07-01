using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAm : MonoBehaviour
{

    /// <summary>
    /// 旋转轴
    /// </summary>
    public Vector3 m_RoateAxisl;

    /// <summary>
    /// 旋转sud
    /// </summary>
    public float m_RotateSpeed;



    // Update is called once per frame
    void Update()
    {
        transform.Rotate(this.m_RoateAxisl*this.m_RotateSpeed*Time.deltaTime);
    }
}
