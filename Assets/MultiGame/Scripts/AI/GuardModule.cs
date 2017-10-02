using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/AI/Guard Module")]
	public class GuardModule : MultiModule {

		[Header("Objectives")]
		[Tooltip("What are we trying to guard?")]
		public GameObject objective;
		[System.NonSerialized]
		public Vector3 objectivePosition;
		[System.NonSerialized]
		public Vector3 persistentMoveTarget;

		[Tooltip("Who you wan' me kill? (Assigns a target that already exists in the scene, good for scripted events)")]
		public GameObject killTarget;
		
		[Header("Behavior Modifiers")]
		[Tooltip("How often do I think about changing targets?")]
		public float targetCooldownTime = 6.0f;
		[Tooltip("How far from my objective can I travel?")]
		public float guardRange = 30.0f;
		[Tooltip("How far do I turn while guarding a position?")]
		public float guardingRotation = 30.0f;
		[Tooltip("How varied, in degrees, is that rotation?")]
		public float rotationVariance = 15.0f;
		[Tooltip("How many seconds between patrol times?")]
		public float wanderInterval = 15.0f;
		[Tooltip("How long do I walk for?")]
		public float wanderWalkTime = 3.0f;
		private float wanderCounter;

		[Tooltip("Do I change orientation automatically?")]
		public bool autoLookAround = true;
		[Tooltip("Do I wander about the area?")]
		public bool wander = true;
		private bool wandering = false;
		private bool returning = false;

		public HelpInfo help = new HelpInfo("Add to an object representing an AI guard. This should be an empty object with a 3D model of a guard parented to it." +
			"\nBy \"Guard\" we mean an AI that should stay in a given area or near a given object, and attack enemies that provoke it. To use this effectively, we recommend" +
			" also adding a NavModule or similar, since this guard will probably need to get around. Also, add some sort of combat component, or attach a turret so it can do harm.");

		[Tooltip("Should I output information to the console?")]
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
		[Header("Available Messages")]
		public MessageHelp wanderHelp = new MessageHelp("Wander", "Causes the Guard Module to immediately begin wandering to a new location.");
		public void Wander () {
			if (objective != null)
				return;
			StopAllCoroutines();
			StartCoroutine(StopWandering(wanderWalkTime));
			wandering = true;
			if (debug)
				Debug.Log("Guard " + gameObject.name + " is now wandering.");
		}

		public MessageHelp changeOrientationHelp = new MessageHelp("ChangeOrientation","Causes the Guard Module to immediately randomize it's orientation based on the parameters supplied on the component.");
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

		public MessageHelp clearTargetHelp = new MessageHelp("ClearTarget","Instantly clear's the Guard Module's objective and kill target.");
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
}