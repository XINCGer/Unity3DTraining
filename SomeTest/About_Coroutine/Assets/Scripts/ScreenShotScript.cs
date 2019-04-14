using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScreenShotScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine("ScreenShotPNG");
        }
    }


    IEnumerator ScreenShotPNG()
    {
        yield return new WaitForEndOfFrame();
        int width = Screen.width;
        int height = Screen.height;
        Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGB24, false);
        texture2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture2D.Apply();
        byte[] bytes = texture2D.EncodeToPNG();
        Destroy(texture2D);
        File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);
    }
}
