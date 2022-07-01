using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GersonFrame.Tool;

namespace GersonFrame.ABFrame
{

    public class ObjectManager : Singleton<ObjectManager>
    {

        ObjectManager() { }

        /// <summary>
        /// 对象池节点
        /// </summary>
        public Transform mRecycleTs { get; protected set; }
        /// <summary>
        /// 场景节点
        /// </summary>
        public Transform mSceneTs { get; protected set; }

        /// <summary>
        /// 未使用（回收的）的使用中的对象 对象池
        /// </summary>
        protected Dictionary<uint, List<ResourceObj>> m_ObjectPoolDic = new Dictionary<uint, List<ResourceObj>>();

        /// <summary>
        /// 暂存的Resourceobj Dic 每一个被objectmanager生成的资源 
        /// </summary>
        protected Dictionary<int, ResourceObj> m_RescourceObjDic = new Dictionary<int, ResourceObj>();

        /// <summary>
        /// ResourceObj 对象池
        /// </summary>
        protected ClassObjectPool<ResourceObj> m_ResourceObjPool = null;
        /// <summary>
        /// 根据异步的guid 存储ResourceObj 来判断是否时异步加载
        /// </summary>
        protected Dictionary<long, ResourceObj> m_AsyncResObjs = new Dictionary<long, ResourceObj>();


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="recyclets"></param>
        public void Init()
        {
            if (mRecycleTs==null|| mSceneTs == null)
            {
                mRecycleTs = new GameObject("RecycleTs").transform;
                mRecycleTs.gameObject.Hide();
                mSceneTs = new GameObject("SceneTs").transform;
                GameObject AssetParent = new GameObject("AssetsParent");
                mRecycleTs.SetParent(AssetParent.transform);
                mSceneTs.SetParent(AssetParent.transform);
                GameObject.DontDestroyOnLoad(AssetParent);
                m_ResourceObjPool = ObjectManager.Instance.GetOrCreateClassPool<ResourceObj>(1000);
            }
        }


        /// <summary>
        /// 清空对象池所有被设置为可以清理的对象
        /// </summary>
        public void ClearCache()
        {
            List<uint> tempList = new List<uint>();

            foreach (uint key in m_ObjectPoolDic.Keys)
            {
                List<ResourceObj> resobjlist = m_ObjectPoolDic[key];
                for (int i = resobjlist.Count - 1; i >= 0; i--)
                {
                    ResourceObj obj = resobjlist[i];
                    //可以克隆 且 可以被清除
                    if (!System.Object.ReferenceEquals(obj.m_CloneObj, null) && obj.m_bClear)
                    {
                        resobjlist.Remove(obj);
                        m_RescourceObjDic.Remove(obj.m_CloneObj.GetInstanceID());
                        GameObject.Destroy(obj.m_CloneObj);
                        obj.Reset();
                        m_ResourceObjPool.Recycle(obj);
                    }
                }
                ///判断资源是否全部被清除
                if (resobjlist.Count <= 0)
                    tempList.Add(key);
            }
            ///清除已经卸载 的资源对象池
            for (int i = 0; i < tempList.Count; i++)
            {
                uint tempcrc = tempList[i];
                if (m_ObjectPoolDic.ContainsKey(tempcrc))
                    m_ObjectPoolDic.Remove(tempcrc);
            }
            tempList.Clear();
            mClearProgress = 1;
        }

        public float mClearProgress { get; private set; } = 0;

        /// <summary>
        /// 异步清除缓存 减少清除资源性能消耗
        /// </summary>
        /// <param name="frameClearcount"></param>
        /// <param name="clearFinish"></param>
        public void ClearCache(MonoBehaviour asyncMono,Action clearFinish,int frameClearcount=20)
        {
            asyncMono.StartCoroutine(IEClearCahce(clearFinish,frameClearcount));
        }

