
using HotDragonRun.Proto;
using UnityEngine;
namespace GersonFrame
{
    public class AudioItem : MonoBehaviour
    {

        private bool m_isbackAudio;

        /// <summary>
        /// 所属哪个物体发出声音
        /// </summary>
        public int mBelongToId { get; private set; }

        /// <summary>
        /// 音效ID
        /// </summary>
        public int mAudioId { get; private set; }

        private bool m_isPause = false;

        public AudioSource mAudiosouce
        {
            get; private set;
        }

        /// <summary>
        /// 是否是背景音效
        /// </summary>
        /// <param name="isbackAudio"></param>
        public void SetAudioSource(bool isbackAudio = false)
        {
            mAudiosouce = this.gameObject.AddComponent<AudioSource>();
            mAudiosouce.playOnAwake = false;
            m_isbackAudio = isbackAudio;
        }

        // Update is called once per frame
        void Update()
        {
            if (mAudiosouce.isPlaying) return;
            if (m_isPause) return;
            if (m_isbackAudio) return;
            Recycle();
        }


        void Recycle()
        {
            AudioManager.Instance.RecycleAudioItem(this);
            gameObject.Hide();
            this.mAudioId = -1;
            this.mBelongToId = -1;
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        public void Play(SystemFunctionConfigAudioConfigConfig audioInfo,AudioClip clip,  float volumemutiple, int belongToId = -1)
        {
            gameObject.Show();
            this.mAudioId = audioInfo.ID;
            this.mBelongToId = belongToId;
            this.mAudiosouce.clip = clip;
            this.mAudiosouce.clip = clip;
            this.mAudiosouce.volume = audioInfo.Volume * volumemutiple;
            this.mAudiosouce.loop = audioInfo.Loop;
            this.mAudiosouce.PlayDelayed(audioInfo.Delay);
        }

        public void Pause()
        {
            m_isPause = true;
            if (!mAudiosouce.isPlaying) return;
            mAudiosouce.Pause();
        }


        public void Resume()
        {
            m_isPause = false;
            if (mAudiosouce.isPlaying) return;
            mAudiosouce.UnPause();
        }


        /// <summary>
        /// belongtoid 和播放时候指定的belongtoid进行对比 以防止错误的停止该组件对新音效的播放
        /// </summary>
        /// <param name="belongtoid"></param>
        public void Stop(int belongtoid=-1)
        {
            if (gameObject.activeInHierarchy&& belongtoid==mBelongToId)
            {
                m_isPause = false;
                mAudiosouce.Stop();
            }
        }


        /// <summary>
        /// 是否静音
        /// </summary>
        /// <param name="ismute"></param>
        public void Mute(bool ismute)
        {
            this.mAudiosouce.mute = ismute;
        }

    }
}