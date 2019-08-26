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
		public float footstepInterval = 0;
		[Range(0,1)]
		public float soundVariance = .05f;
		[Tooltip("If no nearest nav target is found, should we send a message?")]
		public MessageManager.ManagedMessage nearestNotFound;
		[RequiredField("How close should we stop if hunting? Increase this to just inside weapon range for ranged attackers.")]
		public float huntingDistance = .5f;

		/// <summary>
		/// If this is defined, we will try to reach Hunting Distance away from target on the Nav Mesh
		/// </summary>
		private bool hunting;
		[System.NonSerialized]
		public Animator anim;
		private float startingPitch = 1;
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
		private float intervalCounter;
		private Quaternion targetRot;

#if UNITY_EDITOR
		public HelpInfo help = new HelpInfo("This component implements Unity's NavMesh directly, allowing AI to pathfind around easily. You need to bake a navigation mesh for" +
			" your scene before it can work, otherwise you will get an error. Click Window -> Navigation to bake a navmesh." +
			"\n\nTo get started most effectively, we recommend adding some other AI components such as a Guard Module, Melee Module, or others depending on what you want to make." +
			" For example, to make a tank, first create an empty object, and parent a 3D model of a tank to it. Then, add a Guard Module, Nav Module to the base object. Finally," +
			" add a Turret Action to the turret itself, a Targeting Computer (so it can aim at moving rigidbodies correctly), and create an invisible trigger with a Targeting Sensor" +
			" component that sends it's target message to the turret. This creates a tank AI.", "https://youtu.be/ClrtITChqkA");
#endif
		[Tooltip("Should we output useful information to the console?")]
		public bool debug = false;

		void OnValidate() {
			MessageManager.UpdateMessageGUI(ref nearestNotFound, gameObject);
		}

		void Awake () {
			if (nearestNotFound.target == null)
				nearestNotFound.target = gameObject;
			anim = GetComponentInChildren<Animator>();
			source = GetComponent<AudioSource>();
			agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
			if (source != null && movementSound != null) {
				startingPitch = source.pitch;
				source.clip = movementSound;
			}
			if (movementSound != null && footstepInterval == 0)
				footstepInterval = movementSound.length;
			intervalCounter = footstepInterval;
		}

		void OnEnable () {
			lastTouchTime = Time.time;
			targetPosition = transform.position;
			if (anim != null)
				anim.applyRootMotion = false;
			recalcTimer = pathRecalculationInterval;
			lastFramePosition = transform.position;
			//agent.updateRotation = false;
			stunDuration = 0;
		}

		void Update () {
			if (!agent.enabled)
				return;
			stunDuration -= Time.deltaTime;
			intervalCounter -= Time.deltaTime;

			if (stunDuration > 0) {
				agent.isStopped = true;
				return;
			}

			if (navTarget != null) {
				targetPosition = navTarget.transform.position;
				targetRot = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, targetPosition - transform.position, agent.angularSpeed * Time.deltaTime, 0f));

				if (!agent.updateRotation) {
					if (touchingTarget) {
						transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, targetPosition - transform.position, agent.angularSpeed * Time.deltaTime, 0f), Vector3.up);
					} else {
						transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, agent.angularSpeed * Time.deltaTime);
						transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);
					}
				}
			}

			if (lastTouchTime - Time.time > .5f)
				touchingTarget = false;
			
			if (agent != null)
				moveRate = ((Vector3.Distance(transform.position, lastFramePosition) / Time.deltaTime) / (agent.speed));

			if (anim != null && !string.IsNullOrEmpty(animatorMovementFloat))
				anim.SetFloat(animatorMovementFloat, moveRate);

			if (source != null && movementSound != null) {
				if (moveRate > 0) {
					StartCoroutine(RandomizePitch());
					if (intervalCounter < 0) {
						source.Play();
						intervalCounter = footstepInterval;
					}
					//if (!source.isPlaying)
					//	source.Play();
				}
				else {
					StopCoroutine(RandomizePitch());
					source.Stop();
				}
			}


			recalcTimer -= Time.deltaTime;
			if (recalcTimer <= 0)
				BeginPathingTowardsTarget();
		}

		private IEnumerator RandomizePitch() {
			source.pitch = startingPitch + Random.Range(-soundVariance, soundVariance);
			yield return new WaitForSeconds(footstepInterval);
			StartCoroutine(RandomizePitch());
		}

		private void LateUpdate() {
			lastFramePosition = transform.position;
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
			if (_target == null)//why did I write this?
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

		public void Hunt() {
			hunting = true;
		}

		public void StopHunting() {
			hunting = false;
		}

		[Header("Available Messages")]
		public MessageHelp stopNavigatingHelp = new MessageHelp("StopNavigating","Tells the Nav Mesh Agent to stop immediately, but does not affect the Nav Module directly.");
		public void StopNavigating () {
			navTarget = gameObject;
			StopMoving();
		}

		//public MessageHelp stopMovingHelp = new MessageHelp("StopMoving","Causes the Nav Module to stop and target this position as it's move target.");
		private void StopMoving() {
			if (debug)
				Debug.Log("Nav Module " + gameObject.name + " is stopping");
			//navTarget = null;
			targetPosition = transform.position;
			agent.isStopped = true;
		}

		public MessageHelp stunHelp = new MessageHelp("Stun","Causes the agent to be unable to move for a set duration",3,"Time in seconds that the agent should be unable to move");
		void Stun(float duration) {
			if (!gameObject.activeInHierarchy)
				return;
			stunDuration = duration;
			if (debug)
				Debug.Log("NavModule " + gameObject.name + " is being stunned for " + duration + " seconds");
		}

		public MessageHelp navToNearestHelp = new MessageHelp("NavToNearest","Finds the nearest object with a given tag, and if found navigates to it.",4,"The tag of the object we wish to move towards");
		public void NavToNearest(string _tag) {
			GameObject _closest = FindClosestByTag(_tag);
			if (debug)
				Debug.Log("NavModule " + gameObject.name + " is going to the closest object tagged " + _tag + " and found object " + _closest);
			if (_closest != null) {
				SetTarget(_closest);
				MoveTo(_closest.transform.position);
			} else {
				MessageManager.Send(nearestNotFound);
			}
		}
		public MessageHelp enableAgentHelp = new MessageHelp("EnableAgent","Sets the Nav Mesh Agent attached to this object to be enabled, allowing it to control the object's position.");
		public void EnableAgent() {
			agent.enabled = true;
		}

		public MessageHelp disabeAgentHelp = new MessageHelp("DisableAgent","Sets the Nav Mesh Agent attached to this object to be disabled, so that it will not control the object's position.");
		public void DisableAgent() {
			agent.enabled = false;
		}

		public MessageHelp toggleAgentHelp = new MessageHelp("ToggleAgent","Swaps the current enabled state of the Nav Mesh Agent");
		public void ToggleAgent() {
			agent.enabled = !agent.enabled;
		}

		public MessageHelp faceTargetHelp = new MessageHelp("FaceTarget","Causes the AI to always face it's target, no matter what direction it's moving in");
		public void FaceTarget() {
			agent.updateRotation = false;
		}

		public MessageHelp faceMoveDirectionHelp = new MessageHelp("FaceMoveDirection","Causes the AI to always face it's move direction, no matter the direction of the target");
		public void FaceMoveDirection() {
			agent.updateRotation = true;
		}

		void ReturnFromPool() {
			agent.enabled = true;
			navTarget = null;
			targetPosition = transform.position;
		}
	}
}