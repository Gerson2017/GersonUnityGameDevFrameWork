using LitJson;
using System.Collections.Generic;
using UnityEngine;
using GersonFrame.Tool;
using GersonFrame.ABFrame;
using HotDragonRun.Proto;

namespace GersonFrame
{

    public enum AudioType
    {
        BackMusic,
        NormalMusic,//同时存在一个音效源
        MutipleMusic,//同时存在多个音效源
    }



    /// <summary>
    /// 音效播放信息
    /// </summary>
    [System.Serializable]
    public class AudioPlayerInfo
    {
        public int ID;
        private int MaxCount;
        private float MinPlayInternal = 0.1f;
        private int CurrentCount = 0;
        private float lastPlayTime = -1;
        public AudioClip mClip;

        public AudioPlayerInfo(int ID, int maxcount, float minPlayInternal)
        {
            this.ID = ID;
            this.MaxCount = maxcount;
            this.MinPlayInternal = minPlayInternal;
            lastPlayTime = -1;
        }

        public void ResetCount()
        {
            this.CurrentCount = 0;
            lastPlayTime = Time.realtimeSinceStartup;
        }


        /// <summary>
        /// 是否可以播放
        /// </summary>
        /// <returns></returns>
        public bool CanPlay()
        {
            if (lastPlayTime == -1)
            {
                lastPlayTime = Time.realtimeSinceStartup;
                this.CurrentCount++;
                return true;
            }
            else
            {
                float timedis = Time.realtimeSinceStartup - lastPlayTime;
                if (timedis <= this.MinPlayInternal || this.CurrentCount >= MaxCount)
                {
                    return false;
                }
                else if (timedis > this.MinPlayInternal)
                {
                    this.ResetCount();
                    this.CurrentCount++;
                    return true;
                }
                else
                {
                    this.CurrentCount++;
                    return true;
                }
            }
        }

    }

    /// <summary>
    /// 游戏音效设置信息
    /// </summary>
    public class AudioSettingInfo
    {
        public bool BackMusicIsMute = false;
        public bool AudioMuscIsMute = false;
    }

    public class AudioManager : MonoSingleton<AudioManager>
    {

        GameObject m_audioParent;

        [Tooltip("编辑器音量大小 对移动端无效")]
        [Range(0, 1)]
        public float m_Valume = 1;
        [Tooltip("最短时间内的最大播放次数")]
        [Range(1, 50)]
        public int MaxPlayCount = 5;
        [Tooltip("最短播放间隔")]
        [Range(0.01f, 10)]
        public float MinPlayInternal = 0.1f;
        [Tooltip("是否使用音效预加载")]
        public bool m_UsePreLoadAudio;


        private const string m_audioclipPath = "Assets/AudioClips/";

        private SimpleObjectPool<AudioItem> m_mutipleAudioSourcePool = new SimpleObjectPool<AudioItem>(GetMutipleAudioSources);

        /// <summary>
        ///普通音效 同一时间只能存在一个
        /// </summary>
        private List<int> m_normalAudios = new List<int>();
        /// <summary>
        /// 所属物体的音效字典
        /// </summary>
        private Dictionary<int, List<AudioItem>> m_BelongToAudioDic = new Dictionary<int, List<AudioItem>>();


        private Dictionary<int, SystemFunctionConfigAudioConfigConfig> m_audioDic = new Dictionary<int, SystemFunctionConfigAudioConfigConfig>();
        private Dictionary<int, AudioPlayerInfo> m_audioPlayeInfoDic = new Dictionary<int, AudioPlayerInfo>();



        private AudioItem m_backAudioSource;

        /// <summary>
        /// 背景音乐
        /// </summary>
        public AudioItem BackAudioSource
        {
            get
            {
                if (m_backAudioSource == null)
                {
                    this.m_backAudioSource = this.gameObject.AddComponent<AudioItem>();
                    this.m_backAudioSource.SetAudioSource(true);
                }

                return this.m_backAudioSource;
            }
        }


