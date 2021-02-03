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
		[Tooltip("How long should we wait before trying to retarget? Longer delays = better performance when no object is being targeted.")]
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
		[RequiredField("Time it takes to reach the smooth follow position")]
		public float followTime = .15f;
		[RequiredField("Whe automatically orienting towards the player's Y rotation, how fast can we turn?", RequiredFieldAttribute.RequirementLevels.Optional)]
		public float rotationSpeed = 14f;
		//[RequiredField("How long do we wait after breaking rotation to start rotating automatically again?", RequiredFieldAttribute.RequirementLevels.Recommended)]
		//public float refollowTime = 1.2f;
		[Tooltip("An offset vector relative to the facing of the target object.")]
		public Vector3 relativeOffset = new Vector3(0, 3, -3);
		[Tooltip("An offset vector that ignores the orientation of the target.")]
		public Vector3 globalOffset = Vector3.zero;
		[Tooltip("How much should we offset our look target, if at all? (useful for over-the-shoulder games or sidescrollers)")]
		public Vector3 lookOffset = Vector3.up;
		//private float refollowCounter = 0;

		//private GameObject lookAtTarget;

		public enum UpdateModes {Late, Fixed};
		[Header("General Settings")]
		[Tooltip("Change this if you experience jitter")]
		public UpdateModes updateMode = UpdateModes.Fixed;
		[Tooltip("Should we cast a ray back from the Player object to see if there is anything between it and the camera?")]
		public bool checkForObstructions = false;
		[Tooltip("What objects should count as obstructions which will be made invisible if blocking the camera?")]
		public LayerMask obstructionMask;

		[Header("Shake Settings")]
		[Tooltip("Should camera shake when the 'Shake' Message is received?")]
		public bool shake = true;
		//public bool wobble = true;
		[Tooltip("Should the camera pulse it's Field Of View when the 'Shake' Message is received?")]
		public bool pulse = true;
		[RequiredField("How long should camera shake last? This can be changed with the 'SetShakeTime' Message")]
		public float shakeTime = 1f;
		[Tooltip("How should the intensity of the shake change over time?")]
		public AnimationCurve shakeRolloff = new AnimationCurve(new Keyframe[] {new Keyframe(0,1), new Keyframe(1,0) });

		private float shakeMagnitude = 0f;
		private float shakeStartTime = 0;
		private float currentShake = 0;
		private float startingFov = 0;

		private Camera cam;

		private Vector3 finalOffset;//the final offset value calculated by combining the relative and global offsets

		private Vector3 newPos;
		private Vector3 mvVel = Vector3.zero;
		private Transform aimTrans;
		private GameObject hiddenObject = null;

		public HelpInfo help = new HelpInfo("Smart Cam automatically follows an object. If there is no object to follow, it will attempt to find the Player object (default) by tag. " +
			"\n\n" +
			"To use, simply add this component to your Main Camera. The global offset is where the camera rests regardless of character orientation, whereas " +
			"local offset takes player orientation into account. Look Offset is the actual point the camera follows, and it's added to the follow target's position. " +
			"Camera shake is supported, simply send the 'Shake' message to this component by any means, with a magnitude (size of the shake) in floating point.\n" +
			"The primary use for Smart Cam is for top-down or third-person games where you want a camera that is easy-to-use, familiar to players and works in " +
			"most third-person situations.");

		private void Awake() {
			aimTrans = new GameObject("aimTrans").transform;
			aimTrans.parent = transform;
		}

		void OnEnable () {
			if (cam == null)
				cam = GetComponent<Camera>();
			if (cam == null)
				cam = GetComponentInChildren<Camera>();
			shakeStartTime = 0;
			shakeMagnitude = 0;
			currentShake = 0;

			startingFov = cam.fieldOfView;
			autoRetargetCounter = autoRetargetTime;
			try {
				if (target == null)
					target = GameObject.FindGameObjectWithTag(targetTag).transform;
			} catch { }
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
			if (checkForObstructions)
				CheckForObstructions();
			if (updateMode == UpdateModes.Late) {
				FollowTarget();
				if (rotationSpeed > 0)
					RotateToTarget();
			}
		}

		void CheckForObstructions() {
			if (target == null)
				return;
			RaycastHit _hinfo;
			bool didHit = Physics.Linecast(target.transform.position, Camera.main.transform.position, out _hinfo, obstructionMask, QueryTriggerInteraction.Ignore);
			if (didHit) {
				if (hiddenObject != null && _hinfo.collider.gameObject != hiddenObject)
					hiddenObject.GetComponentInChildren<Renderer>().enabled = true;
				hiddenObject = _hinfo.collider.gameObject;
				hiddenObject.GetComponentInChildren<Renderer>().enabled = false;
			}
			else {
				if (hiddenObject != null) {
					hiddenObject.GetComponentInChildren<Renderer>().enabled = true;
					hiddenObject = null;
				}
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
			if (target != null) {
				finalOffset = target.transform.TransformDirection(relativeOffset) + globalOffset;
				newPos = target.position + finalOffset;
				transform.position = Vector3.SmoothDamp(transform.position, newPos,ref mvVel, followTime);//Vector3.Lerp(transform.position, newPos, followSpeed * Time.deltaTime);
			}
		}

		//Vector3 rotvel = Vector3.zero;
		void RotateToTarget() {
			if (target == null)
				return;
			aimTrans.LookAt(target.transform.position + lookOffset, Vector3.up);
			transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, aimTrans.rotation, Time.deltaTime * rotationSpeed);
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
		}

		public MessageHelp setShakeTimeHelp = new MessageHelp("SetShakeTime","Allows you to define a new Shake Time for this camera, which controls how long the camera shakes for.",3,"How long would you like camera shake to last for?");
		public void SetShakeTime(float newTime) {
			shakeTime = newTime;
		}

		public MessageHelp shakeHelp = new MessageHelp("Shake","Shakes the camera immediately by setting a new shake magnitude, which decays over time based on the settings above",3,"How much would you like the camera to shake in world position (or field of view, for pulse mode)?");
		public void Shake(float magnitude) {
			shakeMagnitude = magnitude;
			shakeStartTime = Time.time;
		}

		void ReturnFromPool() {
			target = null;
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
