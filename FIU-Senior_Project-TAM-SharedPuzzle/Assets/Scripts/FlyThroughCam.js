//#pragma strict

//this class is used to allow keyboard movement in editor

//allow me to adjust the speed
var lookSpeed = 15.0;
var moveSpeed = 15.0;
 
var rotationX = 0.0;
var rotationY = 0.0;
 
function Update ()
{
    //use mouse to look around
	rotationX += Input.GetAxis("Mouse X")*lookSpeed;
	rotationY += Input.GetAxis("Mouse Y")*lookSpeed;
	rotationY = Mathf.Clamp (rotationY, -90, 90);
 
	transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
	transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
 
    //uyse keyboard to move
	transform.position += transform.forward*moveSpeed*Input.GetAxis("Vertical");
	transform.position += transform.right*moveSpeed*Input.GetAxis("Horizontal");
}