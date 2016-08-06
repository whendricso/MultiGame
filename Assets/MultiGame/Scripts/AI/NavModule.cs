using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/AI/Nav Module")]
	
	[RequireComponent(typeof(NavMeshAgent))]
	public class NavModule : MultiModule {

		[System.NonSerialized]
		public Animator anim;
		[RequiredFieldAttribute("A float in your Mecanim controller representing movement speed. Must be in range between 0 and 1 where 0 is standing still and 1 is full sprint", RequiredFieldAttribute.RequirementLevels.Recommended)]
		public string animatorMovementFloat;

		[Tooltip("Should we always move towards a specific target?")]
		public GameObject navTarget;
		private Vector3 targetPosition;
		[HideInInspector]
		public NavMeshAgent agent;
		[HideInInspector]//[Tooltip("If you are using an avoidance rig, drop a reference to it here")]
		public GameObject avoidanceDetector;
		private AvoidanceDetector detector;
		[HideInInspector]//[Tooltip("How fast do we turn to avoid obstacles?")]
		public float avoidanceTurnRate = 6.0f;

		[RequiredFieldAttribute("How often, in seconds, do we rebuild path data? (Lower numbers = better quality, higher numbers = better speed)", RequiredFieldAttribute.RequirementLevels.Required)]
		public float pathRecalculationInterval = 0.2f;
		private float recalcTimer;
		private bool touchingTarget = false;
		private float lastTouchTime;
		private Vector3 lastFramePosition = Vector3.zero;

		public HelpInfo help = new HelpInfo("This component implements Unity's NavMesh directly, allowing AI to pathfind around easily. You need to bake a navigation mesh for" +
			" your scene before it can work, otherwise you will get an error. Click Window -> Navigation to bake a navmesh." +
			"\n\nTo get started most effectively, we recommend adding some other AI components such as a Guard Module, Melee Module, or others depending on what you want to make." +
			" For example, to make a tank, first create an empty object, and parent a 3D model of a tank to it. Then, add a Guard Module, Nav Module to the base object. Finally," +
			" add a Turret Action to the turret itself, a Targeting Computer (so it can aim at moving rigidbodies correctly), and create an invisible trigger with a Targeting Sensor" +
			" component that sends it's target message to the turret. This creates a tank AI.");

		[Tooltip("WARNING! SLOW OPERATION Should we output useful information to the console?")]
		public bool debug = false;

		void Awake () {
			anim = GetComponentInChildren<Animator>();

			lastTouchTime = Time.time;
			if (avoidanceDetector != null)
				detector = avoidanceDetector.GetComponent<AvoidanceDetector>();
			recalcTimer = pathRecalculationInterval;
			agent = GetComponent<NavMeshAgent>();
			targetPosition = transform.position;
	//		if(!agent.isOnNavMesh)
	//			Debug.LogWarning("Agent " + gameObject.name + " is not bound to any nav mesh! Traversal may fail.");
		}

		void Start () {
			lastFramePosition = transform.position;
		}

		void Update () {
			if (lastTouchTime > .5f)
				touchingTarget = false;
			if (touchingTarget) {
				transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, navTarget.transform.position - transform.position, agent.angularSpeed * Time.deltaTime,0f));
			}
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

			if (anim != null && !string.IsNullOrEmpty(animatorMovementFloat))
				anim.SetFloat(animatorMovementFloat, ((Vector3.Distance(transform.position, lastFramePosition) / Time.deltaTime) / (agent.speed) ));

			lastFramePosition = transform.position;

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

		public MessageHelp stopMovingHelp = new MessageHelp("StopMoving","Causes the Nav Module to stop and target this position as it's move target.");
		public void StopMoving () {
			if(debug)
				Debug.Log ("Nav Module " + gameObject.name + " is stopping");
			navTarget = null;
			targetPosition = transform.position;
		}

		public void MoveTo (Vector3 _destination) {
			if(debug)
				Debug.Log ("Nav Module " + gameObject.name + " moving to " + _destination);
			targetPosition = _destination;
			BeginPathingTowardsTarget();
		}

		public MessageHelp stopNavigatingHelp = new MessageHelp("StopNavigating","Tells the Nav Mesh Agent to stop immediately, but does not affect the Nav Module directly.");
		public void StopNavigating () {
			agent.Stop();
		}
	}
}