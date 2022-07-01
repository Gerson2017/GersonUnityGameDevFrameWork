using GersonFrame.ABFrame;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Localization
{
    public class ACLocalizationData
    {
        CSVObject csv;
        public ACLocalizationData(string text)
        {
            Init(text);
        }

        void Init(string text)
        {
            csv = CSVReader.Read(text);
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
