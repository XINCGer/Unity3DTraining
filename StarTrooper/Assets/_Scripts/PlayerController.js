#pragma strict

var turnSpeed : float = 10.0f; // 转弯速度
var sensitivity : float = 1.0f; // 敏感度，定义了旋转时Lerp插值的时间
var forwardForce : float = 50.0f; //前向力的大小

private var maxTurnLean : float = 50.0f;
private var maxTilt : float = 50.0f;

private var normalizedSpeed : float = 0.2f; 
private var euler : Vector3 = Vector3.zero;

var guiSpeedElement : Transform;

function Start () 
{
	Screen.orientation = ScreenOrientation.AutoRotation;
	guiSpeedElement.position = new Vector3 (30, normalizedSpeed * Screen.height, 0);
}

function Update () 
{
    for(var touch : Touch in Input.touches)
    {
        if(touch.phase == TouchPhase.Moved)
        {
            normalizedSpeed = touch.position.y / Screen.height;
            guiSpeedElement.position = new Vector3 (30, touch.position.y, 0);
        }
    }
}

function FixedUpdate()
{
    GetComponent.<Rigidbody>().AddRelativeForce(0, 0, normalizedSpeed * forwardForce);
    var acc : Vector3 = Input.acceleration;
    euler.y += acc.x * turnSpeed;
    euler.z = Mathf.Lerp(euler.z, -acc.x * maxTurnLean, 0.2);
    euler.x = Mathf.Lerp(euler.x, acc.y * maxTilt, 0.2);
    var rot : Quaternion = Quaternion.Euler(euler);
    transform.rotation = Quaternion.Lerp(transform.rotation, rot, sensitivity);
}
