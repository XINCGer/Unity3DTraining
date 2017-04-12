using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WebManager : MonoBehaviour
{
	string m_info = "Nothing";
	//用于保存图片信息的Texture2D
	public Texture2D m_uploadImage;
	protected Texture2D m_downloadTexture;
	protected AudioClip m_downloadClip;

	// Use this for initialization
	void Start ()
	{
		StartCoroutine (DownloadSound ());
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnGUI ()
	{
		GUI.BeginGroup (new Rect (Screen.width * 0.5f - 100, Screen.height * 0.5f - 100, 500, 200), "");
		GUI.Label (new Rect (10, 10, 400, 30), m_info);
		//创建按钮
		if (GUI.Button (new Rect (10, 50, 150, 30), "Get Data")) {
			StartCoroutine (IGetData ());
		}
		if (GUI.Button (new Rect (10, 100, 150, 30), "Post Data")) {
			StartCoroutine (IPostData ());
		}
		if (m_downloadTexture != null) {
			GUI.DrawTexture (new Rect (0, 0, m_downloadTexture.width, m_downloadTexture.height), m_downloadTexture);
		}
		if (GUI.Button (new Rect (10, 150, 150, 30), "Request Image")) {
			StartCoroutine (IRequestPNG ());
		}
		GUI.EndGroup ();
	}

	//GET请求
	IEnumerator IGetData ()
	{
		//使用Get方法访问Http地址
		WWW www = new WWW ("http://127.0.0.1/Test/Test.php?username=get&password=12345");
		//等待服务器响应
		yield return www;
		//如果出现错误
		if (www.error != null) {
			m_info = www.error;
			yield return null;
		}
		//获取服务器的响应文本
		m_info = www.text;
	}

	//POST请求
	IEnumerator IPostData ()
	{
		Dictionary<string,string> headers = new Dictionary<string, string> ();
		headers.Add ("Content-Type", "application/x-www-form-urlencoded");
		//将要发送的Post文本
		string data = "username=post&password=6789";
		//将文本转为byte[]
		byte[] bs = System.Text.UTF8Encoding.UTF8.GetBytes (data);
		//向Http服务器提交post数据
		WWW www = new WWW ("http://127.0.0.1/Test/test.php", bs, headers);
		//等待服务器响应
		yield return www;
		if (www.error != null) {
			m_info = www.error;
			yield return null;
		}
		m_info = www.text;
	}

	//获取图片
	IEnumerator IRequestPNG ()
	{
		//将图片读入byte数组
		byte[] bs = m_uploadImage.EncodeToPNG ();
		//创建WWWForm
		WWWForm form = new WWWForm ();
		//添加二进制的Post数据
		form.AddBinaryData ("picture", bs, "screenshot", "image/png");
		//向服务器提交二进制的Post数据
		WWW www = new WWW ("http://127.0.0.1/Test/Test.php", form);
		//等待服务器响应
		yield return www;
		if (www.error != null) {
			m_info = www.error;
			yield return null;
		}
		//收到服务器返回图片数据
		m_downloadTexture = www.texture;
	}

	//下载声音
	IEnumerator DownloadSound ()
	{
		//下载一个声音文件
		WWW www = new WWW ("http://127.0.0.1/Test/music.wav");
		//等待服务器响应
		yield return www;
		if (www.error != null) {
			m_info = www.error;
			yield return null;
		}
		//获得服务器返回的声音文件
		m_downloadClip = www.GetAudioClip (false);
		//播放声音
		GetComponent<AudioSource>().PlayOneShot (m_downloadClip);
	}
}