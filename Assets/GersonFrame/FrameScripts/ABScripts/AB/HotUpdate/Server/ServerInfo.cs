using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

namespace GersonFrame.ABFrame
{


    [System.Serializable]
    /// <summary>
    /// 本地打包时生成的热更版本信息文件
    /// </summary>
    public class HotUpdateVersionInfo
    {
        public string Version;
        public string PackageName;
        public int BundleVersionCode;
    }



    [System.Serializable]
    public class ServerInfo
    {
        /// <summary>
        ///所有版本信息
        /// </summary>
        [XmlElement("GameVersion")]
        public GameVersion GameVersion;
    }

    [System.Serializable]
    /// <summary>
    /// 当前游戏版本对应的所有补丁
    /// </summary>
    public class GameVersion
    {
        /// <summary>
        /// 版本号
        /// </summary>
        [XmlAttribute]
        public string Version;

        /// <summary>
        /// 所有补丁包
        /// </summary>
        [XmlElement]
        public Pathces[] Pathces;

    }





    [System.Serializable]
    /// <summary>
    /// 一个总补丁包 多个AB文件
    /// </summary>
    public class Pathces
    {
        /// <summary>
        /// 当前热更的版本号(当前第几次热更)
        /// </summary>
        [XmlAttribute]
        public string Version;

        /// <summary>
        /// 热更描述
        /// </summary>
        [XmlAttribute]
        public string Des;

        /// <summary>
        /// 所有的热更文件 包含Ab文件
        /// </summary>
        [XmlElement]
        public List<Patch> Files;


    }



    /// <summary>
    /// 单个补丁包 AB文件
    /// </summary>
    [System.Serializable]
    public class Patch
    {
        /// <summary>
        /// 名字
        /// </summary>
        [XmlAttribute]
        public string Name;

        /// <summary>
        /// 热更下载地址
        /// </summary>
        [XmlAttribute]
        public string Url;

        /// <summary>
        /// 当前包的平台
        /// </summary>
        [XmlAttribute]
        public string Platform;

        [XmlAttribute]
        public string MD5;

        /// <summary>
        /// 热更文件大小
        /// </summary>
        [XmlAttribute]
        public float Size;

        /// <summary>
        /// Version
        /// </summary>
        [XmlAttribute]
        public string Version;

    }
}