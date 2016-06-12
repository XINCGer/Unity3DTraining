using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HiScoreApp : MonoBehaviour
{

	// 按钮位置
	public Rect m_uploadBut;
	public Rect m_downLoadBut;

	// 输入框位置
	public Rect m_nameLablePos;
	public Rect m_scoreLablePos;
	public Rect m_nameTxtField;
	public Rect m_scoreTxtField;

	// 滑动框位置
	public Rect m_scrollViewPosition;
	public Vector2 m_scrollPos;
	public Rect m_scrollView;

	// 网格位置
	public Rect m_gridPos;
	public string[] m_hiscores;

	// 用户名
	protected string m_name = "";

	// 得分
	protected string m_score = "";

	// Use this for initialization
	void Start ()
	{

		m_hiscores = new string[20];
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnGUI ()
	{

		m_name = GUI.TextField (m_nameTxtField, m_name);
		m_score = GUI.TextField (m_scoreTxtField, m_score);

		GUI.Label (m_nameLablePos, "用户名");
		GUI.Label (m_scoreLablePos, "得分");

		// 上传分数
		if (GUI.Button (m_uploadBut, "上传")) {
			StartCoroutine (UploadScore (m_name, m_score));
			m_name = string.Empty;//清空UI中的输入
			m_score = string.Empty;//清空UI中的输入
		}

		// 下载分数
		if (GUI.Button (m_downLoadBut, "下载")) {
			StartCoroutine (DownloadScores (m_name, m_score));
		}

		m_scrollPos = GUI.BeginScrollView (m_scrollViewPosition, m_scrollPos, m_scrollView);

		m_gridPos.height = m_hiscores.Length * 30;

		// 显示分数排行榜
		GUI.SelectionGrid (m_gridPos, 0, m_hiscores, 1);

		GUI.EndScrollView ();
	}
	//上传分数
	IEnumerator UploadScore (string name, string score)
	{
		WWWForm form = new WWWForm ();
		form.AddField ("username", name);//传入用户名
		form.AddField ("score", score);//传入分数
		WWW www = new WWW ("127.0.0.1/Test/UploadScore.php", form);
		yield return www;
		if (www.error != null) {
			Debug.LogError (www.error);
		}
	}

	//下载服务器返回的数据，使用MiniJson提供的功能解析
	IEnumerator DownloadScores (string name, string score)
	{
		WWW www = new WWW ("127.0.0.1/Test/DownloadScores.php");
		yield return www;
		if (www.error != null) {
			Debug.LogError (www.error);
		} else {
			//将服务器返回的数据使用MiniJson解析为一个键值对应的Dictionary
			var dict = MiniJSON.Json.Deserialize (www.text) as Dictionary <string,object>;
			//将Dictionary中的所有值取出来，并解析为一个UserData对象
			int index = 0;
			foreach (object v in dict.Values) {
				UserData user = new UserData ();
				MiniJSON.Json.ToObject (user, v);
				m_hiscores [index] = user.username + ":" + user.score;
				index++;
			}
			//Debug.Log(www.text.ToString());
		}
	}

	//UserData类，用来保存服务器返回的数据
	//它的字段名称一定要和服务器返回的Json格式数据的键名一致
	public class UserData
	{
		public int id;
		public string username;
		public int score;
	}
}
