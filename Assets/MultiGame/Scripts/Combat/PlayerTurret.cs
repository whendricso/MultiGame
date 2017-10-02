using UnityEngine;
using System.Collections;

namespace MultiGame {
	[RequireComponent(typeof(TurretAction))]
	public class PlayerTurret : MultiModule {

		public enum AimCorrectionModes {DistantPoint, Raycast};
		[Tooltip("When rotating the turret, should we use Distant Point aim correction, which corrects for crosshair distortion by imagining a point in the far distance, or should we point " +
			"directly at any collider we are looking at directly?")]
		public AimCorrectionModes aimCorrectionMode = AimCorrectionModes.DistantPoint;
		[Tooltip("Layers we can raycast onto for aim correction, only used for 'DistantPoint' Aim Correction Mode.")]
		public LayerMask aimCorrectionMask;

		public float sensitivityX = 2f;
		public float sensitivityY = 2f;
		public float minX = -360f;
		public float maxX = 360f;
		public float minY = -360f;
		public float maxY = 360f;


		[RequiredFieldAttribute("A Game Object representing the spot where bullets exit the weapon muzzle.")]
		public GameObject muzzleTransform;

		[Tooltip("The speed at which the turret rotates toward it's target")]
		public float turnSpeed = 120f;
		[RequiredFieldAttribute("A crosshair texture to show where we want to shoot", RequiredFieldAttribute.RequirementLevels.Recommended)]
		public Texture2D crosshairs;
		[RequiredFieldAttribute("A crosshair to show where we are shooting exactly, this frame (may be different from position we are aiming at)", RequiredFieldAttribute.RequirementLevels.Recommended)]
		public Texture2D crosshairCore;
		[Tooltip("Is this turret currently being controlled by the player?")]
		public bool inUse = false;
		[RequiredFieldAttribute("An optional camera to use just while this turret is active. Will take over completely for Main Camera while in use. Otherwise, the main camera is used to aim.", RequiredFieldAttribute.RequirementLevels.Optional)]
		public Camera turretCam;

//		[RequiredFieldAttribute("A projectile to fire from the turret when activated",RequiredFieldAttribute.RequirementLevels.Recommended)]
//		public GameObject projectile;

		[HideInInspector]
		/// <summary>
		/// The last timestamp when we fired a shot.
		/// </summary>
		public float lastFireTime;

		private string turretCamTag = "Untagged";
		private Camera mainCam;

		private GameObject aimHelper;
		private GameObject pointCaster;
		private GameObject distantPoint;
		private RaycastHit hinfo;

		public bool debug = false;

		public HelpInfo help = new HelpInfo("Player Turret allows the player to control a turret directly. It can accept a crosshair texture. To use, place it on a turret top and send the " +
			"ActivateTurret and DeactivateTurret messages to it. If a Turret Cam is supplied, it will take over for the Main Camera until 'DeactivateTurret' is received.");

		void OnGUI () {
			if (!inUse || crosshairs == null)
				return;

			GUI.DrawTexture(new Rect((.5f*Screen.width) - (.5f*crosshairs.width), (.5f*Screen.height) - (.5f*crosshairs.height), crosshairs.width, crosshairs.height ), crosshairs);
		}

		void Start () {
			lastFireTime = Time.time;
			if (crosshairs == null)
				crosshairs = Resources.Load<Texture2D>("Crosshair");
			if (crosshairCore == null)
				crosshairCore = Resources.Load<Texture2D>("CrosshairCore");

			aimHelper = new GameObject("Aim Helper");
			aimHelper.transform.position = gameObject.transform.position;
			aimHelper.transform.SetParent(transform.parent);
			aimHelper.transform.localRotation = transform.localRotation;

			MouseAim _aim = aimHelper.AddComponent<MouseAim>();
			_aim.sensitivityX = sensitivityX;
			_aim.sensitivityY = sensitivityY;
			_aim.minimumX = minX;
			_aim.minimumY = minY;
			_aim.maximumX = maxX;
			_aim.maximumY = maxY;


			if (turretCam == null)
				turretCam = gameObject.GetComponentInChildren<Camera>();
			if (turretCam != null) {
				turretCamTag = turretCam.gameObject.tag;
			} else {//not controlling via an attached camera, use the main camera instead.
				_aim.enabled = false;
			}

			if (inUse)
				ActivateTurret();
			else
				DeactivateTurret();
		}

