/*
ProjectName: SpaceShooter
Author: 马三小伙儿
Blog: http://www.cnblogs.com/msxh/
Github:https://github.com/XINCGer
Date: 2016/08/24
*/
using UnityEngine;
using System.Collections;

//Boundary用于管理飞船的活动边界值
[System.Serializable]
public class Boundary
{
	public float xMin, xMax, zMin, zMax;

}

public class PlayerController : MonoBehaviour
{
	public float speed = 5.0f;	//飞船的移动速度
	public Boundary boundary;	//控制飞船不飞出画面
	public float tilt=4.0f;		//倾斜系数
	public float fireRate=0.5f;	 //	发射的时间间隔
	public GameObject shot;		//电光弹的预制体
	public Transform shotSpawn; //存储电光弹的transform属性
	private float nextFire =0.0f;	//表示下次可以发射的最早时间

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetButton ("Fire1") && Time.time > nextFire) {
			nextFire=Time.time+fireRate;
			Instantiate(shot,shotSpawn.position,shotSpawn.rotation);
			GetComponent<AudioSource>().Play();	//获取绑定在Player对象上的AudioSource组件，并且调用play方法播放
		}
	}

	void FixedUpdate ()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal"); 	//得到水平方向的输入
		float moveVetical = Input.GetAxis ("Vertical");		//得到垂直方向的输入
		//用上面的水平方向(X轴)和垂直方向(Z轴)的输入创建一个Vector3变量，作为刚体速度
		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVetical);
		Rigidbody rb = GetComponent<Rigidbody> ();
		if (rb != null) {
			rb.velocity = movement * speed;
			rb.position = new Vector3 (Mathf.Clamp (rb.position.x, boundary.xMin, boundary.xMax), 0.0f, Mathf.Clamp (rb.position.z, boundary.zMin, boundary.zMax));
			//飞船左右飞行的时候是绕着z轴旋转，向左运动的时候x轴的速度为负值，而此时旋转角度（逆时针）
			//应为正值，所以要乘一个负的系数
			rb.rotation=Quaternion.Euler(0.0f,0.0f,rb.velocity.x*-tilt);
		}
	}
}
