using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/AI/Nav Module")]
	
	[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
	public class NavModule : MultiModule {
		
		[RequiredField("A float in your Mecanim controller representing movement speed. Must be in range between 0 and 1 where 0 is standing still and 1 is full sprint", RequiredFieldAttribute.RequirementLevels.Recommended)]
		public string animatorMovementFloat;
		[RequiredField("Should we always move towards a specific target?", RequiredFieldAttribute.RequirementLevels.Optional)]
		public GameObject navTarget;
		[RequiredField("How often, in seconds, do we rebuild path data? (Lower numbers = better quality, higher numbers = better speed)", RequiredFieldAttribute.RequirementLevels.Required)]
		public float pathRecalculationInterval = 0.2f;
		[RequiredField("What sound should we play while moving?",RequiredFieldAttribute.RequirementLevels.Recommended)]
		public AudioClip movementSound;

		[System.NonSerialized]
		public Animator anim;
		private float recalcTimer;
		private bool touchingTarget = false;
		private float lastTouchTime;
		private Vector3 lastFramePosition = Vector3.zero;//used to determine the current speed relative to max speed to smoothly transition walk, run, idle animations via movement float
		private Vector3 targetPosition;
		[System.NonSerialized]
		public UnityEngine.AI.NavMeshAgent agent;
		private AudioSource source;
		private float moveRate;
		private float stunDuration = 0;

#if UNITY_EDITOR
		public HelpInfo help = new HelpInfo("This component implements Unity's NavMesh directly, allowing AI to pathfind around easily. You need to bake a navigation mesh for" +
			" your scene before it can work, otherwise you will get an error. Click Window -> Navigation to bake a navmesh." +
			"\n\nTo get started most effectively, we recommend adding some other AI components such as a Guard Module, Melee Module, or others depending on what you want to make." +
			" For example, to make a tank, first create an empty object, and parent a 3D model of a tank to it. Then, add a Guard Module, Nav Module to the base object. Finally," +
			" add a Turret Action to the turret itself, a Targeting Computer (so it can aim at moving rigidbodies correctly), and create an invisible trigger with a Targeting Sensor" +
			" component that sends it's target message to the turret. This creates a tank AI.");
#endif
		[Tooltip("Should we output useful information to the console?")]
		public bool debug = false;

		void Awake () {
			anim = GetComponentInChildren<Animator>();
			source = GetComponent<AudioSource>();
			agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
			if (source != null && movementSound != null) {
				source.clip = movementSound;
			}
		}

		void OnEnable () {
			lastTouchTime = Time.time;
			targetPosition = transform.position;
			recalcTimer = pathRecalculationInterval;
			lastFramePosition = transform.position;
			agent.updateRotation = false;
			stunDuration = 0;
		}

		void Update () {
			stunDuration -= Time.deltaTime;
			if (stunDuration > 0) {
				agent.isStopped = true;
				return;
			}
			if (lastTouchTime > .5f)
				touchingTarget = false;
			if (touchingTarget) {
				transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, navTarget.transform.position - transform.position, agent.angularSpeed * Time.deltaTime,0f),Vector3.up);
			}
			if (navTarget != null) {
				targetPosition = navTarget.transform.position;
				Quaternion targetRot = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, navTarget.transform.position - transform.position, agent.angularSpeed * Time.deltaTime, 0f));
				transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, agent.angularSpeed * Time.deltaTime);
				transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);
			}

			if (agent != null)
				moveRate = ((Vector3.Distance(transform.position, lastFramePosition) / Time.deltaTime) / (agent.speed));

			if (anim != null && !string.IsNullOrEmpty(animatorMovementFloat))
				anim.SetFloat(animatorMovementFloat, moveRate);

			if (source != null && movementSound != null) {
				if (moveRate > 0) {
					if (!source.isPlaying)
						source.Play();
				}
				else {
					source.Stop();
				}
			}

			lastFramePosition = transform.position;

			recalcTimer -= Time.deltaTime;
			if (recalcTimer <= 0)
				BeginPathingTowardsTarget();
		}

		void OnCollisionStay (Collision _collision) {
			lastTouchTime = Time.time;
		}

		void OnCollisionEnter (Collision _collision) {
			lastTouchTime = Time.time;
			if (_collision.gameObject == navTarget)
				touchingTarget = true;
		}

		void OnCollisionExit (Collision _collision) {
			if (_collision.gameObject == navTarget)
				touchingTarget = false;
		}

		void Stun(float duration) {
			if (!gameObject.activeInHierarchy)
				return;
			stunDuration = duration;
			if (debug)
				Debug.Log("NavModule " + gameObject.name + " is being stunned for " + duration + " seconds");
		}

		void SteerForward () {//Used by Guard Module
			if (!gameObject.activeInHierarchy)
				return;
			agent.isStopped = true;
			if(debug)
				Debug.Log ("Nav Module " + gameObject.name + " is steering in direction " + transform.TransformDirection(Vector3.forward) + " up direction: " + transform.up);
			agent.Move((agent.speed * transform.TransformDirection(Vector3.forward)) * Time.deltaTime);
		}

		void BeginPathingTowardsTarget () {
			if (!gameObject.activeInHierarchy)
				return;
			recalcTimer = pathRecalculationInterval;
			if(debug)
				Debug.Log ("Nav Module " + gameObject.name + " is calculating a path towards a target position " + targetPosition);

			agent.SetDestination(targetPosition);
			agent.isStopped = false;
		}

		public void SetTarget (GameObject _target) {
			if (!gameObject.activeInHierarchy)
				return;
			if (_target == null)
				return;
			if(debug)
				Debug.Log ("Nav Module " + gameObject.name + " is setting a target to " + _target.name);
			navTarget = _target;
		}

		public void MoveTo(Vector3 _destination) {
			if (!gameObject.activeInHierarchy)
				return;
			if (debug)
				Debug.Log("Nav Module " + gameObject.name + " moving to " + _destination);
			targetPosition = _destination;
			BeginPathingTowardsTarget();
		}

		[Header("Available Messages")]
		public MessageHelp stopMovingHelp = new MessageHelp("StopMoving","Causes the Nav Module to stop and target this position as it's move target.");
		public void StopMoving () {
			if(debug)
				Debug.Log ("Nav Module " + gameObject.name + " is stopping");
			//navTarget = null;
			targetPosition = transform.position;
		}

		public MessageHelp stopNavigatingHelp = new MessageHelp("StopNavigating","Tells the Nav Mesh Agent to stop immediately, but does not affect the Nav Module directly.");
		public void StopNavigating () {
			agent.isStopped = true;
		}

		void ReturnFromPool() {
			navTarget = null;
			targetPosition = transform.position;
		}
	}
}