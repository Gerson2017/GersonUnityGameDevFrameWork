using System;

namespace GersonFrame.Tool
{
  
    public  class ClassTool
    {
        public static string Name<T>()
        {
            Type t = typeof(T);
            return t.Name;
        }

    }
 


}
