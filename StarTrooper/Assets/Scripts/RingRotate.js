var rotateSpeed : float = 5.0;

function Update () {
	transform.Rotate (0.0, 0.0, rotateSpeed * Time.deltaTime);
}