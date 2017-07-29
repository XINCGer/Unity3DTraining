using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIGameOver : MonoBehaviour
{


    public void GameOver()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Singlton");
        for (int i = 0; i < gos.Length; i++)
        {
            Destroy(gos[i]);
        }
        SceneManager.LoadScene("Menu");
    }
}
