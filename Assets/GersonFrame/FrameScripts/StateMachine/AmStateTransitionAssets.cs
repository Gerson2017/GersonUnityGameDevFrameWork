using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AmStateTransitionAssets:ScriptableObject
{   




    [Header("状态集合")]
    public List<StateTransiton> States = new List<StateTransiton>();


    private Dictionary<string, StateTransiton> m_statetransitionDic;
    public Dictionary<string,StateTransiton> StateTransitionDIc
    {
        get
        {
            if (m_statetransitionDic == null)
            {
                m_statetransitionDic = new Dictionary<string, StateTransiton>();
                for (int i = 0; i < States.Count; i++)
                {
                    m_statetransitionDic[States[i].StateId] = States[i];
                }
            }
            
            return m_statetransitionDic;
        }
    }


}

[System.Serializable]
public struct StateTransiton
{
    public string StateId;

    public bool CanTranSitionAll;

    public List<string> CanTransitonStates;

}

