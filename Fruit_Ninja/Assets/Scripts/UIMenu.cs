using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{

    private Button btnPlay;
    private Button btnSound;

    // Use this for initialization
    void Start()
    {
        InitComponents();
        btnPlay.onClick.AddListener(OnBtnPlayClick);
        btnSound.onClick.AddListener(OnBtnSoundClick);
    }


    private void InitComponents()
    {
        btnPlay = transform.Find("Btn_Play").GetComponent<Button>();
        btnSound = transform.Find("Btn_Sound").GetComponent<Button>();
    }

    private void OnBtnPlayClick()
    {
        SceneManager.LoadScene("",LoadSceneMode.Single);
    }

    private void OnBtnSoundClick()
    {
        
    }

    public void OnDestroy()
    {
        btnPlay.onClick.RemoveAllListeners();
        btnSound.onClick.RemoveAllListeners();
    }


}
