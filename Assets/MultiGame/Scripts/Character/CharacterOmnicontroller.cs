﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MultiGame;

namespace MultiGame {

	[RequireComponent(typeof (CharacterController))]
	[RequireComponent(typeof (AudioSource))]
	public class CharacterOmnicontroller : MultiModule {

		[Header("Important - Must be populated")]
		[Tooltip("What physics layers can the player walk on?")]
		public LayerMask walkRayMask;

		[Header("Input")]
		[Tooltip("Should we raycast towards the camera to see if anything is in the way and hide it if it is?")]
		public bool checkForObstructions = false;
		[Tooltip("What physics layers can obstruct the camera? (We will set them invisible when the player passes behind them)")]
		public LayerMask obstructionMask;
		[Tooltip("Should this component enforce a rotation on the character?")]
		public bool autoTurn = false;
		[Tooltip("Should this component rotate the character to face the mouse pointer using a raycast into the scene?")]
		public bool rotateToPointer = false;
		[Tooltip("Should the controller face in the direction of the movement stick? If true, the character will face one of 8 move directions instantly based on input (octodirectional), instead of turning (tank turning)")]
		public bool octodirectional = false;
		[Tooltip("For octodirectional movement, what is the input stick deadzone? This is also used for WASD movement, as Unity interprets this as stick input.")]
		[Range(0,1)]
		public float inputDeadzone = 0.2f;
		[Tooltip("What key must be held to enable sprinting?")]
		public KeyCode sprintKey = KeyCode.LeftShift;
		[Tooltip("What is the UGUI Slider you would like to use to indicate stamina, if any?")]
		public Slider staminaSlider;

		[Header("Motion")]
		[Tooltip("If true, will auto-parent to the object it stands on, for moving platform support.")]
		public bool platformParent = true;
		public ParentingModes parentingMode = ParentingModes.RootTransform;
		[RequiredFieldAttribute("Moving forward, how fast can the character go normally?")]
		public float runSpeed = 7f;
		[RequiredFieldAttribute("Moving forward, how fast can the character go at full sprint?")]
		public float sprintSpeed = 11f;
		[RequiredFieldAttribute("How long, in seconds, can the character sprint?")]
		public float sprintStamina = 10f;
		[RequiredFieldAttribute("When not sprinting, how quickly does the character's stamina recover?")]
		public float staminaRecovery = 2f;
		[RequiredField("What is the minimum amount of stamina we need to begin sprinting?")]
		public float minimumStamina = 2f;
		[RequiredFieldAttribute("How fast are we able to move sideways?")]
		public float strafeSpeed = 4.5f;
		[RequiredFieldAttribute("How fast can we move backwards?")]
		public float backpedalSpeed = 3f;
		[RequiredFieldAttribute("When rotating based on direction, how fast can we spin (in degrees per second)?")]
		public float autoTurnSpeed = 8f;
		[Tooltip("Maximum forward/backward speed even when airborne")]
		public float maxZVelocity = 10f;
		[Tooltip("Maximum left/right speed even when airborne")]
		public float maxXVelocity = 10f;
		[RequiredFieldAttribute("How close do we stop when chasing the pointer?")]
		public float deadzone = 1.24f;
		[RequiredFieldAttribute("How strong is our jump?")]
		public float jumpPower = 6f;
		[RequiredFieldAttribute("How long can the player continue holding jump to increase the height?")]
		public float extraJumpTime = .8f;
		[Tooltip("How much does the jump slow over time? This defines the arc of the jump while moving up. Gravity defines the fall.")]
		public AnimationCurve jumpRolloff;
		[RequiredFieldAttribute("How much air control do we have?")]
		public float airControlMultiplier  = .3f;

