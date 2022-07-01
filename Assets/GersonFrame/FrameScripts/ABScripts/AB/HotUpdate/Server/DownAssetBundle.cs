using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace GersonFrame.ABFrame
{

    public class DownLoadAssetBundle : DownLoadItem
    {

        UnityWebRequest m_webRequest;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="savePath">下载路径 不包含文件名</param>
        public DownLoadAssetBundle(string url, string savePath) : base(url, savePath)
        {
        }


        public override IEnumerator DownLoad(Action callback = null)
        {
            m_webRequest = UnityWebRequest.Get(Url+"?"+DateTime.Now.Ticks);
            StartDownLoad = true;
            m_webRequest.timeout = 30;
            yield return m_webRequest.SendWebRequest();
            StartDownLoad = false;
            if (m_webRequest.result == UnityWebRequest.Result.ConnectionError)
                Debug.LogError("DownLoad Asset " + this.FileName + " Error:" + m_webRequest.error);
            else
            {
                byte[] bytes = m_webRequest.downloadHandler.data;
                FileTool.CreateFile(SaveFilePath, bytes);
                callback?.Invoke();
                MyDebuger.Log("DownLoad Asset Success " + this.FileName);
            }

        }
        public override void Destory()
        {
            if (m_webRequest != null)
            {
                m_webRequest.Dispose();
                m_webRequest = null;
            }
        }

        public override long GetCurLength()
        {
            if (m_webRequest != null)
            {
                return (long)m_webRequest.downloadedBytes;
            }
            return 0;
        }

        public override long GetLength()
        {
            return 0;
        }

        public override float GetProgress()
        {
            if (m_webRequest != null)
            {
                return m_webRequest.downloadProgress;
            }
            return 0;
        }
    }
}
