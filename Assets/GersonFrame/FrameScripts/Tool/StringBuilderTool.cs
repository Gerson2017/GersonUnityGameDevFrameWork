using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using GersonFrame.ABFrame;

namespace GersonFrame.Tool
{

public class MyStringBuilder
{
    private StringBuilder Builder;

   public MyStringBuilder()
    {
        Builder = new StringBuilder();
    }

    public void SetStrs(params string[] appendStr)
    {
        Builder.Clear();
        for (int i = 0; i < appendStr.Length; i++)
        {
            Builder.Append(appendStr[i]);
        }
    }

    public override string ToString()
    {
        return Builder.ToString();
    }

}

public class StringBuilderTool 
{
   private static ClassObjectPool<MyStringBuilder> builderPool = new ClassObjectPool<MyStringBuilder>(5);

    /// <summary>
    /// 正在使用的
    /// </summary>
    public static List<MyStringBuilder> m_usingBuilderList = new List<MyStringBuilder>();

    public static MyStringBuilder GetStringBuilder(params string[] appendStr )
    {
        MyStringBuilder builder = builderPool.Spwan(true); ;
            if (appendStr.Length>0)
                builder.SetStrs(appendStr);
        return builder;
    }



    /// <summary>
    /// 更新
    /// </summary>
    public static void Update()
    {
        for (int i = 0; i < m_usingBuilderList.Count; i++)
        {
            builderPool.Recycle(m_usingBuilderList[i]);
        }
        m_usingBuilderList.Clear();
    }



}
}
