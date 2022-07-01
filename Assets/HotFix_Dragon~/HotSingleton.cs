using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


    public abstract class HotSingleton<T> where T : HotSingleton<T>
    {
        private static T mInstance;

        public static T Instance
        {
            get
            {
                if (mInstance == null)
                {
                    var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);

                    var ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);

                    if (ctor == null)
                        throw new Exception("Non-public ctor() not found!");

                    mInstance = ctor.Invoke(null) as T;
                }
                return mInstance;
            }
        }

        protected HotSingleton()
        {

        }

    }

