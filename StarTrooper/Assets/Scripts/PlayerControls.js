var turnSpeed : float = 10.0;
var maxTurnLean : float = 50.0;
var maxTilt : float = 50.0;

var sensitivity : float = 10.0;

var forwardForce : float = 1.0;
var guiSpeedElement : Transform;

private var normalizedSpeed : float = 0.2;
private var euler : Vector3 = Vector3.zero;

var horizontalOrientation : boolean = true;

function Awake () {
	if (horizontalOrientation)
	{
		 Screen.orientation = ScreenOrientation.LandscapeLeft;
	}
	else
	{
		 Screen.orientation = ScreenOrientation.Portrait;
	}

	guiSpeedElement.position = new Vector3 (0, normalizedSpeed, 0);
}

function Update () {
	for (var evt : Touch in Input.touches)
	{
		if (evt.phase == TouchPhase.Moved)
		{
			normalizedSpeed = evt.position.y / Screen.height;
			guiSpeedElement.position = new Vector3 (0, normalizedSpeed, 0);
		}
	}
}


function FixedUpdate () {
	GetComponent.<Rigidbody>().AddRelativeForce(0, 0, normalizedSpeed * forwardForce);
	
	var accelerator : Vector3 = Input.acceleration;
	
	// Rotate turn based on acceleration		
	euler.y += accelerator.x * turnSpeed;
	// Since we set absolute lean position, do some extra smoothing on it
	euler.z = Mathf.Lerp(euler.z, -accelerator.x * maxTurnLean, 0.2);

	// Since we set absolute lean position, do some extra smoothing on it
	euler.x = Mathf.Lerp(euler.x, accelerator.y * maxTilt, 0.2);
	
	// Apply rotation and apply some smoothing
	var rot : Quaternion = Quaternion.Euler(euler);
	transform.rotation = Quaternion.Lerp (transform.rotation, rot, sensitivity);
}