        IEnumerator IEClearCahce(Action clearFinish,int frameClearcount)
        {
            float timer = Time.realtimeSinceStartup;
            mClearProgress = 0;
               yield return null;
            int count = 0;
            int index = 0;

            Dictionary<uint,List<ResourceObj>> dics = new Dictionary<uint, List< ResourceObj>>();
            foreach (var item in m_ObjectPoolDic)
            {
                List<ResourceObj> objs = new List<ResourceObj>();
                objs.AddRange(item.Value);
                dics.Add(item.Key, objs);
            }

            foreach (var item in dics)
            {
                mClearProgress = index / dics.Count * 1.0f;
                for (int i = item.Value.Count - 1; i >= 0; i--)
                {
                    if (i >= item.Value.Count)
                    {
                        MyDebuger.LogWarning("回收下标溢出 " + item.Key+ " i= "+i+ " objs.Count  " + item.Value.Count);
                        continue;
                    }
                    ResourceObj obj = item.Value[i];
                    //可以克隆 且 可以被清除
                    if (!ReferenceEquals(obj.m_CloneObj, null) && obj.m_bClear)
                    {
                        item.Value.Remove(obj);
                        MyDebuger.Log($"Remove {obj.m_Crc }  succeed {m_ObjectPoolDic[item.Key].Remove(obj)}");
                        m_RescourceObjDic.Remove(obj.m_CloneObj.GetInstanceID());
                        GameObject.Destroy(obj.m_CloneObj);
                        obj.Reset();
                        m_ResourceObjPool.Recycle(obj);
                        count++;
                        if (count >= frameClearcount)
                        {
                            count = 0;
                            yield return null;
                        }
                    }
                }
                index++;
            }

            List<uint> clearlist = new List<uint>();

            foreach (var item in m_ObjectPoolDic)
            {
                if (item.Value.Count<=0)
                    clearlist.Add(item.Key);
            }

            ///清除已经卸载 的资源对象池
            for (int i = 0; i < clearlist.Count; i++)
               m_ObjectPoolDic.Remove(clearlist[i]);  

            mClearProgress = 0.99f;
            yield return null;
            float timeinternal = Time.realtimeSinceStartup - timer;
            if (timeinternal<1.5f)
                yield return new WaitForSeconds(1.5f - timeinternal);
            mClearProgress = 1;
            clearFinish?.Invoke();
        }


        /// <summary>
        /// 清除某个资源再对象池中所有的资源
        /// </summary>
        public void ClearPoolObject(uint crc)
        {
            List<ResourceObj> reslist = null;
            if (!m_ObjectPoolDic.TryGetValue(crc, out reslist) || reslist == null) return;

            for (int i = reslist.Count - 1; i >= 0; i--)
            {
                ResourceObj resobj = reslist[i];
                //资源可以被清除
                if (resobj.m_bClear)
                {
                    reslist.Remove(resobj);
                    int tempId = resobj.m_CloneObj.GetInstanceID();
                    GameObject.Destroy(resobj.m_CloneObj);
                    resobj.Reset();
                    m_RescourceObjDic.Remove(tempId);
                    m_ResourceObjPool.Recycle(resobj);
                }
            }
            //对象池中的资源为0进行删除
            if (reslist.Count <= 0)
                m_ObjectPoolDic.Remove(crc);
        }


        /// <summary>
        /// 根据对象实例直接获取离线数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public OffLineData FindOffLineData(GameObject obj)
        {
            OffLineData offLineData = null;
            ResourceObj resobj = null;
            m_RescourceObjDic.TryGetValue(obj.GetInstanceID(), out resobj);
            if (resobj != null)
            {
                offLineData = resobj.m_offLineData;
            }
            return offLineData;
        }


        /// <summary>
        /// 从对象池里面取对象
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bclear"></param>
        /// <returns></returns>
        protected ResourceObj GetResourceObjFromPool(uint crc)
        {
            List<ResourceObj> list = null;
            if (this.m_ObjectPoolDic.TryGetValue(crc, out list) && list != null && list.Count > 0)
            {
                ///resourceManager 的引用计数
                ResourceManager.Instance.IncreaseResourceRef(crc);
                ResourceObj resourceObj = list[0];
                list.RemoveAt(0);
                GameObject obj = resourceObj.m_CloneObj;
                if (!System.Object.ReferenceEquals(obj, null))
                {
                    if (!System.Object.ReferenceEquals(resourceObj.m_offLineData, null))
                    {
                        resourceObj.m_offLineData.ResetPrpo();
                    }
                    resourceObj.m_Already = false;
#if UNITY_EDITOR
                    if (obj.name.EndsWith("(Recycle)"))
                        obj.name = obj.name.Replace("(Recycle)", "");
#endif
                }
                return resourceObj;
            }
            return null;
        }

