
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace GersonFrame.ABFrame
{



    [System.Serializable]
    public class ABMD5
    {
        [XmlElement]
        public List<ABMD5Base> ABMD5List { get; set; }
    }


    [System.Serializable]
    public class ABMD5Base
    {
        [XmlAttribute("Name")]
        /// <summary>
        /// AB名字
        /// </summary>
        public string Name { get; set; }

        [XmlAttribute("Md5")]
        ///AB对应的Md5码
        public string Md5 { get; set; }

        [XmlAttribute("Size")]
        /// <summary>
        /// 文件大小
        /// </summary>
        public float Size { get; set; }



    }



}