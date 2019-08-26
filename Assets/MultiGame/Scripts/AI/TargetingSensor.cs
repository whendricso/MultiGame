using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/AI/Targeting Sensor")]
	public class TargetingSensor : MultiModule {

		[Reorderable]
		[Header("Important - must be populated")]
		[Tooltip("What tags are we looking for while targeting? Any Game Object with one of these tags that is found will be passed as a target to the 'Message Receiver' defined below.")]
		public string[] targetTags;
		[Header("Target Settings")]
		[Tooltip("Should we target the root transform of the object detected, or the object itself? Useful if colliders are parented to agents instead of being part of their root object.")]
		public bool targetRoot = false;
		[RequiredField("What object has a targeting computer, or other MultiGame AI component attached that needs a target?", RequiredFieldAttribute.RequirementLevels.Required)]
		public GameObject messageReceiver;
		[RequiredField("How often, in seconds, can we change targets?",RequiredFieldAttribute.RequirementLevels.Required)]
		public float retargetTime = 0.75f;
		[RequiredField("How far away does our target need to get before we look for another?", RequiredFieldAttribute.RequirementLevels.Required)]
		public float maxDistance = 25.0f;

		[Header("Line Of Sight")]
		[Tooltip("Should we check line of sight between the message receiver and the target? This requires a raycast, which queries the physics engine and can be expensive.")]
		public bool checkLOS = false;
		[Tooltip("How much should we offset the ray origin from the origin of the message receiver?")]
		public Vector3 rayOriginOffset = Vector3.zero;
		[Tooltip("How much should we offset the target point of the ray against the target we're checking for LOS?")]
		public Vector3 rayTargetOffset = Vector3.zero;
		[Tooltip("What physics layers can obstruct the view of this AI? This should NOT include the target!")]
		public LayerMask obstructionMask;
		[Tooltip("Should we try to get into line of sight if we don't have it?")]
		public bool chaseToLOS = false;

		private bool canRetarget = true;
		private GameObject lastTarget;

		public HelpInfo help = new HelpInfo("This component should be attached to a trigger that is parented to an AI. It provides target information to other AI components." +
			" To use most effectively, we recommend creating 4 collision layers (at least), one each for friendlies and enemies, and one each for friendly and enemy sensors." +
			" Then in the Physics Manager (Edit -> Project Settings -> Physics) make it so that the enemy sensor collides only with friendlies and vice versa for friendly sensor. " +
			"This speeds up target detection, allowing more AI to be active at once. To further refine target lists, assign some target tags because anything not in that list is " +
			"completely ignored.");

		[Tooltip("WARNING! SLOW OPERATION! Should we output useful information to the console?")]
		public bool debug = false;
		
		void OnEnable () {
			if (messageReceiver == null) {
				Debug.LogError("Targeting Sensor " + gameObject.name + " requires a Message Receiver to assign a target!");
				enabled = false;
				return;
			}
			GetComponent<Collider>().isTrigger = true;
		}

		private void OnDisable() {
			lastTarget = null;
			canRetarget = true;
		}

		void Update () {
			if (lastTarget == null)
				return;
			if (Vector3.Distance(transform.position, lastTarget.transform.position) > maxDistance) {
				lastTarget = null;
				messageReceiver.SendMessage("ClearTarget", SendMessageOptions.DontRequireReceiver);
			}
		}
		
		//Used for target acquisition & hunting behavior
		void OnTriggerStay (Collider other) {
			if (debug)
				Debug.Log("Targeting Sensor " + gameObject.name + " is checking if " + other.gameObject.name + " is a valid target");
			if (!canRetarget)
				return;
			if (!CheckIsValidTarget(other.gameObject))
				return;
			if (lastTarget != null)//Only continue if we have no target currently
				return;

			if (checkLOS) {
				if (!QueryLineOfSight(other.gameObject)) {
					return;
				}
				if (chaseToLOS) {
					messageReceiver.SendMessage("Hunt", SendMessageOptions.DontRequireReceiver);
					messageReceiver.SendMessage("FaceMoveDirection", SendMessageOptions.DontRequireReceiver);
				}
			}
			messageReceiver.SendMessage("SetTarget", other.gameObject, SendMessageOptions.DontRequireReceiver);

			if (debug)
				Debug.Log("Targeting Sensor " + gameObject.name + " set it's target to " + other.gameObject.name);
			canRetarget = false;
			StartCoroutine(ReEnableTargeting());
			if (targetRoot)
				lastTarget = other.transform.root.gameObject;
			else
				lastTarget = other.gameObject;
			messageReceiver.SendMessage("SetTarget", lastTarget, SendMessageOptions.DontRequireReceiver);
		}
		
		IEnumerator ReEnableTargeting () {
			yield return new WaitForSeconds(retargetTime);
			canRetarget = true;
		}
		
		bool CheckIsValidTarget (GameObject _possibleTarget) {
			bool _ret = false;
			foreach (string str in targetTags) {
				if (_possibleTarget.tag == str)
					_ret = true;
			}
			return _ret;
		}

		/// <summary>
		/// Casts a ray into the scene to determine if the target can be seen.
		/// </summary>
		/// <param name="_target">The target we wish to check for discoverability.</param>
		/// <returns>True if no obstruction was found by the ray.</returns>
		bool QueryLineOfSight(GameObject _target) {
			bool _ret = true;
			_ret = !Physics.Linecast(messageReceiver.transform.position + rayOriginOffset, _target.transform.position + rayTargetOffset, obstructionMask);
			return _ret;
		}
	}
}