        /// <summary>
        /// 取下异步加载 需要在所有的当前异步加载都取消掉的情况下 才会正在失效
        /// </summary>
        public void CancelAsyncLoad(int guid)
        {
            ResourceObj resobj = null;
            if (m_AsyncResObjs.TryGetValue(guid, out resobj) && ResourceManager.Instance.CancelAsyncLoad(resobj))
            {
                m_AsyncResObjs.Remove(guid);
                resobj.Reset();
                m_ResourceObjPool.Recycle(resobj);
            }
            else
                Debug.LogWarning("CancelAsyncLoad Fail  guid="+guid);
        }


        /// <summary>
        /// 是否正在异步加载
        /// </summary>
        /// <returns></returns>
        public bool IsAsyncLoading(int guid)
        {
            return m_AsyncResObjs.ContainsKey(guid);
        }


        /// <summary>
        /// 该对象是否是对象池创建的
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool IsObjectManangerCreate(GameObject obj)
        {
            int guid = obj.GetInstanceID();
            return m_RescourceObjDic.ContainsKey(guid);
        }

        

        /// <summary>
        /// 预加载路径 预加载尽量放在一般加载之前否则，如何之前加载的资源未销毁 则可能导致预加载资源无效 使用的是原先可以被销毁的资源
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="count">预加载个数</param>
        /// <param name="clear">场景跳转是否清除</param>
        public void PreLoadGameobject(string path, int count = 1, bool clear = false, Action<GameObject> loadgocallBack=null)
        {
            List<GameObject> tempGameobjectList = new List<GameObject>();
            for (int i = 0; i < count; i++)
            {
                GameObject obj = InstantiateObject(path, false, clear);
                tempGameobjectList.Add(obj);
            }
            for (int i = 0; i < count; i++)
            {
                GameObject obj = tempGameobjectList[i];
                ReleaseObject(obj);
                loadgocallBack?.Invoke(obj);
                obj = null;
            }
            tempGameobjectList.Clear();
        }

        public void PreLoadGameObjcetAsync(MonoBehaviour mono, string path, int count = 1, bool clear = false, Action<GameObject> loadgocallBack = null)
        {
            mono.StartCoroutine(PreLoadGameObjcetAsync(path,count,clear,loadgocallBack));
        }

        IEnumerator PreLoadGameObjcetAsync(string path, int count = 1, bool clear = false, Action<GameObject> loadgocallBack = null)
        {
            List<GameObject> tempGameobjectList = new List<GameObject>();
            for (int i = 0; i < count; i++)
            {
                GameObject obj = InstantiateObject(path, false, clear);
                tempGameobjectList.Add(obj);
            }
            yield return null;
            for (int i = 0; i < count; i++)
            {
                GameObject obj = tempGameobjectList[i];
                ReleaseObject(obj);
                loadgocallBack?.Invoke(obj);
                obj = null;
            }
            tempGameobjectList.Clear();
        }


        /// <summary>
        /// 同步加载
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bclear">是否可以被清除</param>
        /// <returns></returns>
        public GameObject InstantiateObject(string path, bool isSceneObj = true, bool bclear = true)
        {
            uint crc = Crc32.GetCrc32(path);
            ResourceObj resourceObj = GetResourceObjFromPool(crc);
            if (resourceObj == null)
            {
                resourceObj = m_ResourceObjPool.Spwan(true);
                resourceObj.m_Crc = crc;
                resourceObj.m_bClear = bclear;
                resourceObj.m_Guid = ResourceManager.Instance.CreateGuid();
                //resourcemanager提供加载方法
                resourceObj = ResourceManager.Instance.LoadResource(path, resourceObj);
                if (resourceObj.m_ResItem!=null&&resourceObj.m_ResItem.m_Obj != null)
                {
                    try
                    {
                        if (isSceneObj)
                            resourceObj.m_CloneObj = GameObject.Instantiate(resourceObj.m_ResItem.m_Obj, mSceneTs) as GameObject;
                        else
                            resourceObj.m_CloneObj = GameObject.Instantiate(resourceObj.m_ResItem.m_Obj) as GameObject;
                        ///赋值离线加载数据
                        resourceObj.m_offLineData = resourceObj.m_CloneObj.GetComponent<OffLineData>();
                    }
                    catch (Exception e)
                    {
                        MyDebuger.LogError(string.Format("InstantiateObject haserror object {0} error {1}", path,e.ToString()));
                    }
                }
            }
            if (resourceObj.m_CloneObj!=null)
            {
                if (isSceneObj&& resourceObj.m_CloneObj.transform.parent!=mSceneTs) 
                    resourceObj.m_CloneObj.transform.SetParent(this.mSceneTs, false);

                int tempId = resourceObj.m_CloneObj.GetInstanceID();
                if (!m_RescourceObjDic.ContainsKey(tempId))
                    m_RescourceObjDic.Add(tempId, resourceObj);
            }
            else
                MyDebuger.LogError(string.Format("Instantiate Object haserror object {0} fail please check file exist", path));
  
       //     MyDebuger.Log(path + "bclear " + resourceObj.m_bClear);
            return resourceObj.m_CloneObj;
        }

