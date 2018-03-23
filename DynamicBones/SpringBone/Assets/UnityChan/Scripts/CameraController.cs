//CameraController.cs for UnityChan
//Original Script is here:
//TAK-EMI / CameraController.cs
//https://gist.github.com/TAK-EMI/d67a13b6f73bed32075d
//https://twitter.com/TAK_EMI
//
//Revised by N.Kobayashi 2014/5/15 
//Change : To prevent rotation flips on XY plane, use Quaternion in cameraRotate()
//Change : Add the instrustion window
//Change : Add the operation for Mac
//




using UnityEngine;
using System.Collections;

namespace CameraController
{
	enum MouseButtonDown
	{
		MBD_LEFT = 0,
		MBD_RIGHT,
		MBD_MIDDLE,
	};

	public class CameraController : MonoBehaviour
	{
		[SerializeField]
		private Vector3 focus = Vector3.zero;
		[SerializeField]
		private GameObject focusObj = null;

		public bool showInstWindow = true;

		private Vector3 oldPos;

		void setupFocusObject(string name)
		{
			GameObject obj = this.focusObj = new GameObject(name);
			obj.transform.position = this.focus;
			obj.transform.LookAt(this.transform.position);

			return;
		}

		void Start ()
		{
			if (this.focusObj == null)
				this.setupFocusObject("CameraFocusObject");

			Transform trans = this.transform;
			transform.parent = this.focusObj.transform;

			trans.LookAt(this.focus);

			return;
		}
	
		void Update ()
		{
			this.mouseEvent();

			return;
		}

		//Show Instrustion Window
		void OnGUI()
		{
			if(showInstWindow){
				GUI.Box(new Rect(Screen.width -210, Screen.height - 100, 200, 90), "Camera Operations");
				GUI.Label(new Rect(Screen.width -200, Screen.height - 80, 200, 30),"RMB / Alt+LMB: Tumble");
				GUI.Label(new Rect(Screen.width -200, Screen.height - 60, 200, 30),"MMB / Alt+Cmd+LMB: Track");
				GUI.Label(new Rect(Screen.width -200, Screen.height - 40, 200, 30),"Wheel / 2 Fingers Swipe: Dolly");
			}

		}

		void mouseEvent()
		{
			float delta = Input.GetAxis("Mouse ScrollWheel");
			if (delta != 0.0f)
				this.mouseWheelEvent(delta);

			if (Input.GetMouseButtonDown((int)MouseButtonDown.MBD_LEFT) ||
				Input.GetMouseButtonDown((int)MouseButtonDown.MBD_MIDDLE) ||
				Input.GetMouseButtonDown((int)MouseButtonDown.MBD_RIGHT))
				this.oldPos = Input.mousePosition;

			this.mouseDragEvent(Input.mousePosition);

			return;
		}

		void mouseDragEvent(Vector3 mousePos)
		{
			Vector3 diff = mousePos - oldPos;

			if(Input.GetMouseButton((int)MouseButtonDown.MBD_LEFT))
			{
				//Operation for Mac : "Left Alt + Left Command + LMB Drag" is Track
				if(Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftCommand))
				{
					if (diff.magnitude > Vector3.kEpsilon)
						this.cameraTranslate(-diff / 100.0f);
				}
				//Operation for Mac : "Left Alt + LMB Drag" is Tumble
				else if (Input.GetKey(KeyCode.LeftAlt))
				{
					if (diff.magnitude > Vector3.kEpsilon)
						this.cameraRotate(new Vector3(diff.y, diff.x, 0.0f));
				}
				//Only "LMB Drag" is no action.
			}
			//Track
			else if (Input.GetMouseButton((int)MouseButtonDown.MBD_MIDDLE))
			{
				if (diff.magnitude > Vector3.kEpsilon)
					this.cameraTranslate(-diff / 100.0f);
			}
			//Tumble
			else if (Input.GetMouseButton((int)MouseButtonDown.MBD_RIGHT))
			{
				if (diff.magnitude > Vector3.kEpsilon)
					this.cameraRotate(new Vector3(diff.y, diff.x, 0.0f));
			}
				
			this.oldPos = mousePos;	

			return;
		}

		//Dolly
		public void mouseWheelEvent(float delta)
		{
			Vector3 focusToPosition = this.transform.position - this.focus;

			Vector3 post = focusToPosition * (1.0f + delta);

			if (post.magnitude > 0.01)
				this.transform.position = this.focus + post;

			return;
		}

		void cameraTranslate(Vector3 vec)
		{
			Transform focusTrans = this.focusObj.transform;

			vec.x *= -1;

			focusTrans.Translate(Vector3.right * vec.x);
			focusTrans.Translate(Vector3.up * vec.y);

			this.focus = focusTrans.position;

			return;
		}

		public void cameraRotate(Vector3 eulerAngle)
		{
			//Use Quaternion to prevent rotation flips on XY plane
			Quaternion q = Quaternion.identity;
 
			Transform focusTrans = this.focusObj.transform;
			focusTrans.localEulerAngles = focusTrans.localEulerAngles + eulerAngle;

			//Change this.transform.LookAt(this.focus) to q.SetLookRotation(this.focus)
			q.SetLookRotation (this.focus) ;

			return;
		}
	}
}
