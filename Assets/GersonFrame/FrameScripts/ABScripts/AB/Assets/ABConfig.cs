using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GersonFrame.ABFrame
{

    public  enum ABAssetType
    {
        Texture,
        Audios,
        Shader,
        ConfigFiles,
        Other,
    }


   [CreateAssetMenu(fileName = "ABConfig", menuName = "CreateABConfig", order =1000)]
    public class ABConfig : ScriptableObject
    {

        public AssetLoadModel m_AssetsLoadeModel = AssetLoadModel.LoadFromEditor;

        /// <summary>
        /// 单个文件所在文件夹路径，会遍历这个文件夹下的所有文件信息,所有文件名字不能重复,必须保证名字的唯一性
        /// </summary>
        [Header("预制体所在文件夹路径")]
        public List<string> m_PrefabsFilePath = new List<string>();

        /// <summary>
        /// 所有文件夹AB包
        /// </summary>
        [Header("配置文件，音效文件，材质贴图动画等等")]
        public List<FileDirABName> m_AllFileDirAB = new List<FileDirABName>();

        [System.Serializable]//序列化结构体 字典不能使用该特性进行序列化
        public struct FileDirABName
        {
            public string ABName;
            public string Path;
            public ABAssetType AssetType;
        }



    }

}
