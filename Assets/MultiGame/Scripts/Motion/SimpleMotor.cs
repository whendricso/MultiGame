using UnityEngine;
using System.Collections;

public class SimpleMotor : MonoBehaviour {

	public Vector3 impetus = Vector3.zero;
	public Vector3 localImpetus = Vector3.zero;
	
	void Update () {
		transform.position += impetus * Time.deltaTime;
		transform.localPosition += localImpetus * Time.deltaTime;
	}
}
