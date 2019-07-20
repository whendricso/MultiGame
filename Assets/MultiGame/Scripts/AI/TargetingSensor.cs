using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/AI/Targeting Sensor")]
	public class TargetingSensor : MultiModule {

		[ReorderableAttribute]
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
		
		void OnTriggerStay (Collider other) {
			if (debug)
				Debug.Log("Targeting Sensor " + gameObject.name + " is checking if " + other.gameObject.name + " is a valid target");
			if (!canRetarget)
				return;
			if (!CheckIsValidTarget(other.gameObject))
				return;
			if (lastTarget != null)
				return;
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
		
		bool CheckIsValidTarget (GameObject possibleTarget) {
			bool ret = false;
			foreach (string str in targetTags) {
				if (possibleTarget.tag == str)
					ret = true;
			}
			return ret;
		}

	}
}