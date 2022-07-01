
using Google.Protobuf.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HotGersonFrame.Tool
{
   public  interface ICanSearch
    {

        int SearchID { get; }

    }


    public class SearchTool
    {

        public static T GetSerchValue<T>(List<T> canSearches, int searchid) where T:ICanSearch
        {
            if (canSearches == null || canSearches.Count < 1)
            {
                System.Type t = typeof(T);
                MyDebuger.LogErrorFormat("GetSerchIndex 要查找的数据集为空或数据数量为 0  searid= {0} type {1} } ", searchid, t);
                return default(T);
            }
            int low = 0;
            int high = canSearches.Count - 1;
            int mid = 0;
            while (low <= high)
            {
                int tempvalue = Mathf.CeilToInt((float)(searchid - canSearches[low].SearchID) / (float)(canSearches[high].SearchID - canSearches[low].SearchID) * (high - low));
                //插值查找 适用于关键字数据分布比较均匀 静态有序的数据
                mid = low + tempvalue;
                if (canSearches.Count > mid)
                {
                    if (searchid < canSearches[mid].SearchID)
                        high = mid - 1;
                    else if (searchid > canSearches[mid].SearchID)
                        low = mid + 1;
                    else return canSearches[mid];
                }
                else
                {
                    MyDebuger.LogErrorFormat("GetSerchIndex 未查找到数据{0} 在数据集 {1} ", searchid, canSearches);
                    return default(T);
                }
            }
            MyDebuger.LogErrorFormat("GetSerchIndex 未查找到数据{0} 在数据集 {1} ", searchid, canSearches);
            return default(T);
        }


        public static T GetSerchValue<T>(RepeatedField<T> canSearches, int searchid) where T : ICanSearch
        {
            if (canSearches == null || canSearches.Count < 1)
            {
                System.Type t = typeof(T);
                MyDebuger.LogErrorFormat("GetSerchIndex 要查找的数据集为空或数据数量为 0  searid= {0} type {1} } ", searchid, t);
                return default(T);
            }
            int low = 0;
            int high = canSearches.Count - 1;
            int mid = 0;
            while (low <= high)
            {
                int tempvalue = Mathf.CeilToInt((float)(searchid - canSearches[low].SearchID) / (float)(canSearches[high].SearchID - canSearches[low].SearchID) * (high - low));
                //插值查找 适用于关键字数据分布比较均匀 静态有序的数据
                mid = low + tempvalue;
                if (canSearches.Count > mid)
                {
                    if (searchid < canSearches[mid].SearchID)
                        high = mid - 1;
                    else if (searchid > canSearches[mid].SearchID)
                        low = mid + 1;
                    else return canSearches[mid];
                }
                else
                {
                    MyDebuger.LogErrorFormat("GetSerchIndex 未查找到数据{0} 在数据集 {1} ", searchid, canSearches);
                    return default(T);
                }
            }
            MyDebuger.LogErrorFormat("GetSerchIndex 未查找到数据{0} 在数据集 {1} ", searchid, canSearches);
            return default(T);
        }

        public static T GetSerchValue<T>(T[] canSearches, int searchid) where T : ICanSearch
        {
            if (canSearches == null || canSearches.Length < 1)
            {
                MyDebuger.LogErrorFormat("GetSerchIndex 要查找的数据集为空或数据数量为 0  searid= {0}} ", searchid);
                return default(T);
            }
            int low = 0;
            int high = canSearches.Length - 1;
            int mid = 0;
            while (low <= high)
            {
                int tempvalue = Mathf.CeilToInt((float)(searchid - canSearches[low].SearchID) / (float)(canSearches[high].SearchID - canSearches[low].SearchID) * (high - low));
                //插值查找 适用于关键字数据分布比较均匀 静态有序的数据
                mid = low + tempvalue;
                if (searchid < canSearches[mid].SearchID)
                    high = mid - 1;
                else if (searchid > canSearches[mid].SearchID)
                    low = mid + 1;
               else return canSearches[mid];
            }
            MyDebuger.LogErrorFormat("GetSerchIndex 未查找到数据{0} 在数据集 {1} ", searchid, canSearches);
            return default(T);
        }

        public static int GetSerchIndex<T>(T[] canSearches, int searchid) where T : ICanSearch
        {
            if (canSearches == null || canSearches.Length < 1)
            {
                MyDebuger.LogErrorFormat("GetSerchIndex 要查找的数据集为空或数据数量为 0  searid= {0}} ", searchid);
                return 0;
            }
            int low = 0;
            int high = canSearches.Length - 1;
            int mid = 0;
            while (low <= high)
            {
                int tempvalue = Mathf.CeilToInt((float)(searchid - canSearches[low].SearchID) / (float)(canSearches[high].SearchID - canSearches[low].SearchID) * (high - low));
                //插值查找 适用于关键字数据分布比较均匀 静态有序的数据
                mid = low + tempvalue;
                if (searchid < canSearches[mid].SearchID)
                    high = mid - 1;
                else if (searchid > canSearches[mid].SearchID)
                    low = mid + 1;
               else return mid;
            }
            MyDebuger.LogErrorFormat("GetSerchIndex 未查找到数据{0} 在数据集 {1} ", searchid, canSearches);
            return 0;
        }
        public static int GetSerchIndex<T>(List<T> canSearches, int searchid) where T : ICanSearch
        {
            if (canSearches == null || canSearches.Count < 1)
            {
                System.Type t = typeof(T);
                MyDebuger.LogErrorFormat("GetSerchIndex 要查找的数据集为空或数据数量为 0  searid= {0} type {1} } ", searchid, t);
                return 0;
            }
            int low = 0;
            int high = canSearches.Count - 1;
            int mid = 0;
            while (low <= high)
            {
                float tempvalue =(float)(searchid - canSearches[low].SearchID) / (float)(canSearches[high].SearchID - canSearches[low].SearchID) * (high - low);
                //插值查找 适用于关键字数据分布比较均匀 静态有序的数据
                mid =Mathf.CeilToInt(low + tempvalue);
                if (mid>high)
                {
                    MyDebuger.LogError( "查找异常  mid>heigh");
                    break;
                }
                if (searchid < canSearches[mid].SearchID)
                    high = mid - 1;
                else if (searchid > canSearches[mid].SearchID)
                    low = mid + 1;
                else return mid;

            }
            MyDebuger.LogErrorFormat("GetSerchIndex 未查找到数据{0} 在数据集 {1} ", searchid, canSearches);
            return 0;
        }
    }

    }
