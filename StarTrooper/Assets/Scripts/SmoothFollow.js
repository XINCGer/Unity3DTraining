/*
This camera smoothes out rotation around the y-axis and height.
Horizontal Distance to the target is always fixed.

There are many different ways to smooth the rotation but doing it this way gives you a lot of control over how the camera behaves.

For every of those smoothed values we calculate the wanted value and the current value.
Then we smooth it using the Lerp function.
Then we apply the smoothed values to the transform's position.
*/

// The target we are following
var target : Transform;
// The distance in the x-z plane to the target
var distance : float = 10.0;
// the height we want the camera to be above the target
var height : float = 5.0;
// How much we 
var heightDamping : float = 2.0;
var rotationDamping : float = 3.0;


function LateUpdate (){
	// Early out if we don't have a target
	if (!target)
		return;
	
	// Calculate the current rotation angles
	var wantedRotationAngle : float = target.eulerAngles.y;
	var wantedHeight : float = target.position.y + height;
		
	var currentRotationAngle : float = transform.eulerAngles.y;
	var currentHeight : float = transform.position.y;
	
	// Damp the rotation around the y-axis
	var dt : float = Time.deltaTime;
	currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * dt);//Time.GetMyDeltaTime());
	
	// Damp the height
	currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * dt);//Time.GetMyDeltaTime());
	//System.Console.WriteLine("dt: {0}", dt);//Time.GetMyDeltaTime());
	//Debug.Log("dt: " + Time.deltaTime);
	
	// Convert the angle into a rotation
	var currentRotation : Quaternion = Quaternion.Euler (0, currentRotationAngle, 0);
	
	// Set the position of the camera on the x-z plane to:
	// distance meters behind the target
	//transform.position = target.position;
	var pos : Vector3 = target.position - currentRotation * Vector3.forward * distance;
	pos.y = currentHeight;
	
	// Set the height of the camera
	transform.position = pos;
	
	// Always look at the target
	transform.LookAt (target);
}