		[Header("Animation")]
		[RequiredFieldAttribute("Name of a floating-point value in the attached Animator, if any. Sends the Vertical axis.", RequiredFieldAttribute.RequirementLevels.Recommended)]
		public string animatorRun = "Run";
		[RequiredField("Name of a boolean value in the attached animator, if any. Indicates the state of character sprinting.",RequiredFieldAttribute.RequirementLevels.Recommended)]
		public string animatorSprint = "Sprint";
		[RequiredFieldAttribute("Name of a floating-point value in the attached Animator, if any. Sends the Horizontal axis", RequiredFieldAttribute.RequirementLevels.Recommended)]
		public string animatorStrafe = "Strafe";
		[RequiredFieldAttribute("Name of a trigger in the attached Animator, if any.", RequiredFieldAttribute.RequirementLevels.Recommended)]
		public string animatorJump = "Jump";
		[RequiredFieldAttribute("An optional return trigger fired when we hit the ground", RequiredFieldAttribute.RequirementLevels.Optional)]
		public string animatorJumpReturn = "";

		[Header("Audio")]
		[Tooltip("A separate audio source for foley sounds")]
		public AudioSource foleyAudio;
		[Tooltip("How much should we vary the pitch of the foley sounds?")][Range(0,1)]
		public float pitchVariance = .05f;
		[Tooltip("Looping footstep sound")]
		public AudioClip runSound;
		[Tooltip("Minimum vertical input (between 0 and 1) needed to hear the footfall sound.")]
		public float minRunThreshold = .3f;
		[Tooltip("Time, in seconds, between footfalls")]
		public float footstepInterval = .35f;
		//[Tooltip("Looping footstep sound")]
		//public AudioClip strafeSound;
		[Tooltip("Played when we begin jumping")]
		public AudioClip jumpSound;
		[Tooltip("Played when we land")]
		public AudioClip landingSound;

		[Header("Combat")]
		[Reorderable]
		public List<string> attackableTags = new List<string>();
		[Tooltip("How far is the center of the character from it's origin point? Ex: if the origin is at the feet and the character is 2m tall, the center is at 1m")]
		public float characterCenterYOffset = 1;
		[Tooltip("Should we use a default attack for this character? You can disable this if you are using another system for melee attacks.")]
		public bool useDefaultAttack = true;
		[Tooltip("What is this character's base (unarmed) damage?")]
		public float baseDamage = 25f;
		[Tooltip("How long after the start of the animation do we wait to apply damage? Time this with the point when the attack should connect.")]
		public float damageDelay = 1;
		[RequiredField("How far from the center of the character can the melee attack reach?")]
		public float baseReach = 3f;
		[Range(0,1)][Tooltip("The percentage of the character's field of view (assuming 180 degrees FOV) that counts as a hit. At 0, the entire FOV is valid, at 1, none of it is")]
		public float baseArc = .4f;
		[RequiredFieldAttribute("How long must we wait between attacks?")]
		public float attackCooldown = 1f;
		[RequiredFieldAttribute("Which Input Manager button are we using to activate the default melee attack?")]
		public string attackButton = "Fire1";
		[Tooltip("Which hardware key are we using to activate the default melee attack? (optional, set to 'None' to deactivate)")]
		public KeyCode attackKey = KeyCode.Mouse0;
		[RequiredFieldAttribute("Name of a trigger in the attached Animator, if any.", RequiredFieldAttribute.RequirementLevels.Recommended)]
		public string animatorAttack = "Attack";
		[Tooltip("Hyyyyah!!")]
		public AudioClip attackSound;
		public MessageManager.ManagedMessage attackMessage;

		public enum ParentingModes {Transform, RootTransform };

		[ReorderableAttribute]
		[Header("Custom")]
		[Tooltip("Custom input actions which respond to a key and/or button press. When activated, any supplied parameters for that CustomAction will be used and the rest ignored.")]
		public List<CustomAction> customActions = new List<CustomAction>();


		float totalDamage = 0;
		float reach = 0;
		float meleeArc = 0;
		float stunTime = 0;

		private Inventory inventory;
		private bool jumping = false;
		private float jumpTimer;
		private GameObject hiddenObject;
		//		private bool moveLocked = false;

