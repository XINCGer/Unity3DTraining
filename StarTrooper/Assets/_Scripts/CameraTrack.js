#pragma strict

var target : Transform; 
var distance : float = 10.0;
var height : float = 5.0;

private var heightDamping : float = 2.0;
private var rotationDamping : float = 3.0;

function LateUpdate()
{
	if(! target) return;
	var targetRotation : float = target.eulerAngles.y;
	var targetHeight : float = target.position.y + height;
	    
	var curRotation : float = transform.eulerAngles.y;
	var curHeight : float = transform.position.y;

	curRotation = Mathf.LerpAngle(curRotation, targetRotation, rotationDamping * Time.deltaTime);
	curHeight= Mathf.Lerp(curHeight, targetHeight, heightDamping * Time.deltaTime);
	
	var r : Quaternion = Quaternion.Euler(0.0f, curRotation, 0.0f);
	var p : Vector3 = target.position - r * Vector3.forward * distance;
	p.y = curHeight;

	transform.position = p;
	transform.LookAt(target);
}
