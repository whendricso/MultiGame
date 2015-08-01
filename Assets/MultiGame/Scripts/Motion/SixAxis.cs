using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class SixAxis : MonoBehaviour {

	public float forwardThrust = 10.0f;
	public float sidewaysThrust = 10.0f;
	public float reverseThrust = 10.0f;
	public float upwardThrust = 10.0f;
	public float downwardThrust = 10.0f;
	public float deadzone = 0.25f;
	public KeyCode upKey = KeyCode.Space;
	public KeyCode downKey = KeyCode.LeftShift;
	public bool useLateUpdate = false;

	private Vector2 stickInput = Vector2.zero;
	private bool goUp = false;
	private bool goDown = false;
	private Vector3 thrustVec = Vector3.zero;

	void Update () {
		if (!useLateUpdate)
			UpdateInputState();
	}

	void LateUpdate () {
		if (useLateUpdate)
			UpdateInputState();
	}

	void UpdateInputState () {
		stickInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		if(stickInput.magnitude < deadzone)
			stickInput = Vector2.zero;
		else
			stickInput = stickInput.normalized * ((stickInput.magnitude - deadzone) / (1 - deadzone));
		
		if (Input.GetKeyDown(upKey))
			goUp = true;
		if (Input.GetKeyUp(upKey))
			goUp = false;
		if (Input.GetKeyDown(downKey))
			goDown = true;
		if (Input.GetKeyUp(downKey))
			goDown = false;
	}
	
	void FixedUpdate () {
		if (stickInput.y > 0.0f)
			thrustVec.z = forwardThrust * stickInput.y;
		if (stickInput.y < 0.0f)
			thrustVec.z = reverseThrust * stickInput.y;
		if (stickInput.y == 0.0f)
			thrustVec.z = 0.0f;
		if (goUp)
			thrustVec.y = upwardThrust;
		if (goDown)
			thrustVec.y = -downwardThrust;
		if (!goUp && !goDown)
			thrustVec.y = 0.0f;
		thrustVec.x = stickInput.x * sidewaysThrust;


		GetComponent<Rigidbody>().AddRelativeForce(thrustVec);
		
	}
}
