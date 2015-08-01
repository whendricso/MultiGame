using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class NavModule : MonoBehaviour {

	public GameObject navTarget;
	private Vector3 targetPosition;
	[HideInInspector]
	public NavMeshAgent agent;
	public GameObject avoidanceDetector;
	private AvoidanceDetector detector;
	public float avoidanceTurnRate = 6.0f;

	public float pathRecalculationInterval = 0.2f;
	private float recalcTimer;
	private bool touchingTarget = false;
	private float lastTouchTime;

	public bool debug = false;

	void Awake () {
		lastTouchTime = Time.time;
		if (avoidanceDetector != null)
			detector = avoidanceDetector.GetComponent<AvoidanceDetector>();
		recalcTimer = pathRecalculationInterval;
		agent = GetComponent<NavMeshAgent>();
		targetPosition = transform.position;
//		if(!agent.isOnNavMesh)
//			Debug.LogWarning("Agent " + gameObject.name + " is not bound to any nav mesh! Traversal may fail.");
	}

	void Update () {
		if (lastTouchTime > .5f)
			touchingTarget = false;
		if(navTarget != null)
			targetPosition = navTarget.transform.position;
//		if (pathRecalculationInterval <= 0) {
//			SteerTowardsTarget();
//			return;
//		}
		if (Vector3.Distance(transform.position, targetPosition) < agent.stoppingDistance) {
			targetPosition = transform.position;
			return;
		}

		recalcTimer -= Time.deltaTime;
		if (recalcTimer <= 0)
			BeginPathingTowardsTarget();
	}

	void OnCollisionStay (Collision _collision) {
		lastTouchTime = Time.time;
	}

	void OnCollisionEnter (Collision _collision) {
		if (_collision.gameObject == navTarget)
			touchingTarget = true;
	}

	void OnCollisionExit (Collision _collision) {
		if (_collision.gameObject == navTarget)
			touchingTarget = false;
	}

	void SteerTowardsTarget () {
		if (Vector3.Distance(transform.position, targetPosition) <= agent.stoppingDistance || (agent.stoppingDistance == 0 && Vector3.Distance(transform.position, targetPosition) <= 1f))
			return;
		if(navTarget == null) {
			targetPosition = transform.position;
			return;
		}
		if(debug)
			Debug.Log ("Nav Module " + gameObject.name + " is steering towards target " + navTarget);
		transform.LookAt(targetPosition, Vector3.up);
		transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, transform.eulerAngles.z);
		SteerForward(false);
		if(debug)
			Debug.Log("agent.move is being called for " + gameObject.name);
	}

	void SteerForward (bool _avoidObstacles) {
		agent.Stop();
		if (_avoidObstacles)
			AvoidObstacle();
		if(debug)
			Debug.Log ("Nav Module " + gameObject.name + " is steering in direction " + transform.TransformDirection(Vector3.forward) + " up direction: " + transform.up);
		agent.Move((agent.speed * transform.TransformDirection(Vector3.forward)) * Time.deltaTime);
	}

	void AvoidObstacle () {
		if (detector == null)
			return;

		if(debug)
			Debug.Log("Nav Module " + gameObject.name + " is avoiding an obstacle.");

		if (detector.leftDetected) {
			transform.RotateAround(transform.position, Vector3.up, -avoidanceTurnRate);
			return;
		}
		if (detector.rightDetected) {
			transform.RotateAround(transform.position, Vector3.up, avoidanceTurnRate);
			return;
		}
		if (detector.centerDetected) {
			transform.RotateAround(transform.position, Vector3.up, 180f);
		}
	}

	void BeginPathingTowardsTarget () {
		recalcTimer = pathRecalculationInterval;
		if(debug)
			Debug.Log ("Nav Module " + gameObject.name + " has begun pathing towards a target position " + targetPosition);
//		if (!agent.isOnNavMesh)
//			return;
		agent.SetDestination(targetPosition);
		agent.Resume();
	}

	public void SetTarget (GameObject _target) {
		if (_target == null)
			return;
		if(debug)
			Debug.Log ("Nav Module " + gameObject.name + " is setting a target to " + _target.name);
		navTarget = _target;
	}

	public void StopMoving () {
		if(debug)
			Debug.Log ("Nav Module " + gameObject.name + " is stopping");
		navTarget = null;
		targetPosition = transform.position;
	}

	public void MoveTo (Vector3 _destination) {
		if(debug)
			Debug.Log ("Nav Moduel " + gameObject.name + " moving to " + _destination);
		targetPosition = _destination;
		BeginPathingTowardsTarget();
	}

	public void StopNavigating () {
		agent.Stop();
	}
}
