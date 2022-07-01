using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GersonFrame;

public class ColliderCallBackTool : MonoBehaviourSimplify
{

    public event System.Action<Collider> m_TriggerEnterCallBack;
    public event System.Action<Collider> m_TriggerExitCallBack;
    public event System.Action<Collision> m_CollisionEnterCallBack;
    public event System.Action<Collision> m_CollisionExitCallBack;



    private void OnTriggerEnter(Collider other)
    {
        m_TriggerEnterCallBack?.Invoke(other);
    }


    private void OnTriggerExit(Collider other)
    {
        m_TriggerExitCallBack?.Invoke(other);
    }


    private void OnCollisionEnter(Collision collision)
    {
        m_CollisionEnterCallBack?.Invoke(collision);
    }


    private void OnCollisionExit(Collision collision)
    {
        m_CollisionExitCallBack?.Invoke(collision);
    }

    protected override void OnBeforeDestroy()
    {

    }
}
