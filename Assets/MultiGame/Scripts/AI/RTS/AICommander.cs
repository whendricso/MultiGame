using UnityEngine;
using System.Collections;

public class AICommander : MonoBehaviour {

	public GameObject moveTarget;
	public bool raycastDown = false;
	public LayerMask commandRayMask;

	void Start () {
		Collider _coll = GetComponent<Collider>();
		if (!_coll.isTrigger)
			_coll.isTrigger = true;

		if (moveTarget == null) {
			Debug.LogError("AICommander " + gameObject.name + " needs a move target indicator assigned in the inspector!");
			enabled = false;
			return;
		}
	}
	
	void OnTriggerStay (Collider _other) {
		if (!enabled)
			return;

		if (!raycastDown)
			_other.SendMessage("MoveTo", moveTarget.transform.position, SendMessageOptions.DontRequireReceiver);
		else {
			RaycastHit _hinfo;
			bool _didHit = Physics.Raycast(moveTarget.transform.position, Vector3.down, out _hinfo, Mathf.Infinity,commandRayMask);
			if (_didHit)
				_other.SendMessage("MoveTo", _hinfo.point, SendMessageOptions.DontRequireReceiver);
		}
	}
}