		[System.Serializable]
		public class CustomAction {
			public KeyCode key;
			public string button;
			public float cooldown;
			public string mecanimTrigger;
			public AudioClip sound;
			public MessageManager.ManagedMessage message;
			[HideInInspector]
			public float timeStamp = 0f;

			public CustomAction (KeyCode _key, string _button, float _cooldown, string _mecanimTrigger, AudioClip _sound, MessageManager.ManagedMessage _message) {
				key = _key;
				button = _button;
				cooldown = _cooldown;
				mecanimTrigger = _mecanimTrigger;
				sound = _sound;
				message = _message;
			}
		}

		//[System.NonSerialized]
		//bool grounded = false;
		//private bool grnd = false;//debugging only
		private bool inAir = false;
		private float adjustedVertical = 0f;
//		private float originalPitch;
		private float attackTimer = 0f;
		/// <summary>
		/// True relative velocity of the character
		/// </summary>
		private Vector3 trueVelocity;
		/// <summary>
		/// Most recent ground velocity
		/// </summary>
		private Vector3 lastGroundVel;
		//private Vector3 lastPos;
//		private Vector3 previousPlatformPosition = Vector3.zero;
		private Rigidbody rigid;
		private RaycastHit hinfo;
		private Vector3 mouseHitPos = Vector3.zero;
		private bool mouseDidHit = false;
//		private bool didHit = false;
		private CharacterController controller;
		private Animator anim;
		private AudioSource source;
		//private bool footFall = false;
		private float footFallCounter = 0;
		private GameObject platform;//if non-null, we're standing on something, and should move if it moves
		private float stunDuration = 0;
		private float bonusDamage = 0;

		private float foleyStartingPitch = 1;

		List<MeleeWeapon> weapons = new List<MeleeWeapon>();

		private float currentStamina = 0;
		private bool sprinting = false;

		float xSpd = 0;
		float ySpd = 0;
		float zSpd = 0;

		int xDir = 0;
		int zDir = 0;

		Vector3 motionVector = new Vector3();

		public HelpInfo help = new HelpInfo("Character Omnicontroller is an all-purpose player input controller which allows for various third person perspectives as well as first person " +
			"perspective controllers. Try using it with 'Auto Rotate' disabled and add a 'MouseAim' component for FPS style. If the character has an Animator component anywhere in it's heirarchy, this controller" +
			" will send animation commands to it, trigger names are set above. To use for third person, make sure you have a camera in your scene either parented to the character or using a 'SmartCam' component. " +
			"For top-down games we recommend using 'Rotate To Pointer' to give an intuitive feel.");

		public bool debug = false;

		void OnValidate () {
			MessageManager.UpdateMessageGUI(ref attackMessage, gameObject);
			foreach (CustomAction action in customActions) {
				MessageManager.UpdateMessageGUI( ref action.message, gameObject);
			}
		}

		void OnEnable () {

			currentStamina = sprintStamina;

			if (inventory == null)
				inventory = GetComponentInChildren<Inventory>();
			if (foleyAudio != null)
				foleyStartingPitch = foleyAudio.pitch;
			if (attackMessage.target == null)
				attackMessage.target = gameObject;
			if (rigid == null)
				rigid = GetComponent<Rigidbody>();
			jumpTimer = extraJumpTime;
			if (walkRayMask == LayerMask.GetMask("None"))
				walkRayMask = LayerMask.GetMask("Default");

			if (rigid != null) {
				rigid.isKinematic = true;
				rigid.useGravity = false;
			}
			if (controller == null)
				controller = GetComponent<CharacterController>();
			if (anim == null)
				anim = GetComponentInChildren<Animator>();
			if (source == null)
				source = GetComponent<AudioSource>();

			if (jumpRolloff.keys.Length < 1) {
				jumpRolloff.AddKey(0f,0.05f);//if the curve is empty, make a nice default
				jumpRolloff.AddKey(1f, 1f); 
			}
			trueVelocity = transform.InverseTransformVector(controller.velocity);
			lastGroundVel = Vector3.zero;

			UpdateMeleeDamageValue();
			foreach (CustomAction action in customActions) {
				action.timeStamp = Time.time;
				if (action.message.target == null)
					action.message.target = gameObject;
			}

			if (Camera.main == null && octodirectional)
				Debug.LogError("Character Omnicontroller " + gameObject.name + " requires a Main Camera in the scene for Octodirectional movement. Please make sure that your scene contains a camera tagged 'MainCamera'");

		}