        //==============================游戏音效设置数据=============================
        /// <summary>
        /// 获取音效设置
        /// </summary>
        /// <returns></returns>
        public AudioSettingInfo GetMusicSettingData()
        {
            AudioSettingInfo settinginfo = PlayerPrefsTool.GetData("AudioSetting", new AudioSettingInfo());
            return settinginfo;
        }

        /// <summary>
        /// 保存音效设置
        /// </summary>
        /// <param name="backmusicIsmute">背景音效</param>
        /// <param name="audioMuscIsMute">普通音效</param>
        public void SetMusicSettingData(bool backmusicIsmute, bool audioMuscIsMute)
        {
            AudioSettingInfo settinginfo = GetMusicSettingData();
            settinginfo.AudioMuscIsMute = audioMuscIsMute;
            settinginfo.BackMusicIsMute = backmusicIsmute;
            PlayerPrefsTool.SetData("AudioSetting", settinginfo);
            BackAudioSource.Mute(settinginfo.BackMusicIsMute);

        }


        MyStringBuilder m_audioNameBuilder;

        /// <summary>
        /// 加载音效文件
        /// </summary>
        public void LoadAudioClips()
        {
            if (!Application.isEditor)
                m_Valume = 1;

            m_audioParent = new GameObject("AudioRoot");
            TextAsset audiotext = ResourceManager.Instance.LoadResource<TextAsset>("Assets/Configs/ProtoBytes/SystemFunctionConfigAudioConfig.bytes");
            if (audiotext == null)
            {
                MyDebuger.LogError("not found AudioInfo in Assets/Configs/ProtoBytes/AudioConfig.bytes");
                return;
            }
            SystemFunctionConfigAudioConfigConfigData audioConfigConfigData = SystemFunctionConfigAudioConfigConfigData.Parser.ParseFrom(audiotext.bytes);
           // List<AudioInfo> audioList = JsonMapper.ToObject<List<AudioInfo>>(audiotext.text);
            if (audioConfigConfigData.SystemFunctionConfigAudioConfigConfigs.Count < 1) MyDebuger.LogWarning("音乐配置文件中没有找到可用信息");
            for (int i = 0; i < audioConfigConfigData.SystemFunctionConfigAudioConfigConfigs.Count; i++)
            {
                SystemFunctionConfigAudioConfigConfig audiInfo = audioConfigConfigData.SystemFunctionConfigAudioConfigConfigs[i];
                int id = audiInfo.ID;
                m_audioDic[id] = audiInfo;
                AudioPlayerInfo audioPlayerInfo = new AudioPlayerInfo(id, this.MaxPlayCount, this.MinPlayInternal);
                m_audioPlayeInfoDic[id] = audioPlayerInfo;
                if (m_UsePreLoadAudio)
                {
                    AudioClip clip = LoadAudoClip(audiInfo.AudioName);
                    if (clip == null) continue;
                    audioPlayerInfo.mClip = clip;
                }

            }
            MyDebuger.Log("音效信息加载完毕");
        }



        AudioClip LoadAudoClip(string audiName)
        {
            if (m_audioNameBuilder == null)
                m_audioNameBuilder = StringBuilderTool.GetStringBuilder();
            m_audioNameBuilder.SetStrs(m_audioclipPath, audiName);
            AudioClip clip = ResourceManager.Instance.LoadResource<AudioClip>(m_audioNameBuilder.ToString());
            if (clip == null)
            {
                MyDebuger.LogError("not found " + m_audioclipPath + audiName);
                return null;
            }
            return clip;
        }

        bool LoadAudoClip(int id)
        {
            if (m_audioNameBuilder == null)
                m_audioNameBuilder = StringBuilderTool.GetStringBuilder();
            if (m_audioPlayeInfoDic.ContainsKey(id))
            {
                if (m_audioPlayeInfoDic[id].mClip == null)
                {
                    AudioClip clip = LoadAudoClip(m_audioDic[id].AudioName);
                    if (clip != null)
                    {
                        m_audioPlayeInfoDic[id].mClip = clip;
                        return true;
                    }
                    return false;
                }
                return true;
            }
            MyDebuger.LogError("not found  audio id " + id);
            return false;
        }


