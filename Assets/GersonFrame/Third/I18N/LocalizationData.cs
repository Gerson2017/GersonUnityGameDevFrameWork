using GersonFrame.ABFrame;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Localization
{
    public class LocalizationData
    {
        SystemLanguage language;
        CSVObject csv;

        public LocalizationData(SystemLanguage language)
        {
            this.language = language;
            Init();
        }

        void Init()
        {
            string path = "Localization/" + language.ToString() + ".csv";
            if (Application.isPlaying)
            {
                TextAsset textAsset =ResourceManager.Instance.LoadResource<TextAsset>("Assets/" + path);
                if (textAsset != null)
                    csv = CSVReader.Read(textAsset.text);
            }
            else
            {
                var filePath = Application.dataPath + "/" + path;
                var text = File.ReadAllText(filePath);
                csv = CSVReader.Read(text);
            }
        }

        public string GetValueByKey(string key, string defaultString, string columnName = "VALUE")
        {
            if (csv != null)
            {
                Dictionary<string, object> data = csv.GetDataByKey(key);
                if (data != null && data.ContainsKey(columnName) && data[columnName] != null)
                {
                    return data[columnName].ToString();
                }
                else
                {
                    //Debug.LogWarning("There is localization error. KEY :" + key);
                    return defaultString;
                }
            }
            else
            {
                return defaultString;
            }
        }
    }
}
