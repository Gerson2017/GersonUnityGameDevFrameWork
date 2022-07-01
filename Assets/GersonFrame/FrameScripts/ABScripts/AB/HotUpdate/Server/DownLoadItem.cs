using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GersonFrame.ABFrame
{

    public abstract class DownLoadItem
    {
        /// <summary>
        /// 资源路径
        /// </summary>
        public string Url { get; protected set; }

        /// <summary>
        /// 存储路径 不包含文件名
        /// </summary>
        public string SavePath { get; protected set; }

        /// <summary>
        /// 文件名 不包含后缀
        /// </summary>
        public string FileNameNoExt { get; protected set; }
        /// <summary>
        /// 文件后缀
        /// </summary>
        public string FileExt { get; protected set; }

        /// <summary>
        /// 文件名 包含后缀
        /// </summary>
        public string FileName { get; protected set; }

        /// <summary>
        /// 存储全路径 路径+文件+后缀
        /// </summary>
        public string SaveFilePath { get; protected set; }

        /// <summary>
        /// 原文件大小
        /// </summary>
        public long FileLengh { get; protected set; }

        /// <summary>
        /// 当前下载的大小
        /// </summary>
        public long CurLength { get; protected set; }


        /// <summary>
        /// 是否开始下载
        /// </summary>
        public bool StartDownLoad { get; protected set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="savePath">存储路径 不包含文件名</param>
        public DownLoadItem(string url, string savePath)
        {
            Url = url;
            SavePath = savePath;
            StartDownLoad = false;
            //获取不包含后缀的文件名
            FileNameNoExt = Path.GetFileNameWithoutExtension(url);
            //获取文件后缀
            FileExt = Path.GetExtension(Url);
            FileName = string.Format("{0}{1}", FileNameNoExt, FileExt);
            SaveFilePath = string.Format("{0}/{1}{2}", SavePath, FileNameNoExt, FileExt);
        }



        /// <summary>
        /// 资源下载协程
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public virtual IEnumerator DownLoad(Action callback)
        {
            yield return null;
        }

        /// <summary>
        /// 获取下载进度
        /// </summary>
        /// <returns></returns>
        public abstract float GetProgress();

        /// <summary>
        /// 获取当前已经下载了多少
        /// </summary>
        /// <returns></returns>
        public abstract long GetCurLength();

        /// <summary>
        /// 获取当前下载文件的大小
        /// </summary>
        /// <returns></returns>
        public abstract long GetLength();

        /// <summary>
        /// 销毁当前对象
        /// </summary>
        public abstract void Destory();

    }
}
