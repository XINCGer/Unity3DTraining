using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    private AudioManager audioManager = null;
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip audioDieA;
    [SerializeField]
    private AudioClip audioDieB;
    [SerializeField]
    private AudioClip audioJump;
    // Use this for initialization
    void Start()
    {
        audioManager = this;
        audioSource = this.GetComponent<AudioSource>();
    }

    public void PlayJumpEffect()
    {
        if (audioJump != null)
        {
            audioSource.PlayOneShot(audioJump);
        }
    }

    public void PlayDieEffect()
    {
        if (audioDieA != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(audioDieA);
            Invoke("PlayDieEffectB",1.0f);
        }
    }

    private void PlayDieEffectB()
    {
        if (audioDieB != null)
        {
            audioSource.PlayOneShot(audioDieB);
        }
    }
}