		void FixedUpdate () {
			UpdatePlatformParent();
			stunDuration -= Time.deltaTime;

			UpdateMeleeDamageValue();

			if (staminaSlider != null)
				staminaSlider.value = (currentStamina/sprintStamina);

			if (stunDuration > 0) {
				if (anim != null)
					anim.speed = 0;
				return;
			}
			else {
				if (anim != null)
					anim.speed = 1;
			}

			UpdateAttack();
			UpdateCustomActions();
			UpdateJump();
			UpdateRotation();

			UpdateMotion();//must be called last!
		}

		void UpdateMeleeDamageValue() {
			weapons.Clear();
			weapons.AddRange(transform.root.GetComponentsInChildren<MeleeWeapon>());
			totalDamage = -baseDamage;
			meleeArc = baseArc;
			stunTime = 0;
			reach = baseReach;
			foreach (MeleeWeapon weapon in weapons) {
				if (!string.IsNullOrEmpty(weapon.animationTrigger))
					animatorAttack = weapon.animationTrigger;
				totalDamage -= weapon.damageValue;
				if (weapon.arc < meleeArc)
					meleeArc = weapon.arc;
				if (weapon.stunTime > stunTime)
					stunTime = weapon.stunTime;
				if (weapon.reach > reach)
					reach = weapon.reach;
				damageDelay = weapon.damageDelay;
			}
		}

		bool recovering = false;

