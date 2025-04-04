﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;

namespace MultiGame {
	[AddComponentMenu("MultiGame/Motion/Object Seeker")]
	public class ObjectSeeker : MultiModule {

		[Header("Targeting Settings")]
		[RequiredField("The object this camera should follow",RequiredFieldAttribute.RequirementLevels.Optional)]
		public Transform target;
		[RequiredField("The tag of the object we wish to follow.")]
		public string targetTag = "Player";
		[Tooltip("Should we search for a new target if the current one is lost?")]
		public bool autoRetarget = true;
//		[Tooltip("How long should we wait before trying to retarget?")]
//		public float autoRetargetTime = .6f;
//		private float autoRetargetCounter;

		[Header("Auto move settings")]
		[Tooltip("If true, follow speed is ignored and we're always at the target position")]
		public bool instant = false;
		[RequiredField("Max speed to catch up, if not set to 'Instant'")]
		public float followSpeed = 5f;
		[RequiredField("Whe automatically orienting towards the player's Y rotation, how fast can we turn?")]
		public float rotationSpeed = 1f;
		[Tooltip("Should we offset our target position in world coordinates?")]
		public Vector3 offset = Vector3.zero;
		//		[RequiredField("How long do we wait after breaking rotation to start rotating automatically again?", RequiredFieldAttribute.RequirementLevels.Recommended)]
		//		public float refollowTime = 1.2f;
		//		private float refollowCounter = 0;

		public enum UpdateModes {Late, Fixed};
		[Tooltip("Change this if you experience jitter")]
		public UpdateModes updateMode = UpdateModes.Late;

//		private MouseAim mAim;
		private Vector3 newPos;

		public HelpInfo help = new HelpInfo("Object Seeker moves smoothly towards a target. If there is no object to follow, it will attempt to find the Player object by tag.");

		//void Awake () {
//			mAim = GetComponent<MouseAim>();
//			autoRetargetCounter = autoRetargetTime;
		//}

		void FixedUpdate ()
		{
			if (updateMode == UpdateModes.Fixed)
				FollowTarget();
		}

		void LateUpdate () {
			if (updateMode == UpdateModes.Late)
				FollowTarget();
		}

		void FollowTarget () {
			if (target != null) {
				newPos = target.position;
				if (instant)
					transform.position = newPos;
				else
					transform.position = Vector3.Lerp (transform.position, newPos + offset, followSpeed * Time.deltaTime);
			} else {
				try {
					target = GameObject.FindGameObjectWithTag(targetTag).transform;
				}
				catch { }//ignore null ref
			}
		}
	}
}