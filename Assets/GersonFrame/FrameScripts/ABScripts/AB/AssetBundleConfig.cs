using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

namespace GersonFrame.ABFrame
{


[System.Serializable]
public class AssetBundleConfig 
{
    [XmlElement("ABList")]
    public List<ABBase> ABList = new List<ABBase>();
   
}

[System.Serializable]
public class ABBase
{
    /// <summary>
    /// 全路径
    /// </summary>
    [XmlAttribute("Path")]
    public string Path{get;set;}
    /// <summary>
    /// Crc 标识 类似MD5码 精确度小于MD5
    /// </summary>
    [XmlAttribute("Crc")]
    public uint Crc { get; set; }
    [XmlAttribute("ABName")]
    /// <summary>
    /// 资源所在的包名  
    /// </summary>
    public string ABName { get; set; }
    /// <summary>
    /// 资源名 一个包里可能存在多个资源
    /// </summary>
    [XmlAttribute("AssetName")]
    public string AssetName { get; set; }
    /// <summary>
    /// 依赖的Ab资源包
    /// </summary>
    [XmlElement("ABDependences")]
    public List<string> ABDependences { get; set; }


}

}