		/// <summary>
		/// After everything else is processed, update and apply the motion vector
		/// </summary>
		void UpdateMotion() {
			if (currentStamina > minimumStamina)
				recovering = false;

			sprinting = Input.GetKey(sprintKey) && !recovering;

			if (currentStamina <= 0) {
				sprinting = false;
				recovering = true;
			}

			trueVelocity = transform.InverseTransformVector(controller.velocity);
			if (anim != null) {
				if (!string.IsNullOrEmpty(animatorRun)) {
					if (rotateToPointer) {
						if (Vector3.Distance(transform.position, mouseHitPos) > deadzone)
							anim.SetFloat(animatorRun, Input.GetAxis("Vertical"));
					}
					else
						anim.SetFloat(animatorRun, Input.GetAxis("Vertical"));
				}
				if (!string.IsNullOrEmpty(animatorStrafe))
					anim.SetFloat(animatorStrafe, Input.GetAxis("Horizontal"));
				if (!string.IsNullOrEmpty(animatorSprint))
					anim.SetBool(animatorSprint, sprinting);

			}
			if (Input.GetAxis("Vertical") > 0f) {
				if (sprinting && !octodirectional)
					currentStamina -= Time.deltaTime;
				adjustedVertical = Input.GetAxis("Vertical") * ((sprinting) ? sprintSpeed : runSpeed);
				footFallCounter -= Time.deltaTime;
				if (foleyAudio != null && Input.GetAxis("Vertical") > minRunThreshold) {
					if (footFallCounter < 0) {
						footFallCounter = footstepInterval;
						foleyAudio.pitch = foleyStartingPitch + Random.Range(-pitchVariance, pitchVariance);
						if (controller.isGrounded)
							foleyAudio.PlayOneShot(runSound);
					}
				}
			}
			else
				adjustedVertical = Input.GetAxis("Vertical") * backpedalSpeed;

			if (octodirectional && sprinting)
				adjustedVertical = Input.GetAxis("Vertical") * -sprintSpeed;

			if (rotateToPointer || !autoTurn)
				xSpd = Input.GetAxis("Horizontal") * strafeSpeed;
			else {
				if (octodirectional)
					xSpd = Input.GetAxis("Horizontal") * ((sprinting) ? sprintSpeed : runSpeed);
			}
			zSpd = adjustedVertical;

			if (rotateToPointer) {
				if (Vector3.Distance(transform.position, mouseHitPos) > deadzone)
					motionVector.z = controller.isGrounded ? zSpd : (trueVelocity.z < maxZVelocity ? lastGroundVel.z + zSpd * airControlMultiplier : maxZVelocity);
				else
					motionVector.z = 0;
			} else {
				if (!octodirectional)
					motionVector.z = controller.isGrounded ? zSpd : (trueVelocity.z < maxZVelocity ? lastGroundVel.z + zSpd * airControlMultiplier : maxZVelocity);
			}

			motionVector.y = ySpd;
			if (!octodirectional)
				motionVector.x = controller.isGrounded ? xSpd : (trueVelocity.x < maxXVelocity ? lastGroundVel.x + xSpd * airControlMultiplier : maxXVelocity);
			else {
				//motionVector.z = controller.isGrounded ? (xSpd + zSpd) * .5f : ((trueVelocity.z + trueVelocity.x) * .5f < (maxZVelocity + maxXVelocity) * .5f ? ((lastGroundVel.x + lastGroundVel.z) * .5f + (xSpd * airControlMultiplier + zSpd * airControlMultiplier) * .5f) : (maxXVelocity + maxZVelocity) * .5f);
				//motionVector.x = 0;
				motionVector.z = controller.isGrounded ? (Mathf.Abs(zSpd) > Mathf.Abs(xSpd) ? Mathf.Abs(zSpd) : Mathf.Abs(xSpd)) : (trueVelocity.z < maxZVelocity ? lastGroundVel.z + Mathf.Abs(zSpd) * airControlMultiplier : maxZVelocity);
			}
			if (!sprinting)
				currentStamina += staminaRecovery * Time.deltaTime;
			if (currentStamina > sprintStamina)
				currentStamina = sprintStamina;

			if (octodirectional) {
				if (sprinting && (Mathf.Abs(Input.GetAxis("Horizontal")) > inputDeadzone || Mathf.Abs(Input.GetAxis("Vertical")) > inputDeadzone ))
					currentStamina -= Time.deltaTime;
			}

			controller.Move(transform.TransformVector(motionVector * Time.fixedDeltaTime));//this is the only call to Move or SimpleMove
		}

		Vector3 octoRelativeIndex = Vector3.zero;

		void UpdateRotation() {
			if (Input.GetAxis("Horizontal") > inputDeadzone)
				xDir = 1;
			if (Input.GetAxis("Horizontal") < -inputDeadzone)
				xDir = -1;
			if (Input.GetAxis("Horizontal") < inputDeadzone && Input.GetAxis("Horizontal") > -inputDeadzone)
				xDir = 0;

			if (Input.GetAxis("Vertical") > inputDeadzone)
				zDir = 1;
			if (Input.GetAxis("Vertical") < -inputDeadzone)
				zDir = -1;
			if (Input.GetAxis("Vertical") < inputDeadzone && Input.GetAxis("Vertical") > -inputDeadzone)
				zDir = 0;

			if (!autoTurn)
					return;

			if (rotateToPointer) {
				mouseDidHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hinfo, 1000f, walkRayMask);
				if (mouseDidHit)
					mouseHitPos = hinfo.point;
				if (controller.isGrounded)
					controller.transform.LookAt(new Vector3(mouseHitPos.x, transform.position.y, mouseHitPos.z));
			}
			else {
				if (controller.isGrounded) {
					if (octodirectional) {
						if (xDir != 0 || zDir != 0) {
							if (xDir == 0 && zDir == 1)
								octoRelativeIndex = new Vector3(0, 0, 0);
							if (xDir == 1 && zDir == 1)
								octoRelativeIndex = new Vector3(0, 45, 0);
							if (xDir == 1 && zDir == 0)
								octoRelativeIndex = new Vector3(0, 90, 0);
							if (xDir == 1 && zDir == -1)
								octoRelativeIndex = new Vector3(0, 135, 0);
							if (xDir == 0 && zDir == -1)
								octoRelativeIndex = new Vector3(0, 180, 0);
							if (xDir == -1 && zDir == -1)
								octoRelativeIndex = new Vector3(0, 225, 0);
							if (xDir == -1 && zDir == 0)
								octoRelativeIndex = new Vector3(0, 270, 0);
							if (xDir == -1 && zDir == 1)
								octoRelativeIndex = new Vector3(0, 315, 0);

							if (Camera.main != null) {
								controller.transform.eulerAngles = (new Vector3(0, Camera.main.transform.eulerAngles.y + octoRelativeIndex.y, 0));
							}

						}
					} else
						controller.transform.RotateAround(transform.position, Vector3.up, (autoTurnSpeed * Input.GetAxis("Horizontal")));
				}
			}
		}

