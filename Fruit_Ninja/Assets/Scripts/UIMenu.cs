using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{

    private Button btnPlay;
    private Button btnSound;

    private Image imgSound;

    public Sprite[] spriteSound;

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
        imgSound = transform.Find("Btn_Sound").GetComponent<Image>();
    }

    private void OnBtnPlayClick()
    {
        SceneManager.LoadScene("Play",LoadSceneMode.Single);
    }

    private void OnBtnSoundClick()
    {
        if (GameManager.GetInstance().IsAudioPlaying())
        {
            GameManager.GetInstance().PauseSound();
            imgSound.sprite = spriteSound[1];
        }
        else
        {
            GameManager.GetInstance().PlaySound();
            imgSound.sprite = spriteSound[0];
        }


    }

    public void OnDestroy()
    {
        btnPlay.onClick.RemoveAllListeners();
        btnSound.onClick.RemoveAllListeners();
    }


}
