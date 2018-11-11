using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame
{
	//[RequireComponent(typeof(MouseAim))]
	public class SmartCam : MultiModule
	{
		[Header("Targeting Settings")]
		[RequiredField("The object this camera should follow",RequiredFieldAttribute.RequirementLevels.Optional)]
		public Transform target;
		[RequiredField("The tag of the object we wish to follow.")]
		public string targetTag = "Player";
		//[RequiredField("The tag of the object we should always keep on screen.",RequiredFieldAttribute.RequirementLevels.Optional)]
		//public string autoLookTag = "Player";
		[Tooltip("Should we search for a new target if the current one is lost?")]
		public bool autoRetarget = true;
		[Tooltip("How long should we wait before trying to retarget?")]
		public float autoRetargetTime = .6f;
		private float autoRetargetCounter;
		
		/*
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
		*/

		[Header("Auto move settings")]
		[RequiredField("Max speed to catch up")]
		public float followSpeed = 5f;
		[RequiredField("Whe automatically orienting towards the player's Y rotation, how fast can we turn?", RequiredFieldAttribute.RequirementLevels.Optional)]
		public float rotationSpeed = 1f;
		[RequiredField("How long do we wait after breaking rotation to start rotating automatically again?", RequiredFieldAttribute.RequirementLevels.Recommended)]
		public float refollowTime = 1.2f;
		[Tooltip("An offset vector relative to the facing of the target object.")]
		public Vector3 relativeOffset = new Vector3(0, 3, -3);
		[Tooltip("An offset vector that ignores the orientation of the target.")]
		public Vector3 globalOffset = Vector3.zero;
		[Tooltip("How much should we offset our look target, if at all? (useful for over-the-shoulder games or sidescrollers)")]
		public Vector3 lookOffset = Vector3.up;
		//private float refollowCounter = 0;

		//private GameObject lookAtTarget;

		public enum UpdateModes {Late, Fixed};
		[Tooltip("Change this if you experience jitter")]
		public UpdateModes updateMode = UpdateModes.Late;

		[Header("Shake Settings")]
		public bool shake = true;
		//public bool wobble = true;
		public bool pulse = true;
		public float shakeTime = 1f;
		public AnimationCurve shakeRolloff = new AnimationCurve(new Keyframe[] {new Keyframe(0,1), new Keyframe(1,0) });

		private float shakeMagnitude = 0f;
		private float shakeStartTime = 0;
		private float currentShake = 0;
		private float startingFov = 0;

		private Camera cam;

		private Vector3 finalOffset;//the final offset value calculated by combining the relative and global offsets

		private Vector3 newPos;
		private Transform aimTrans;

		public HelpInfo help = new HelpInfo("Smart Cam automatically follows an object. If there is no object to follow, it will attempt to find the Player object by tag.");

		void Awake () {
			cam = GetComponent<Camera>();
			if (cam == null)
				cam = GetComponentInChildren<Camera>();
			startingFov = cam.fieldOfView;
			aimTrans = new GameObject("aimTrans").transform;
			aimTrans.parent = transform;
			autoRetargetCounter = autoRetargetTime;
			if (target == null)
				target = GameObject.FindGameObjectWithTag(targetTag).transform;
		}

		void Update() {//acquire target
			UpdateShake();
			aimTrans.transform.position = transform.position;
			autoRetargetCounter -= Time.deltaTime;
			if (target == null) {
				if (autoRetargetCounter < 0 && !string.IsNullOrEmpty(targetTag)) {
					autoRetargetCounter = autoRetargetTime;
					try {
						target = GameObject.FindGameObjectWithTag(targetTag).transform;
					}
					catch { }
				}
			}
			else
				aimTrans.LookAt(target);
		}

		void FixedUpdate ()
		{
			if (updateMode == UpdateModes.Fixed) {
				FollowTarget();
				if (rotationSpeed > 0)
					RotateToTarget();
			}
		}

		void LateUpdate () {
			if (updateMode == UpdateModes.Late) {
				FollowTarget();
				if (rotationSpeed > 0)
					RotateToTarget();
			}
		}

		void UpdateShake() {
			if ((Time.time - shakeStartTime) > shakeTime)
				return;
			if (shakeMagnitude <= 0)
				return;

			currentShake = shakeRolloff.Evaluate((Time.time - shakeStartTime) / shakeTime) * shakeMagnitude;
			if (shake)
				transform.position += new Vector3(Random.Range(-currentShake,currentShake), Random.Range(-currentShake, currentShake), Random.Range(-currentShake, currentShake));
			//if (wobble)
				//transform.rotation = new Quaternion(Random.Range(-currentShake, currentShake) + transform.rotation.x, Random.Range(-currentShake, currentShake) + transform.rotation.y, Random.Range(-currentShake, currentShake) + transform.rotation.z, Random.Range(-currentShake, currentShake)+ transform.rotation.w);
			if (pulse)
				cam.fieldOfView = startingFov + Random.Range(-currentShake, currentShake);
				
		}

		void FollowTarget() {
			if (target) {
				finalOffset = target.transform.TransformDirection(relativeOffset) + globalOffset;
				newPos = target.position + finalOffset;
				transform.position = Vector3.Lerp(transform.position, newPos, followSpeed * Time.deltaTime);
			}
		}

		void RotateToTarget() {
			aimTrans.LookAt(target.transform.position + lookOffset, Vector3.up);
			transform.rotation = Quaternion.Slerp(transform.rotation, aimTrans.rotation, Time.deltaTime * rotationSpeed);
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
		}

		public void SetShakeTime(float newTime) {
			shakeTime = newTime;
		}

		public void Shake(float magnitude) {
			shakeMagnitude = magnitude;
			shakeStartTime = Time.time;
		}

		/*
		[Header("Available Messages")]
		public MessageHelp setXSensitivityHelp = new MessageHelp("SetXSensitivity","Sets the up/down sensitivity which controls rotation of the camera on the X axis",3,"The new sensitivity level. Default is 2.");
		public void SetXSensitivity (float _sensitivity) {
			sensitivityX = _sensitivity;
		}
		public MessageHelp setYSensitivityHelp = new MessageHelp("SetXSensitivity","Sets the left/right sensitivity which controls rotation of the camera on the Y axis",3,"The new sensitivity level. Default is 2.");
		public void SetYSensitivity (float _sensitivity) {
			sensitivityY = _sensitivity;
		}
		*/
	}
}
