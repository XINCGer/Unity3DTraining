var xMotion : boolean = true;
var yMotion : boolean = true;
var zMotion : boolean = true;

@script AddComponentMenu("Physics/Constrain Motion")
function FixedUpdate () {
	var relativeSpeed : Vector3 = transform.InverseTransformDirection (GetComponent.<Rigidbody>().velocity);
	
	if (!xMotion)
		relativeSpeed.x = 0;
	if (!yMotion)
		relativeSpeed.y = 0;
	if (!zMotion)
		relativeSpeed.z = 0;
		
	GetComponent.<Rigidbody>().AddRelativeForce (-relativeSpeed, ForceMode.VelocityChange);
}

