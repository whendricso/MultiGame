using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class InputTorqueMotor : MonoBehaviour {

	public string axis = "Vertical";
	public float power = 10.0f;
	public Vector3 outputAxes = Vector3.right;

	void Start () {
	
	}
	
	void FixedUpdate () {
		rigidbody.AddRelativeTorque((power * Input.GetAxis(axis)) * outputAxes);
	}
}
