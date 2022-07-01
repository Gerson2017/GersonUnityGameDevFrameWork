using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class CSVReader
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public static CSVObject Read(string content)
    {
        var list = new List<Dictionary<string, object>>();
        CSVObject csv = new CSVObject();

        var lines = Regex.Split(content, LINE_SPLIT_RE);

        if (lines.Length <= 1)
        {
            csv.SetDictionary(list);
            return csv;
        }
        
        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\n", "\n");
                object finalvalue = value;
                int n;
                float f;
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            }
            list.Add(entry);
        }
        csv.SetDictionary(list);
        return csv;
    }
}

public class CSVObject
{
    List<Dictionary<string, object>> dict;

    public void SetDictionary(List<Dictionary<string, object>> dict)
    {
        this.dict = dict;
    }

    public Dictionary<string, object> GetDataByKey(string code)
    {
        for (int i = 0; i < dict.Count; i++)
        {
            if(dict[i].ContainsKey("KEY"))
            {
                if (dict[i]["KEY"].Equals(code))
                {
                    return dict[i];
                }
            }
        }
        return null;
    }
}
