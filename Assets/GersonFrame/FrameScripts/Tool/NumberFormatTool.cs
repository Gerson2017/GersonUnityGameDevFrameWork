using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GersonFrame.Tool
{


    public class NumberFormatTool
    {
        public static string[] NUM_FORMAT_END = {"万", "亿", "万亿", "T", "aa", "bb", "cc", "dd", "ee", "ff",
        "gg", "hh", "ii", "jj", "kk", "ll", "mm", "nn", "oo", "pp",
        "qq", "rr", "ss", "tt", "uu", "vv", "ww", "xx", "yy", "zz",
        "Aa", "Bb", "Cc", "Dd", "Ee", "Ff", "Gg", "Hh", "Ii", "Jj",
        "Kk", "Ll", "Mm", "Nn", "Oo", "Pp", "Qq", "Rr", "Ss", "Tt",
        "Uu", "Vv", "Ww", "Xx", "Yy", "Zz"};
        public static string Format6(long value)
        {
            System.Numerics.BigInteger num = new System.Numerics.BigInteger(value);

            string[] exponentialArr = num.ToString("E").Split('E');

            float digit = float.Parse(exponentialArr[0]);
            int digitCount = int.Parse(exponentialArr[1]);

            string str = "";
            if (digitCount > 6)
            {
                int digitIdx = Mathf.FloorToInt((digitCount - 6) / 3);
                digit = digit * Mathf.Pow(10, digitCount - (digitIdx + 1) * 3);

                str = str + Mathf.FloorToInt(digit / 1000);

                int end = Mathf.FloorToInt(digit % 1000);
                if (end >= 100)
                {
                    str = str + Mathf.FloorToInt(end);
                }
                else if (end >= 10)
                {
                    str = str + "0" + Mathf.FloorToInt(end);
                }
                else
                {
                    str = str + "00" + Mathf.FloorToInt(end);
                }
                str = str + NUM_FORMAT_END[digitIdx];
            }
            else
            {
                if (digitCount >= 3)
                {
                    str = "" + num / 1000;
                    System.Numerics.BigInteger end = num % 1000;
                    if (end >= 100)
                    {
                        str = str + end;
                    }
                    else if (end >= 10)
                    {
                        str = str + "0" + end;
                    }
                    else
                    {
                        str = str + "00" + end;
                    }
                }
                else
                {
                    str = "" + num;
                }
            }

            return str;
        }
        public static string FormatDecimal(long value)
        {
            System.Numerics.BigInteger num = new System.Numerics.BigInteger(value);
            string[] exponentialArr = num.ToString("E").Split('E');

            float digit = float.Parse(exponentialArr[0]);
            int digitCount = int.Parse(exponentialArr[1]);

            string str = "";
            if (digitCount >= 5)
            {
                int digitIdx = Mathf.FloorToInt((digitCount - 5) / 4);
                digit = Mathf.FloorToInt((digit * Mathf.Pow(10, digitCount - (digitIdx + 1) * 4)));

                str = digit.ToString("#") + NUM_FORMAT_END[digitIdx];
            }
            else
            {
                str = num.ToString();
            }

            return str;
        }

        public static string FormatTime(int value)
        {
            int hour = value / 3600;
            int min = (value - hour * 3600) / 60;
            int sec = value % 60;
            return string.Format("{0:D2}:{1:D2}:{2:D2}", hour, min, sec);
        }
        public static string FormatTime4(int value)
        {
            int min = value / 60;
            int sec = value % 60;
            return string.Format("{0:D2}:{1:D2}", min, sec);
        }
        public static string FormatTimeMin(int value)
        {
            int hour = value / 3600;
            int min = (value - hour * 3600) / 60;

            return string.Format("{0:D2}:{1:D2}", hour, min);
        }


        /// <summary>       
        /// 时间戳转为C#格式时间    timeStamp=146471041000    count=3=> 时分秒
        /// </summary>       
        /// <param name=”timeStamp”></param>       
        /// <returns></returns>       
        public static string[] StringNowDateTimeSubtract(bool addhours = true,bool addminutes=true,bool addsecond=true)
        {
            DateTime currentTime = System.DateTime.Now;
            int count = 0;

            if (addhours) count++;
            if (addminutes) count++;
            if (addsecond) count++;

            int hour = 24 - currentTime.Hour;
            int minutes = 0 - currentTime.Minute;
            if (minutes <= 0)
            {
                minutes += 60;
                hour--;
            }
            int second = 0 - currentTime.Second;

            if (second < 0)
            {
                second += 60;
                minutes--;
            }
            if (minutes <= 0)
            {
                minutes += 60;
                hour--;
            }

            string[] timestr = new string[count];
            int index = 0;
            if (addhours)
            {
                if (hour < 10)
                    timestr[index] = "0" + hour;
                else
                    timestr[index] =  hour.ToString();
                index++;
            }
            if (addminutes)
            {
                
                if (minutes < 10) 
                    timestr[index] = "0" + minutes;
                else
                    timestr[index] = minutes.ToString();
                index++;
            }
            if (addsecond)
            {
                if (second < 10)
                    timestr[index] = "0" + second;
                else
                    timestr[index] = second.ToString();
            }
            return timestr;
        }
    }
}