        /// <summary>
        /// 同步加载场景 默认在场景节点下isSceneObj=true  默认会被清除bclear 为了调用方便才写的
        /// </summary>
        /// <param name="patharay"></param>
        /// <returns></returns>
        public GameObject InstantiateObject(params string[] patharay)
        {
            MyStringBuilder stbuilder = StringBuilderTool.GetStringBuilder(patharay);
            return InstantiateObject(stbuilder.ToString());
        }


        /// <summary>
        /// 同步加载场景 默认在场景节点下isSceneObj=true  默认会被清除bclear 为了调用方便才写的
        /// </summary>
        /// <param name="patharay"></param>
        /// <returns></returns>
        public T GetCompentByInstanceObject<T>(string path, bool isSceneObj = true, bool bclear = true) where T:MonoBehaviour
        {
            GameObject go = InstantiateObject(path, isSceneObj, bclear);
            if (go == null) {
                MyDebuger.LogError("not found object " + path); return null;
            }
            T t = go.GetComponent<T>();
            return t;
        }


        /// <summary>
        /// 同步加载场景 默认在场景节点下isSceneObj=true  默认会被清除bclear 为了调用方便才写的
        /// </summary>
        /// <param name="patharay"></param>
        /// <returns></returns>
        public T GetCompentByInstanceObject<T>(params string[] paths) where T : MonoBehaviour
        {
            MyStringBuilder stbuilder = StringBuilderTool.GetStringBuilder(paths);
            return GetCompentByInstanceObject<T>(stbuilder.ToString());
        }

        /// <summary>
        /// 异步资源加载
        /// </summary>
        /// <param name="path"></param>
        /// <param name="finishCallback">加载完成回调</param>
        /// <param name="loadResPriority"></param>
        /// <param name="isSceneObj"></param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        /// <param name="bclear">跳场景是否删除</param>
        /// <returns></returns>
        public long InstantiateObjectAsync(string path, OnAsyncObjFinishDele finishCallback, LoadResPriority loadResPriority, bool isSceneObj = false, object param1 = null, object param2 = null, object param3 = null, bool bclear = true)
        {
            if (string.IsNullOrEmpty(path))
                return 0;
            uint crc = Crc32.GetCrc32(path);
            ResourceObj obj = GetResourceObjFromPool(crc);
            if (obj != null)
            {
                if (isSceneObj)
                    obj.m_CloneObj.transform.SetParent(this.mSceneTs, false);
                finishCallback?.Invoke(path, obj.m_CloneObj, param1, param2, param3);
                return obj.m_Guid;
            }

            int guid = ResourceManager.Instance.CreateGuid();
            obj = m_ResourceObjPool.Spwan(true);
            obj.m_Crc = crc;
            obj.m_bClear = bclear;
            obj.m_setSceneParent = isSceneObj;
            obj.m_FinishCallback = finishCallback;
            obj.m_Guid = guid;
            obj.Param1 = param1;
            obj.Param2 = param2;
            obj.Param3 = param3;
            if (!m_AsyncResObjs.ContainsKey(guid))
                m_AsyncResObjs.Add(guid, obj);
            //调用resourmanager 异步加载
            ResourceManager.Instance.AsnycLoadObjResource(path, obj, OnLoadInstanceObjFinish, loadResPriority);
            return guid;
        }