		void Update () {
			if (!inUse)
				return;
			//TODO: Add additional targeting help to the Muzzle Transform, accuracy currently suffers if the transform is not exactly along the Z axis!
			if (aimHelper != null) {
				if (turretCam != null && turretCam.transform.parent != aimHelper.transform) {
					turretCam.transform.SetParent(aimHelper.transform);
					turretCam.transform.localRotation = Quaternion.identity;
				} else {
					AimAtMain();
				}


			}

//			ToDistantPoint();
			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation( aimHelper.transform.forward, transform.parent.up), turnSpeed * Time.deltaTime );
			transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, 0f);
		}

		private void AimAtMain () {
			if (aimCorrectionMode == AimCorrectionModes.DistantPoint) {
				ToDistantPoint();
			} else {//raycast aim correction
				if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hinfo, 1500f, aimCorrectionMask, QueryTriggerInteraction.Ignore)) {
					aimHelper.transform.LookAt(hinfo.point, transform.parent.up);
				} else {
					ToDistantPoint();
				}
			}
		}

		private void ToggleCameras (bool _toggle) {
			if (turretCam != null) {
				turretCam.enabled = _toggle;
				if (mainCam == null)
					mainCam = Camera.main;
				if (mainCam != null)
					mainCam.enabled = !_toggle;
				if (_toggle) {
					turretCam.gameObject.tag = "MainCamera";
				} else {
					turretCam.tag = turretCamTag;
				}
				if (mainCam != null) {
					if (_toggle) {
						mainCam.gameObject.tag = "Untagged";
					} else {
						mainCam.gameObject.tag = "MainCamera";
					}
				}
			}

		}

		/// <summary>
		/// Rotates to a direction pointing to a point far away from the center of the camera, for aim correction
		/// </summary>
		/// <returns>The distant point.</returns>
		private void ToDistantPoint () {
			if (distantPoint == null)
				distantPoint = new GameObject("DistantPoint");
			if (pointCaster == null) {
				pointCaster = new GameObject("PointCaster");
				try {
					pointCaster.transform.parent = Camera.main.transform;
				} catch {
					if (Camera.allCameras.Length > 0)
						pointCaster.transform.SetParent(Camera.allCameras[0].transform);
				}
				pointCaster.transform.localPosition = Vector3.zero;
				pointCaster.transform.localRotation = Quaternion.identity;
				distantPoint.transform.parent = pointCaster.transform;
				distantPoint.transform.localPosition = new Vector3(0.0f, 0.0f, 1500.0f);
			}
			aimHelper.transform.LookAt(distantPoint.transform);
//			Quaternion _look = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(aimHelper.transform.forward, transform.parent.up), turnSpeed * Time.deltaTime);
			//transform.eulerAngles = new Vector3(_look.eulerAngles.x + transform.eulerAngles.x, _look.eulerAngles.y + transform.eulerAngles.y, transform.eulerAngles.z);
		}

		public MessageHelp activeTurretHelp = new MessageHelp("ActivateTurret","Causes the turret to become active, controlled by the player.");
		public void ActivateTurret () {
			inUse = true;
			ToggleCameras(true);
			if (debug)
				Debug.Log("PlayerTurret " + gameObject.name + " was activated");
		}

		public MessageHelp deactivateTurretHelp = new MessageHelp("DeactivateTurret","Causes the turret to reset to deactivated state, call 'ActivateTurret' to start using it again");
		public void DeactivateTurret () {
			inUse = false;
			ToggleCameras(false);
			if (debug)
				Debug.Log("PlayerTurret " + gameObject.name + " was deactivated");
		}


	}
}