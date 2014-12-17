using UnityEngine;
using System.Collections;

public class TargetingSensor : MonoBehaviour {
	
	public GameObject messageReceiver;
	private GameObject lastTarget;
	public float retargetTime = 0.75f;
	private bool canRetarget = true;
	public float maxDistance = 25.0f;
	public string[] targetTags;
	
	void Start () {
		if (messageReceiver == null) {
			Debug.LogError("Targeting Sensor requires a Message Receiver to assign a target!");
			enabled = false;
			return;
		}
		collider.isTrigger = true;
	}
	
	void Update () {
		if (lastTarget == null)
			return;
		if (Vector3.Distance(transform.position, lastTarget.transform.position) > maxDistance) {
			lastTarget = null;
			messageReceiver.SendMessage("ClearTarget", SendMessageOptions.DontRequireReceiver);
		}
	}
	
	void OnTriggerEnter (Collider other) {
		if (canRetarget) {
			if (!CheckIsValidTarget(other.gameObject))
				return;
			lastTarget = other.gameObject;
			messageReceiver.SendMessage("SetTarget", lastTarget, SendMessageOptions.DontRequireReceiver);
			canRetarget = false;
			StartCoroutine(ReEnableTargeting());
		}
		
	}
	
	void OnTriggerStay (Collider other) {
		if (!canRetarget)
			return;
		if (!CheckIsValidTarget(other.gameObject))
			return;
		if (lastTarget != null)
			return;
		lastTarget = other.gameObject;
		messageReceiver.SendMessage("SetTarget", lastTarget, SendMessageOptions.DontRequireReceiver);
	}
	
	IEnumerator ReEnableTargeting () {
		yield return new WaitForSeconds(retargetTime);
		canRetarget = true;
	}
	
	bool CheckIsValidTarget (GameObject possibleTarget) {
		bool ret = false;
		foreach (string str in targetTags) {
			if (possibleTarget.tag == str)
				ret = true;
		}
		return ret;
	}
	
}
