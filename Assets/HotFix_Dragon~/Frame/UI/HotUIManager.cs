

using GersonFrame.UI;
using UnityEngine;
using System.Collections;

namespace HotGersonFrame
{
    public static class HotUIManager
    {


        public static Transform CanvasTs => UIManager.Instance.CanvasTs;


        public static Camera UICamera => UIManager.Instance.UICamera;


        public static RectTransform RectCanvasTs => UIManager.Instance.RectCanvasTs;


        public static void StopCoroutine(IEnumerator enumerator)
        {
            UIManager.Instance.StopCoroutine(enumerator);
        }
        public static void StartCoroutine(IEnumerator enumerator)
        {
            UIManager.Instance.StartCoroutine(enumerator);
        }

        public static void ResetAllShowingView()
        {
            UIManager.ResetAllShowingView();
        }

  


        /// <summary>
        /// 显示热更域的UI界面 非热更域界面会报错
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uimgr"></param>
        /// <param name="param"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        /// <returns></returns>
        public static BaseHotView ShowHotView<T>(object param = null, object param2 = null, object param3 = null) where T : BaseHotView
        {
            var type = typeof(T);
            if (Application.isEditor)
            {
                if (!type.FullName.Contains("HotFix_Dragon"))
                {
                    MyDebuger.LogError("ShowHotView<T> 只能使用该方法打开热更域的界面 viewname " + type.Name);
                }
            }

            return UIManager.ShowView(type.Name, true, param, param2, param3);
        }

        /// <summary>
        /// 关闭热更域的UI界面 非热更域界面会报错
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uimgr"></param>
        /// <param name="param"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        /// <returns></returns>
        public static void HideHotView<T>(bool destory = false) where T : BaseHotView
        {
            var type = typeof(T);
            if (Application.isEditor)
            {

                if (!type.FullName.Contains("HotFix_Dragon"))
                {
                    MyDebuger.LogError("HideHotView<T> 只能使用该方法关闭热更域的界面 viewname " + type.Name);
                }
            }

            UIManager.HideView(type.Name, destory);
        }


        public static BaseHotView GetHotView<T>() where T : BaseHotView
        {
            var type = typeof(T);
            if (Application.isEditor)
            {
                if (!type.FullName.Contains("HotFix_Dragon"))
                {
                    MyDebuger.LogError("GetHotView<T> 只能使用该方法获取热更域的界面 viewname " + type.Name);
                }
            }
             return UIManager.GetView(type.Name);
        }



    }
}
