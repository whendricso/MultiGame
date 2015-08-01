using UnityEngine;
using System.Collections;

public class GuardModule : MonoBehaviour {

	public GameObject objective;
	[System.NonSerialized]
	public Vector3 objectivePosition;
	[System.NonSerialized]
	public Vector3 persistentMoveTarget;

	public GameObject killTarget;
	public float targetCooldownTime = 6.0f;

	public float guardRange = 30.0f;
	public float guardingRotation = 30.0f;
	public float rotationVariance = 15.0f;
	public float wanderInterval = 15.0f;
	public float wanderWalkTime = 3.0f;
	private float wanderCounter;

	public bool autoLookAround = true;
	public bool wander = true;
	private bool wandering = false;
	private bool returning = false;

	public bool debug = false;

	void Awake () {
		wanderCounter = wanderInterval;
		if (objective == null)
			objectivePosition = transform.position;
		else
			objectivePosition = objective.transform.position;
	}

	void Start () {
		persistentMoveTarget = transform.position;
	}

	void Update () {
		if (killTarget == null && Vector3.Distance(transform.position, objectivePosition) > guardRange)
			StartCoroutine(StopWandering(0));



		//Debug.Log(Vector3.Distance(transform.position, objectivePosition));
		if (objective != null) {
			objectivePosition = objective.transform.position;
			gameObject.SendMessage("MoveTo",objectivePosition, SendMessageOptions.DontRequireReceiver);
		}
		else
			objectivePosition = persistentMoveTarget;

		if (!returning) {
			if (Vector3.Distance(transform.position, objectivePosition) > guardRange && objective == null) {
				if (debug)
					Debug.Log("Guard " + gameObject.name + " has left guard range, returning...");
				returning = true;
				StopAllCoroutines();
				StopWandering(0);
				gameObject.SendMessage("MoveTo",objectivePosition, SendMessageOptions.DontRequireReceiver);
			}
		}
		if (Vector3.Distance(transform.position, objectivePosition) < guardRange) {
			returning = false;
			gameObject.SendMessage("StopMoving", SendMessageOptions.DontRequireReceiver);
		}
		wanderCounter -= Time.deltaTime;
		if (wanderCounter <= 0) {
			wanderCounter = wanderInterval;
			if (autoLookAround || wander)
				ChangeOrientation();
			if(wander) {
				Wander();
			}
		}
		if (wandering && !returning)
			gameObject.SendMessage("SteerForward", true, SendMessageOptions.DontRequireReceiver);
			
	}

	IEnumerator StopWandering (float _delay) {
		yield return new WaitForSeconds(_delay);
		if (killTarget == null) {
			wandering = false;
			returning = true;
		}
		gameObject.SendMessage("MoveTo", objectivePosition, SendMessageOptions.DontRequireReceiver);
	}

	public void Wander () {
		if (objective != null)
			return;
		StopAllCoroutines();
		StartCoroutine(StopWandering(wanderWalkTime));
		wandering = true;
		if (debug)
			Debug.Log("Guard " + gameObject.name + " is now wandering.");
	}

	public void ChangeOrientation () {
		float _y = transform.position.y;
		float _sign = 1.0f;
		float _variance = rotationVariance;
		if (Random.Range(0f,1f) >= .5f)
			_sign *= -1;
		if (Random.Range(0f,1f) >= .5f)
			_variance *= -1;

		_y += (guardingRotation + _variance) * _sign;

		transform.RotateAround(transform.position, Vector3.up, _y);
	}

	public void SetTarget (GameObject _target) {
		if (objective != null)
			return;
		if (debug)
			Debug.Log("Guard " + gameObject.name + " is now targeting " + _target.name);
		killTarget = _target;
		objective = _target;
		SetObjective(_target.transform.position);
	}

	public void ClearTarget () {
		if (debug)
			Debug.Log("Guard " + gameObject.name + " cleared it's target.");
		killTarget = null;
		objective = null;
		StartCoroutine(StopWandering(0));
	}

	public void SetObjective (Vector3 _position) {
		objectivePosition = _position;
	}

	public void MoveTo (Vector3 _position) {
		persistentMoveTarget = _position;
		SetObjective(_position);
	}
	
}
