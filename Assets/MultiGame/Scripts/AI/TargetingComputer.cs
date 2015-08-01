using UnityEngine;
using System.Collections;

public class TargetingComputer : MonoBehaviour
{
	public Rigidbody mainBody;
	public float shotSpeed;
	public GameObject target;
	public bool autoLook = true;//Automatically look at the target position?
	public bool constrainX = false;

	public bool debug = false;
	
	//velocities
	//Vector3 shooterVelocity = shooter.rigidbody ? shooter.rigidbody.velocity : Vector3.zero;
	//Vector3 targetVelocity = target.rigidbody ? target.rigidbody.velocity : Vector3.zero;
	 
	//calculate intercept
	//Vector3 interceptPoint = FirstOrderIntercept (transform.position, shooterVelocity, shotSpeed, targetPosition, targetVelocity);

	void Start() {
		if (mainBody == null) {
			Debug.LogError("Targeting Computer requires a reference to a parent's rigidbody! " + gameObject.name);
			enabled = false;
			return;
		}
	}
	
	void FixedUpdate() {
		if ((target != null) && (autoLook)) {
			//Debug.Log("Target body: " + target.GetComponent<BodyRegister>().myBody);
			if(target.GetComponent<Rigidbody>() != null)
				transform.LookAt(FirstOrderIntercept(target.transform.GetComponent<Rigidbody>().velocity));
			else
				transform.LookAt(target.transform);
		}
		if (constrainX) {
			transform.eulerAngles = new Vector3(0.0f, transform.eulerAngles.y, transform.eulerAngles.z);
		}
	}
	
	//first-order intercept using absolute target position
	public Vector3 FirstOrderIntercept(Vector3 targetVelocity)
	{
		Vector3 targetRelativeVelocity = targetVelocity - mainBody.velocity;
		float t = FirstOrderInterceptTime (shotSpeed,
										target.transform.position - transform.position,
										targetRelativeVelocity);
		return target.transform.position + t * (targetRelativeVelocity);
	}
	
	//first-order intercept using relative target position
	public float FirstOrderInterceptTime (float shotSpeed, Vector3 targetRelativePosition, Vector3 targetRelativeVelocity)
	{
		float velocitySquared = targetRelativeVelocity.sqrMagnitude;
		if (velocitySquared < 0.001f)
			return 0f;
 
		float a = velocitySquared - shotSpeed * shotSpeed;
 
		//handle similar velocities
		if (Mathf.Abs (a) < 0.001f) {
			float t = -targetRelativePosition.sqrMagnitude / (2f * Vector3.Dot (targetRelativeVelocity, targetRelativePosition));
			return Mathf.Max (t, 0f); //don't shoot back in time
		}
 
		float b = 2f * Vector3.Dot (targetRelativeVelocity, targetRelativePosition),
			c = targetRelativePosition.sqrMagnitude,
			determinant = b * b - 4f * a * c;
 
		if (determinant > 0f) { //determinant > 0; two intercept paths (most common)
			float t1 = (-b + Mathf.Sqrt (determinant)) / (2f * a),
				t2 = (-b - Mathf.Sqrt (determinant)) / (2f * a);
			if (t1 > 0f) {
				if (t2 > 0f)
					return Mathf.Min (t1, t2); //both are positive
				else
					return t1; //only t1 is positive
			} else
				return Mathf.Max (t2, 0f); //don't shoot back in time
		} else if (determinant < 0f) //determinant < 0; no intercept path
			return 0f;
		else //determinant = 0; one intercept path, pretty much never happens
			return Mathf.Max (-b / (2f * a), 0f); //don't shoot back in time
	}
	
	public void SetTarget (GameObject tgt) {
		if (debug)
			Debug.Log("Targeting Computer " + gameObject.name + " is firing at " + tgt.name);
		target = tgt;
	}
	
	public void ClearTarget () {
		if (debug)
			Debug.Log("Targeting Computer " + gameObject.name + " is clearing it's target.");
		target = null;
	}
	
	public void ToggleAutoLook() {
		ToggleAutoLook(!autoLook);
	}
	
	public void ToggleAutoLook (bool val) {
		autoLook = val;
	}
	
}