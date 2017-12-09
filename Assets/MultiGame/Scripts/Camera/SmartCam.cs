using UnityEngine;
using System.Collections;
using System;
using MultiGame;

namespace MultiGame
{
	[RequireComponent(typeof(MouseAim))]
	public class SmartCam : MultiModule
	{
		[Header("Targeting Settings")]
		[RequiredField("The object this camera should follow",RequiredFieldAttribute.RequirementLevels.Optional)]
		public Transform target;
		[RequiredField("The tag of the object we wish to follow.")]
		public string targetTag = "Player";
		[RequiredField("The tag of the object we should always keep on screen.",RequiredFieldAttribute.RequirementLevels.Optional)]
		public string autoLookTag = "Player";
		[Tooltip("Should we search for a new target if the current one is lost?")]
		public bool autoRetarget = true;
		[Tooltip("How long should we wait before trying to retarget?")]
		public float autoRetargetTime = .6f;
		private float autoRetargetCounter;

		[Header("Input Settings")]
		[Tooltip("The key used to break the view for camera orbit using the mouse")]
		public KeyCode mouseBreak = KeyCode.Mouse1;
		[RequiredField("Horizontal camera control axis (for controllers)",RequiredFieldAttribute.RequirementLevels.Recommended)]
		public string camAdjustX = "";
		[RequiredField("Vertical camera control axis (for controllers)", RequiredFieldAttribute.RequirementLevels.Recommended)]
		public string camAdjustY = "";
		[RequiredField("How sensitive is the up/down look direction?")]
		public float sensitivityX = 2f;
		[RequiredField("How sensitive is the left/right look direction?")]
		public float sensitivityY = 2f;

		[Header("Auto move settings")]
		[RequiredField("Max speed to catch up")]
		public float followSpeed = 5f;
		[RequiredField("Whe automatically orienting towards the player's Y rotation, how fast can we turn?")]
		public float rotationSpeed = 1f;
		[RequiredField("How long do we wait after breaking rotation to start rotating automatically again?", RequiredFieldAttribute.RequirementLevels.Recommended)]
		public float refollowTime = 1.2f;
		private float refollowCounter = 0;

		private GameObject lookAtTarget;

		public enum UpdateModes {Late, Fixed};
		[Tooltip("Change this if you experience jitter")]
		public UpdateModes updateMode = UpdateModes.Late;




		private MouseAim mAim;
		private Vector3 newPos;

		public HelpInfo help = new HelpInfo("Smart Cam automatically follows an object. If there is no object to follow, it will attempt to find the Player object by tag.");

		void Awake () {
			mAim = GetComponent<MouseAim>();
			autoRetargetCounter = autoRetargetTime;
		}

		void FixedUpdate ()
		{
			if (lookAtTarget == null && !string.IsNullOrEmpty (autoLookTag)) {
				lookAtTarget = GameObject.FindGameObjectWithTag (autoLookTag);
			}
			if (target == null) {
				autoRetargetCounter -= Time.deltaTime;
				if (autoRetargetCounter <= 0) {
					autoRetargetCounter = autoRetargetTime;
					if (GameObject.FindGameObjectWithTag(targetTag) != null)
						target = GameObject.FindGameObjectWithTag(targetTag).transform;
				}
			}

			if (updateMode == UpdateModes.Fixed)
				FollowTarget();
		}

		void LateUpdate () {
			if (updateMode == UpdateModes.Late)
				FollowTarget();
		}

		void FollowTarget () {
			if (target) {
				newPos = target.position;
				transform.position = Vector3.Lerp (transform.position, newPos, followSpeed * Time.deltaTime);
				RotateToTarget();
			}
		}

		void RotateToTarget () {
			refollowCounter -= Time.deltaTime;
			if (Input.GetKey(mouseBreak))
				refollowCounter = refollowTime;

			if (refollowCounter <= 0f) {
				if (lookAtTarget != null) {
					transform.LookAt (lookAtTarget.transform, Vector3.up);
				} else {
					mAim.enabled = false;
					transform.rotation = Quaternion.Slerp (transform.rotation, target.rotation, Time.deltaTime * rotationSpeed);
				}
			} else {
				mAim.enabled = true;
				if (!string.IsNullOrEmpty(camAdjustX) && !string.IsNullOrEmpty(camAdjustY)) {
					transform.RotateAround(transform.position, Vector3.right, Input.GetAxis(camAdjustX) * sensitivityX);
					transform.RotateAround(transform.position, Vector3.up, Input.GetAxis(camAdjustY) * sensitivityY);
				}
			}
		}
			

		[Header("Available Messages")]
		public MessageHelp setXSensitivityHelp = new MessageHelp("SetXSensitivity","Sets the up/down sensitivity which controls rotation of the camera on the X axis",3,"The new sensitivity level. Default is 2.");
		public void SetXSensitivity (float _sensitivity) {
			sensitivityX = _sensitivity;
		}
		public MessageHelp setYSensitivityHelp = new MessageHelp("SetXSensitivity","Sets the left/right sensitivity which controls rotation of the camera on the Y axis",3,"The new sensitivity level. Default is 2.");
		public void SetYSensitivity (float _sensitivity) {
			sensitivityY = _sensitivity;
		}
	}
}
