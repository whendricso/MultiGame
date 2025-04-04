﻿using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/AI/Guard Module")]
	public class GuardModule : MultiModule {

		[Header("Objectives")]
		[Tooltip("What are we trying to guard?")]
		public GameObject objective;
		[System.NonSerialized]
		public Vector3 objectivePosition;//Set to match the position of the objective, if any, otherwise we guard our starting position
		//[System.NonSerialized]
		//public Vector3 persistentMoveTarget;
		[RequiredField("Tag of the object we want to guard", RequiredFieldAttribute.RequirementLevels.Optional)]
		public string guardObjectiveTag;
		[Tooltip("How often do we search for our objective by tag? (Only occurs if a Guard Objective Tag is defined)")]
		public float objectiveSearchTime = 2;
		[Tooltip("Who you wan' me kill? (Assigns a target that already exists in the scene, good for scripted events)")]
		public GameObject killTarget;

		[Header("Line Of Sight")]
		public float visionRange = 45;
		[Tooltip("When we're looking for our target, what collision layers can block our view?")]
		public LayerMask obstructionMask;
		public Vector3 lookRayOffset = Vector3.zero;
		public Vector3 targetRayOffset = Vector3.zero;
		public bool switchWalkMode = false;
		
		[Header("Behavior Modifiers")]
		//[Tooltip("How often do I think about changing targets?")]
		//public float targetCooldownTime = 6.0f;
		[Tooltip("How far from my objective can I travel?")]
		public float guardRange = 30.0f;
		[Tooltip("How far do I turn, in degrees, while guarding a position?")]
		public float guardingRotation = 30.0f;
		[Tooltip("How varied, in degrees, is that rotation?")]
		public float rotationVariance = 15.0f;
		[Tooltip("How many seconds between patrol times?")]
		public float wanderInterval = 15.0f;
		[Tooltip("How long do I walk for?")]
		public float wanderWalkTime = 3.0f;
		private float wanderCounter;
		private float currentSearchTime = 0;
		private float targetSearchTime = 0;
		private bool hunting = false;
		private Vector3 lastSeenPosition = Vector3.zero;
		private bool targetInView = false;
		private bool targetInViewLastFrame = false;

		[Tooltip("Do I change orientation automatically?")]
		public bool autoLookAround = true;
		[Tooltip("Do I wander about the area?")]
		public bool wander = true;
		private bool wandering = false;
		private bool returning = false;

#if UNITY_EDITOR

		public HelpInfo help = new HelpInfo("Add to an object representing an AI guard or cannon fodder. This should be an empty object with a 3D model of a guard parented to it." +
			"\nBy \"Guard\" we mean an AI that should stay in a given area or near a given object, and attack enemies that provoke it. To use this effectively, we recommend" +
			" also adding a NavModule or similar, since this guard will probably need to get around. Also, add some sort of combat component, or attach a turret so it can do harm.");

#endif
		[Tooltip("Should I output information to the console?")]
		public bool debug = false;

		void OnEnable () {
			wandering = false;
			returning = false;
			wanderCounter = wanderInterval;
			lastSeenPosition = transform.position;
			if (objective == null)
				objectivePosition = transform.position;
			else
				objectivePosition = objective.transform.position;
		}

		private void OnDisable() {
			StopAllCoroutines();
		}

		void FixedUpdate () {
			wanderCounter -= Time.deltaTime;
			currentSearchTime -= Time.deltaTime;
			targetSearchTime -= Time.deltaTime;

			if (objective != null)
				objectivePosition = objective.transform.position;
			else {
				if (currentSearchTime < 0 && !string.IsNullOrEmpty(guardObjectiveTag)) {
					currentSearchTime = objectiveSearchTime;
					GameObject _nearest = FindClosestByTag(guardObjectiveTag);
					if (_nearest != null)
						objective = _nearest;
				}
			}

			UpdateTargetView();

			if (killTarget == null) {
				hunting = false;
				UpdateWander();
			} else {
				if (switchWalkMode) {
					if (targetInView/* && !targetInViewLastFrame*/)
						gameObject.SendMessage("FaceTarget", SendMessageOptions.DontRequireReceiver);
					else if (!targetInView/* && targetInViewLastFrame*/)
						gameObject.SendMessage("FaceMoveDirection", SendMessageOptions.DontRequireReceiver);
				}
				UpdateAttack();
			}
			

			targetInViewLastFrame = targetInView;
		}
		
		private void UpdateTargetView() {
			if (killTarget != null && Vector3.Distance(killTarget.transform.position, transform.position) < visionRange) {
				targetInView = !Physics.Linecast(transform.position + lookRayOffset, killTarget.transform.position + targetRayOffset, obstructionMask);
			} else {
				targetInView = false;
			}
			if (debug && killTarget != null)
				Debug.DrawLine(transform.position + lookRayOffset, killTarget.transform.position + targetRayOffset,(targetInView ? XKCDColors.PaleGreen : XKCDColors.YellowOrange));
		}

		private void UpdateWander() {

			if (Vector3.Distance(transform.position, objectivePosition) > guardRange)
				StartCoroutine(StopWandering(0));

			if (wanderCounter <= 0) {
				wanderCounter = wanderInterval;
				if (autoLookAround || wander)
					ChangeOrientation();
				if (wander) {
					Wander();
				}
			}
		}

		private void UpdateAttack() {
			if (targetSearchTime < 0 && killTarget != null) {
				targetSearchTime = objectiveSearchTime;
				gameObject.SendMessage("SetTarget", killTarget, SendMessageOptions.DontRequireReceiver);//A hack! Shouldn't be sending messages every frame on AI
			}
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
		public MessageHelp huntHelp = new MessageHelp("Hunt","Enables hunting mode, where we will try to go to the target's last known position");
		public void Hunt() {
			hunting = true;
		}

		public MessageHelp stopHuntingHelp = new MessageHelp("StopHunting","Disables hunting mode, so normal target following behavior willbe used instead");
		public void StopHunting() {
			hunting = false;
		}

		public MessageHelp wanderHelp = new MessageHelp("Wander", "Causes the Guard Module to immediately begin wandering to a new location.");
		public void Wander () {
			gameObject.SendMessage("FaceMoveDirection", SendMessageOptions.DontRequireReceiver);
			if (!gameObject.activeInHierarchy)
				return;
			//if (objective != null)
			//	return;
			StopAllCoroutines();
			StartCoroutine(StopWandering(wanderWalkTime));
			wandering = true;
			if (debug)
				Debug.Log("Guard " + gameObject.name + " is now wandering.");
		}

		public MessageHelp changeOrientationHelp = new MessageHelp("ChangeOrientation","Causes the Guard Module to immediately randomize it's orientation based on the parameters supplied on the component.");
		public void ChangeOrientation () {
			if (!gameObject.activeInHierarchy)
				return;
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
			if (!gameObject.activeInHierarchy)
				return;
			gameObject.SendMessage("FaceTarget", SendMessageOptions.DontRequireReceiver);
			//if (objective != null)
			//	return;
			if (debug)
				Debug.Log("Guard " + gameObject.name + " is now targeting " + _target.name);
			killTarget = _target;
			//objective = _target;
			SetObjective(_target.transform.position);
		}

		public MessageHelp clearTargetHelp = new MessageHelp("ClearTarget","Instantly clear's the Guard Module's objective and kill target.");
		public void ClearTarget () {
			if (debug)
				Debug.Log("Guard " + gameObject.name + " cleared it's target.");
			killTarget = null;
			//objective = null;
			StartCoroutine(StopWandering(0));
		}

		public MessageHelp targetNearestHelp = new MessageHelp("TargetNearest","If an object with the given tag is found, target that!",4,"The tag of the object you wish to target.");
		public void TargetNearest(string _tag) {

			if (!gameObject.activeInHierarchy)
				return;

			GameObject _nearest = FindClosestByTag(_tag);
			if (_nearest == null) {
				if (debug)
					Debug.Log("Guard Module " + gameObject.name + " could not find any target tagged " + _tag);
				return;
			}
			if (debug)
				Debug.Log("Guard Module " + gameObject.name + " found a target tagged " + _tag);
			SetTarget(_nearest);
		}

		public void SetObjective (Vector3 _position) {
			if (!gameObject.activeInHierarchy)
				return;
			objectivePosition = _position;
		}

		public void MoveTo (Vector3 _position) {
			if (!gameObject.activeInHierarchy)
				return;
			//persistentMoveTarget = _position;
			//SetObjective(_position);
		}

		private void ReturnFromPool() {
			ClearTarget();
		}
	}
}