        /// <summary>
        ///  当实例化资源加载完毕时的回调
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="obj">中间类</param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        protected void OnLoadInstanceObjFinish(string path, ResourceObj resobj, object param1 = null, object param2 = null, object param3 = null)
        {
            if (resobj == null) return;
            if (resobj.m_ResItem.m_Obj == null)
            {
#if UNITY_EDITOR
                MyDebuger.LogError("异步加载实例化资源错误:" + path);
#endif
            }
            else
            {
                resobj.m_CloneObj = GameObject.Instantiate(resobj.m_ResItem.m_Obj) as GameObject;
                ///赋值离线加载数据
                resobj.m_offLineData = resobj.m_CloneObj.GetComponent<OffLineData>();
            }

            ///加载完成从正在加载的异步中移除
            if (m_AsyncResObjs.ContainsKey(resobj.m_Guid))
                m_AsyncResObjs.Remove(resobj.m_Guid);

            if (resobj.m_CloneObj != null && resobj.m_setSceneParent)
            {
                resobj.m_CloneObj.transform.SetParent(this.mSceneTs, false);
            }

            if (resobj.m_FinishCallback != null)
            {
                int tempId = resobj.m_CloneObj.GetInstanceID();
                if (!m_RescourceObjDic.ContainsKey(tempId))
                    m_RescourceObjDic.Add(tempId, resobj);

                resobj.m_FinishCallback(path, resobj.m_CloneObj, resobj.Param1, resobj. Param2, resobj.Param3);
            }
        }

        /// <summary>
        ///回收资源
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="maxCacheCount"> 回收是保留多少个 -1就是无穷个 0就是销毁当前回收的</param>
        /// <param name="destorycache">当obj资源所依赖的AB包的引用计数=0时(不被引用时) 是否卸载AB资源</param>
        /// <param name="recycleParent"></param>
        public bool ReleaseObject(GameObject obj, int maxCacheCount = -1, bool destorycache = false, bool recycleParent = true)
        {
            if (obj == null) return false;
            int guid = obj.GetInstanceID();
            ResourceObj resobj = null;

            if (!m_RescourceObjDic.TryGetValue(guid, out resobj))
            {
                MyDebuger.LogError(obj.name+ "对象不是ObjectManager 创建的 或者对象已经被清除");
                return false;
            }

            if (resobj == null)
            {
                MyDebuger.LogError("缓存的ResourceObj为空");
                return false;
            }

            if (resobj.m_Already)
            {
                MyDebuger.LogError("该对象已经放回对象池了 检查自己是否清空引用!");
                return false;
            }
#if UNITY_EDITOR
            if (obj != null)
                obj.name += "(Recycle)";
#endif
            List<ResourceObj> list = null;
            ///不缓存 直接销毁
            if (maxCacheCount == 0)
            {
                m_RescourceObjDic.Remove(guid);
                ResourceManager.Instance.ReleaseResource(resobj, destorycache);
                resobj.Reset();
                m_ResourceObjPool.Recycle(resobj);
            }
            //回收到对象池
            else
            {
                if (!m_ObjectPoolDic.TryGetValue(resobj.m_Crc, out list) || list == null)
                {
                    list = new List<ResourceObj>();
                    m_ObjectPoolDic.Add(resobj.m_Crc, list);
                }
                if (resobj.m_CloneObj)
                {
                    if (recycleParent)
                        resobj.m_CloneObj.transform.SetParent(this.mRecycleTs);
                    else
                        resobj.m_CloneObj.SetActive(false);
                }
                //只有这部分会存入缓存 存入的时候对引用计数进行减少 取的时候对引用计数进行减少
                if (maxCacheCount < 0 || list.Count < maxCacheCount)
                {
                    list.Add(resobj);
                    resobj.m_Already = true;
                    //resourceManager 做一个引用计数
                    ResourceManager.Instance.DecreaseResourceRef(resobj);
                }
                //达到最大缓存个数
                else
                {
                    m_RescourceObjDic.Remove(guid);
                    ResourceManager.Instance.ReleaseResource(resobj, destorycache);
                    resobj.Reset();
                    m_ResourceObjPool.Recycle(resobj);
                }
            }
            return true;
        }



        #region 类对象池使用 IOC容器

        protected Dictionary<Type, object> m_ClassPoolDic = new Dictionary<Type, object>();

        /// <summary>
        /// 创建类对象池 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="max"></param>
        /// <returns></returns>
        public ClassObjectPool<T> GetOrCreateClassPool<T>(int max) where T : class, new()
        {
            Type type = typeof(T);
            object outobj = null;
            if (!m_ClassPoolDic.TryGetValue(type, out outobj) || outobj == null)
            {
                ClassObjectPool<T> pool = new ClassObjectPool<T>(max);
                m_ClassPoolDic.Add(type, pool);
                return pool;
            }
            return outobj as ClassObjectPool<T>;
        }
        #endregion
    }
}
