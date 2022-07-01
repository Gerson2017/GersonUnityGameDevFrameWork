using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace GersonFrame.Tool
{
    public class PlayerPrefsTool 
    {

        /// <summary>
        /// 是否存在数据
        /// </summary>
        /// <returns></returns>
        public static bool ExistData(string dataKey)
        {
            return PlayerPrefs.HasKey(dataKey);
        }


        /**获取Int类型数据 */
        public static int GetInt(string valueName, int defaultvalue = 0)
        {
            var value = PlayerPrefs.GetInt(valueName);
            if (value == 0)
            {
                value = defaultvalue;
                PlayerPrefsTool.SetInt(valueName, value);
            }
            return value;
        }

        /**设置Int类型数据 */
        public static void SetInt(string valueName, int value)
        {
            PlayerPrefs.SetInt(valueName, value);
        }




        /**获取Sytring类型数据 */
        public static string GetStr(string valueName, string defaultvalue = "")
        {
            var value = PlayerPrefs.GetString(valueName);
            if (value == "")
            {
                value = defaultvalue;
                PlayerPrefsTool.SetStr(valueName, value);
            }
            return value;
        }

        /**设置String类型数据 */
        public static void SetStr(string valueName, string valuestr)
        {
            PlayerPrefs.SetString(valueName, valuestr);
        }

        /**获取对象类型数据 */
        public static T GetData<T>(string valueName, T defaultvalue)
        {
            string jsonData = PlayerPrefsTool.GetStr(valueName);
            if (jsonData == null || jsonData == "")
            {
                PlayerPrefsTool.SetData<T>(valueName, defaultvalue);
                return defaultvalue;
            }
            T t = LitJson.JsonMapper.ToObject<T>(jsonData);
            return t;
        }

        /**设置对象类型数据 */
        public static void SetData<T>(string valueName, T valuedata)
        {
            try
            {
                if (typeof(T).Name == "String")
                {
                    PlayerPrefs.SetString(valueName, valuedata.ToString());
                }
                else
                {
                    string datastr = LitJson.JsonMapper.ToJson(valuedata);
                    PlayerPrefs.SetString(valueName, datastr);
                }
            }
            catch (System.Exception e)
            {
                MyDebuger.LogError(e);
            }
        }
    }
}
