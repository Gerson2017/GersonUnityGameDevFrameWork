


using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 热更程序域命名空间
/// </summary>
public class DomainName : ScriptableObject
{
    public List<DomainInfo> domainInfos = new List<DomainInfo>();
}


public enum DomainType
{
    MagicLand,
    AlienMaze,
    UI,
    MsgCenter,
    FightHot,
}

[System.Serializable]//序列化结构体 字典不能使用该特性进行序列化
public struct DomainInfo
{
    public string ID;
    public DomainType domainType;
    public string Name;
}