#pragma strict

var rotateSpeed : float = 10.0f;

function Update () 
{
	transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
}
