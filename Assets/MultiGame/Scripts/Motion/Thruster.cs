using UnityEngine;
using System.Collections;

public class Thruster : MonoBehaviour {

	public bool thrusting = false;
	public Vector3 thrust;
	public string axis = "Vertical";
	public Space space = Space.Self;
	public GameObject target;

	public bool useInputAxis = false;
	public float inputSensitivity = 0.2f;

	private Rigidbody rigid;
	private bool useTargetRigidbody = false;

	void Start () {
		if (GetComponent<Rigidbody>() == null && target == null) {
			Debug.LogError("Thruster " + gameObject.name + "must have attached rigidbody or a target with a rigidbody to work!");
			enabled = false;
			return;
		}
		if (target.GetComponent<Rigidbody>() == null) {
			Debug.LogError("Thruster " + gameObject.name + "must have attached rigidbody or a target with a rigidbody to work!");
			enabled = false;
			return;
		}
		if (target != null) {
			useTargetRigidbody = true;
			rigid = target.GetComponent<Rigidbody>();
		}
	}

	void FixedUpdate () {
		if (!useInputAxis) {
			if (thrusting) {
				if (space == Space.Self) {
					if (!useTargetRigidbody)
						GetComponent<Rigidbody>().AddRelativeForce(thrust, ForceMode.Force);
					else
						rigid.AddRelativeForce(thrust, ForceMode.Force);
						

				}
				else {
					if (!useTargetRigidbody)
						GetComponent<Rigidbody>().AddForce(thrust, ForceMode.Force);
					else
						rigid.AddForce(thrust, ForceMode.Force);
						
				}
			}
		}
		else {//use input axis
			if ( Mathf.Abs(Input.GetAxis(axis)) > inputSensitivity) {
				if (space == Space.Self) {
					if (!useTargetRigidbody)
						GetComponent<Rigidbody>().AddRelativeForce(thrust * Input.GetAxis(axis), ForceMode.Force);
					else
						rigid.AddRelativeForce(thrust * Input.GetAxis(axis), ForceMode.Force);
						
				}
				else {
					if (!useTargetRigidbody)
						GetComponent<Rigidbody>().AddForce(thrust * Input.GetAxis(axis), ForceMode.Force);
					else
						rigid.AddForce(thrust * Input.GetAxis(axis), ForceMode.Force);
						
				}
			}
		}
	}

	public void BeginThrust () {
		thrusting = true;
	}

	public void EndThrust () {
		thrusting = false;
	}

	public void ThrustAmount (float scalar) {
		if (scalar != 0.0f) {
			if (space == Space.Self)
				GetComponent<Rigidbody>().AddRelativeForce(thrust * scalar);
			else
				GetComponent<Rigidbody>().AddForce(thrust * scalar, ForceMode.Force);
		}
	}

	public void ThrustVector (Vector3 input) {
		Debug.Log("Thrust " + input);
		if (space == Space.Self)
			GetComponent<Rigidbody>().AddRelativeForce(new Vector3( input.x * thrust.x, input.y * thrust.y, input.z * thrust.z));
		else
			GetComponent<Rigidbody>().AddForce(new Vector3( input.x * thrust.x, input.y * thrust.y, input.z * thrust.z));
	}
}
//Copyright 2014 William Hendrickson
