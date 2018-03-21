using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssetBundleDemo
{
    /// <summary>
    /// 音频信息的类
    /// </summary>
    public class AudioInfo
    {
        public string bundleName;   //资源包名称 -> 小写
        public string sourceName;   //资源名称
    }

    public class SoundManager : MonoBehaviour
    {

        private static SoundManager instance = null;

        public static SoundManager _Instacne
        {
            get
            {
                return instance;
            }
        }
        /// <summary>
        /// 保存当前的声音资源
        /// </summary>
        public Dictionary<string, AudioSource> DicAudio = new Dictionary<string, AudioSource>();
        /// <summary>
        /// 保存当前的声音信息
        /// </summary>
        public Dictionary<string, AudioInfo> DictAudioInfo = new Dictionary<string, AudioInfo>();

        public void InitData()
        {
            instance = this;

            AudioInfo audioInfo;
            //拍照声音信息
            audioInfo = new AudioInfo();
            audioInfo.bundleName = "audioasset/capture";
            audioInfo.sourceName = "Capture";
            DictAudioInfo.Add("Capture", audioInfo);
            //猴子声音信息
            audioInfo = new AudioInfo();
            audioInfo.bundleName = "audioasset/monkey";
            audioInfo.sourceName = "Monkey";
            DictAudioInfo.Add("Monkey", audioInfo);

            //保存拍照声音
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
            DicAudio.Add("Capture", source);

            //保存猴子声音
            source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
            DicAudio.Add("Monkey", source);

        }

        /// <summary>
        /// 加载声音
        /// </summary>
        /// <returns></returns>
        public void LoadAudioClip(AudioInfo info)
        {
            AssetBundle bundle = SourceManager._Instance.getAssetBundle(info.bundleName);
            AudioClip clip = bundle.LoadAsset<AudioClip>(info.sourceName);
            DicAudio[info.sourceName].clip = clip;
        }

        public void PlayAudio(string audio)
        {
            if (DicAudio.ContainsKey(audio))
                DicAudio[audio].Play();
        }


    }
}