        public AudioItem PlayeAudio(int id, int belongtoId = -1)
        {
            if (LoadAudoClip(id))
            {
                AudioPlayerInfo audioPlayerInfo = m_audioPlayeInfoDic[id];
                if (audioPlayerInfo.CanPlay())
                {
                    SystemFunctionConfigAudioConfigConfig audioInfo = this.m_audioDic[id];
                    return this.AudioSourcePlay(audioInfo, belongtoId, audioPlayerInfo.mClip);
                }
            }
            else MyDebuger.LogError("play audio fail  not found audio " + id);
            return null;
        }


        private AudioItem GetPlayAudioSource(int id)
        {
            if (!m_audioDic.ContainsKey(id))
            {
                MyDebuger.LogError("play audio fail  not found audio " + id);
                return null;
            }
            AudioType tempaudioType = (AudioType)System.Enum.Parse(typeof(AudioType), m_audioDic[id].AudioType);
            switch (tempaudioType)
            {
                case AudioType.BackMusic:
                    if (this.GetMusicSettingData().BackMusicIsMute)
                        BackAudioSource.Mute(true);
                    return BackAudioSource;
                case AudioType.NormalMusic:
                    if (this.m_normalAudios.Contains(id)) return null;
                    m_normalAudios.Add(id);
                    AudioItem nromalaudioItem = GetAuidoItem();
#if UNITY_EDITOR
                    if (nromalaudioItem != null)
                        nromalaudioItem.gameObject.name = "normalAudio";
#endif
                    return nromalaudioItem;
                case AudioType.MutipleMusic:
                    AudioItem mutipleaudioItem = GetAuidoItem();
#if UNITY_EDITOR
                    if (mutipleaudioItem != null)
                        mutipleaudioItem.gameObject.name = "mutipleAudio";
#endif
                    return mutipleaudioItem;
                default:
                    MyDebuger.LogError("can not found audio type " + tempaudioType);
                    return null;
            }
        }

        AudioItem GetAuidoItem()
        {
            if (this.GetMusicSettingData().AudioMuscIsMute) return null;
            AudioItem audiosource = this.m_mutipleAudioSourcePool.Allocate();
            audiosource.transform.SetParent(m_audioParent.transform);
            return audiosource;
        }

        /// <summary>
        /// 获得重叠播放音效播放 
        /// </summary>
        /// <returns></returns>
        private static AudioItem GetMutipleAudioSources()
        {
            GameObject go = new GameObject();
            AudioItem audioItem = go.AddComponent<AudioItem>();
            audioItem.SetAudioSource();
            return audioItem;
        }
        private AudioItem AudioSourcePlay(SystemFunctionConfigAudioConfigConfig audoinfo, int belongtoId,AudioClip clip=null)
        {
            AudioItem audioItem = this.GetPlayAudioSource(audoinfo.ID);
            if (audioItem == null) return null;
            if (clip!=null)
                audioItem.Play(audoinfo, clip, this.m_Valume, belongtoId);
            else
                audioItem.Play(audoinfo,m_audioPlayeInfoDic[audoinfo.ID].mClip,  this.m_Valume, belongtoId);

            if (belongtoId != -1)
            {
                if (!this.m_BelongToAudioDic.ContainsKey(belongtoId))
                    m_BelongToAudioDic[belongtoId] = new List<AudioItem>();
                this.m_BelongToAudioDic[belongtoId].Add(audioItem);
            }
            return audioItem;
        }




        public void PaseAudio(AudioItem audioItem)
        {
            audioItem.Pause();
        }

        public void PauseAllAudioBy(int id)
        {
            for (int i = 0; i < this.m_mutipleAudioSourcePool.UsingCount; i++)
            {
                AudioItem item = this.m_mutipleAudioSourcePool.mUsingStack[i];
                if (item.mAudioId == id)
                {
                    item.Pause();
                }
            }
        }




        public void ResumeItem(AudioItem audiitem)
        {
            audiitem.Resume();
        }

