using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private static GameManager _instance = null;
    private AudioSource audioSource;

    void Awake()
    {
        _instance = this;
    }


    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        audioSource = this.GetComponent<AudioSource>();
    }

    public static GameManager GetInstance()
    {
        return _instance;
    }

    public void PlaySound()
    {
        audioSource.Play();
    }

    public void PauseSound()
    {
        audioSource.Pause();
    }

    public void StopSound()
    {
        audioSource.Stop();
    }

    public void SetAudioClip(AudioClip clip)
    {
        audioSource.clip = clip;
    }

    public bool IsAudioPlaying()
    {
        return audioSource.isPlaying;
    }
}
