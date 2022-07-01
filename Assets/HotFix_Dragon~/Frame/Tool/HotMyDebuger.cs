using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotGersonFrame.Tool
{
   public  class HotMyDebuger
    {

        public static void LogError<T>(T obj, string errorInfo) 
        {
            Type t = typeof(T);
            MyDebuger.LogError(t .FullName+" "+ errorInfo);
        }


        public static void LogWarning<T>(T obj, string warningInfo)
        {
            Type t = typeof(T);
            MyDebuger.LogWarning(t.FullName + " " + warningInfo);
        }


    }
}
