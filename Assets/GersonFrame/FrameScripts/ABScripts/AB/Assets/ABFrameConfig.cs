using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GersonFrame.ABFrame
{
    public class ABFrameConfig : ScriptableObject
    {
        [Header("AB配置信息路径")]
        public string AbconfigPath = "";
        [Header("AB资源XML文本路径")]
        public string AssetbundleXMLConfigPath = "";
        [Header("AB资源Bytes文本路径")]
        public string AssetbundleBytesConfigPath = "";
        [Header("针对IL2CPP模式下的TimeLine找不到问题 预留预制体路径")]
        public string TimeLinePrefabPath = "";
    }



    public class ABFrameConfigGeter
    {

#if UNITY_EDITOR
        private const string ABFrameConfigPath = "Assets/GersonFrame/ABFrame/Data/ABFrameConfig.asset";
        /// <summary>
        /// 获取框架配置信息
        /// </summary>
        public static ABFrameConfig Config
        {
            get
            {
                return UnityEditor.AssetDatabase.LoadAssetAtPath<ABFrameConfig>(ABFrameConfigPath);
            }
        }

#endif

    }

}