        /// <summary>
        /// 恢复音效播放 临时方案 后期要使用ID替换
        /// </summary>
        /// <param name="audioName"></param>
        public void ResumeAllAudioById(int id)
        {
            for (int i = 0; i < this.m_mutipleAudioSourcePool.UsingCount; i++)
            {
                AudioItem item = this.m_mutipleAudioSourcePool.mUsingStack[i];
                if (item.mAudioId == id)
                    item.Resume();
            }
        }


        public void StopAudio(AudioItem audioitem)
        {
            audioitem.Stop();
        }

        public void StopAllAudioBy(int id)
        {
            for (int i = 0; i < this.m_mutipleAudioSourcePool.UsingCount; i++)
            {
                AudioItem item = this.m_mutipleAudioSourcePool.mUsingStack[i];
                if (item.mAudioId == id)
                {
                    item.Stop();
                }
            }
        }


        public AudioItem PlayMutipleAudio(int id, int belongtoId = -1)
        {
            if (LoadAudoClip(id))
            {
                AudioPlayerInfo audioPlayerInfo = m_audioPlayeInfoDic[id];
                if (!audioPlayerInfo.CanPlay())
                    return null;
                SystemFunctionConfigAudioConfigConfig info = this.m_audioDic[id];
                if (info.AudioType != AudioType.MutipleMusic.ToString())
                {
                    MyDebuger.LogWarning("audio type is wrong " + info.AudioType + " id=" + id + " belongToId " + belongtoId);
                    return null;
                }
                return this.AudioSourcePlay(info, belongtoId, audioPlayerInfo.mClip);
            }
            else
            {
                MyDebuger.LogError("PlayMutipleAudio play audio fail  not found audio " + id + " belongToId " + belongtoId);
                return null;

            }
        }





        public void RecycleAudioItem(AudioItem audioItem)
        {
            if (m_normalAudios.Contains(audioItem.mAudioId))
                m_normalAudios.Remove(audioItem.mAudioId);
            if (audioItem.mBelongToId != -1)
                this.m_BelongToAudioDic[audioItem.mBelongToId].Remove(audioItem);
            this.m_mutipleAudioSourcePool.Recycle(audioItem);
        }


        /// <summary>
        /// 暂停播放所有音效（是否包含背景音效）
        /// </summary>
        /// <param name="containsBackMusic"></param>
        public void PauseAllAudio(bool containsBackMusic = false)
        {
            if (containsBackMusic)
                BackAudioSource.Pause();

            for (int i = 0; i < this.m_mutipleAudioSourcePool.UsingCount; i++)
                this.m_mutipleAudioSourcePool.mUsingStack[i].Pause();

        }


        /// <summary>
        /// 暂停播放所有音效（是否包含背景音效）
        /// </summary>
        /// <param name="containsBackMusic"></param>
        public void ResumeAllAudio(bool containsBackMusic = false)
        {
            if (containsBackMusic)
                BackAudioSource.Resume();

            for (int i = 0; i < this.m_mutipleAudioSourcePool.UsingCount; i++)
            {
                this.m_mutipleAudioSourcePool.mUsingStack[i].Resume();
            }
        }


        /// <summary>
        /// 暂停播放所有音效（是否包含背景音效）
        /// </summary>
        /// <param name="containsBackMusic"></param>
        public void StopAllAudio(bool containsBackMusic = false)
        {
            if (containsBackMusic)
                BackAudioSource.Stop();
            for (int i = 0; i < this.m_mutipleAudioSourcePool.UsingCount; i++)
            {
                this.m_mutipleAudioSourcePool.mUsingStack[i].Stop();
            }
        }

        /// <summary>
        /// 根据所属物体ID停止播放音效
        /// </summary>
        /// <param name="belongToId"></param>
        public void StopAudioByBelongToId(int belongToId)
        {
            if (this.m_BelongToAudioDic.ContainsKey(belongToId))
            {
                List<AudioItem> audioItems = this.m_BelongToAudioDic[belongToId];
                for (int i = 0; i < audioItems.Count; i++)
                {
                    audioItems[i].Stop();
                }
            }
        }

    }
}