		void UpdateJump() {
			//jump cancellation
			jumpTimer -= Time.fixedDeltaTime;
			if (Input.GetButtonUp("Jump"))
				jumping = false;
			if (jumpTimer <= 0)
				jumping = false;
			if (controller.isGrounded) {
				jumpTimer = extraJumpTime;
				jumping = false;
				if (inAir) {
					inAir = false;//we just landed!
					if (anim != null && !string.IsNullOrEmpty(animatorJumpReturn))
						anim.SetTrigger(animatorJumpReturn);
					if (foleyAudio != null) {
						foleyAudio.pitch = foleyStartingPitch + (Random.Range(-pitchVariance, pitchVariance));
						foleyAudio.PlayOneShot(landingSound);
					}
				}
			}

			//jump activation
			if (controller.isGrounded && Input.GetButton("Jump")) {
				if (!jumping) {
					jumping = true;
					inAir = true;
					if (jumpSound != null) {
						if (foleyAudio != null) {
							foleyAudio.pitch = foleyStartingPitch + (Random.Range(-pitchVariance, pitchVariance));
							foleyAudio.PlayOneShot(jumpSound);
						}
					}
					if (anim != null)
						if (!string.IsNullOrEmpty(animatorJump))
							anim.SetTrigger(animatorJump);
				}
			}

			//jump processing
			if (jumping) {
				if (jumpTimer > 0)
					ySpd = jumpPower * jumpRolloff.Evaluate(jumpTimer / extraJumpTime);//controller.Move(Time.deltaTime * transform.TransformVector(0f, jumpPower * jumpRolloff.Evaluate(jumpTimer/extraJumpTime),0f));
			}
			else {
				if (!controller.isGrounded)
					ySpd = ySpd > Physics.gravity.y ? ySpd += Physics.gravity.y * Time.deltaTime : Physics.gravity.y;
			}
		}

		void UpdatePlatformParent() {
			if (!controller.isGrounded) {
				platform = null;
				if (platformParent)
					transform.SetParent(null);
			}
		}

		void UpdateAttack() {
			attackTimer -= Time.fixedDeltaTime;
			if (inventory != null && inventory.showInventoryGUI)
				return;

				if (useDefaultAttack && attackTimer < 0f) {
				if (Input.GetButtonDown(attackButton) || Input.GetKeyDown(attackKey)) {
					if (anim != null)
						if (!string.IsNullOrEmpty(animatorAttack))
							anim.SetTrigger(animatorAttack);
					MessageManager.Send(attackMessage);

					if (attackableTags.Count > 0) {
						StartCoroutine(ExecuteAttack());
					}

					if (attackSound != null) {
						source.PlayOneShot(attackSound);
					}
					attackTimer = attackCooldown;
				}
			}
		}

		IEnumerator ExecuteAttack() {
			yield return new WaitForSeconds(damageDelay);
			List<GameObject> possibleTargets = new List<GameObject>();
			foreach (string tgtTag in attackableTags)
				possibleTargets.AddRange(GameObject.FindGameObjectsWithTag(tgtTag));
			if (debug)
				Debug.Log("Found " + possibleTargets.Count + " possible targets");
			foreach (GameObject possibleTarget in possibleTargets) {
				if (debug)
					Debug.Log("Target " + possibleTarget.name + " distance is " + Vector3.Distance(possibleTarget.transform.position, transform.position) + "/" + reach + " and arc is " + Vector3.Dot(transform.forward.normalized, (possibleTarget.transform.position - (transform.position + Vector3.up * characterCenterYOffset)).normalized) + "/" + meleeArc);
				if (Vector3.Distance(possibleTarget.transform.position, transform.position) < reach && Vector3.Dot(transform.forward.normalized, (possibleTarget.transform.position - (transform.position + Vector3.up * characterCenterYOffset)).normalized) > meleeArc) {
					if (debug)
						Debug.Log("Sending 'ModifyHealth' and 'Stun' messages to " + possibleTarget.name + " for " + totalDamage + " damage.");
					possibleTarget.SendMessage("ModifyHealth", totalDamage - bonusDamage, SendMessageOptions.DontRequireReceiver);
					bonusDamage = 0;
					if (stunTime > 0)
						possibleTarget.SendMessage("Stun", stunTime,SendMessageOptions.DontRequireReceiver);
				}
			}
		}

		void UpdateCustomActions () {
			if (anim != null) {
				if (inventory != null && inventory.showInventoryGUI)
					return;

					foreach (CustomAction action in customActions) {
					if ((!string.IsNullOrEmpty(action.button) &&  Input.GetButtonDown(action.button) )|| Input.GetKeyDown(action.key)) {
						if (Time.time - action.timeStamp > action.cooldown) {
							action.timeStamp = Time.time;
							if (!string.IsNullOrEmpty(action.mecanimTrigger ))
								anim.SetTrigger(action.mecanimTrigger);

							source.PlayOneShot(action.sound);

							if (!string.IsNullOrEmpty(action.message.message))
								MessageManager.Send(action.message);
						}
}
				}
			}
		}

		void LateUpdate () {
			if (controller.isGrounded) {
				lastGroundVel = transform.InverseTransformVector( controller.velocity);
			}
			transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);

			if (checkForObstructions) {
				RaycastHit _hinfo;
				bool didHit = Physics.Linecast(transform.position, Camera.main.transform.position, out _hinfo, obstructionMask, QueryTriggerInteraction.Ignore);
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
		}

		//parent us to a platform if platform parenting is enabled
		void OnControllerColliderHit ( ControllerColliderHit _hit) {
			if (controller.isGrounded == false)
				return;
			//Debug.Log("" + Vector3.Dot(Vector3.down, (_hit.point - transform.position)));//debug for platform parent sometimes failing
			if (Vector3.Dot(Vector3.down, (_hit.point - transform.position)) >.9f) {
				if (_hit.moveDirection.y < 0.9f && _hit.normal.y > 0.5f) {
					platform = _hit.gameObject;
					if (platformParent)
						transform.SetParent(parentingMode == ParentingModes.RootTransform ? platform.transform.root : platform.transform, true);
				}
			}
		}

		[Header("Available Messages")]
		public MessageHelp stunHelp = new MessageHelp("Stun","Prevents the character from moving or attacking for a specific period of time",3,"The time, in seconds, that the stun should last.");
		public void Stun(float duration) {
			stunDuration = duration;
		}

		public MessageHelp setBonusDamageHelp = new MessageHelp("SetBonusDamage", "Adds additional damage to the next attack. Resets to 0 after attack", 3, "How much additional damage should we add?");
		public void SetBonusDamage(float dmg) {
			if (debug)
				Debug.Log("Setting bonus damage to " + dmg);
			bonusDamage = dmg;
		}
	}
}