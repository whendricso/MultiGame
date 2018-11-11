using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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


		[Header("Motion")]
		[Tooltip("If true, will auto-parent to the object it stands on, for moving platform support.")]
		public bool platformParent = true;
		[RequiredFieldAttribute("Moving forward, how fast can the character go at full sprint?")]
		public float runSpeed = 7f;
		[RequiredFieldAttribute("How fast are we able to move sideways?")]
		public float strafeSpeed = 4.5f;
		[RequiredFieldAttribute("How fast can we move backwards?")]
		public float backpedalSpeed = 3f;
		[RequiredFieldAttribute("When rotating based on direction, how fast can we spin (in degrees per second)?")]
		public float autoTurnSpeed = 8f;
		public float maxZVelocity = 10f;
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

		[ReorderableAttribute]
		[Header("Custom")]
		[Tooltip("Custom input actions which respond to a key and/or button press. When activated, any supplied parameters for that CustomAction will be used and the rest ignored.")]
		public List<CustomAction> customActions = new List<CustomAction>();

		float totalDamage = 0;
		float reach = 0;
		float meleeArc = 0;
		float stunTime = 0;

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
		private bool footFall = false;
		private float footFallCounter = 0;
		private GameObject platform;//if non-null, we're standing on something, and should move if it moves
		private float stunDuration = 0;
		private float bonusDamage = 0;

		private float foleyStartingPitch = 1;

		List<MeleeWeapon> weapons = new List<MeleeWeapon>();

		float xSpd = 0;
		float ySpd = 0;
		float zSpd = 0;
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

		void Stun(float duration) {
			stunDuration = duration;
		}

		void Start () {
			if (foleyAudio != null)
				foleyStartingPitch = foleyAudio.pitch;
			if (attackMessage.target == null)
				attackMessage.target = gameObject;
			rigid = GetComponent<Rigidbody>();
			jumpTimer = extraJumpTime;
			if (walkRayMask == LayerMask.GetMask("None"))
				walkRayMask = LayerMask.GetMask("Default");

			if (rigid != null) {
				rigid.isKinematic = true;
				rigid.useGravity = false;
			}
			controller = GetComponent<CharacterController>();
			anim = GetComponentInChildren<Animator>();
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
		}

		void FixedUpdate () {
			UpdatePlatformParent();
			stunDuration -= Time.deltaTime;
			if (stunDuration > 0) {
				anim.speed = 0;
				return;
			}
			else
				anim.speed = 1;
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
				if (weapon.arc > meleeArc)
					meleeArc = weapon.arc;
				if (weapon.stunTime > stunTime)
					stunTime = weapon.stunTime;
				if (weapon.reach > reach)
					reach = weapon.reach;
				damageDelay = weapon.damageDelay;
			}
		}

		/// <summary>
		/// After everything else is processed, update and apply the motion vector
		/// </summary>
		void UpdateMotion() {
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
			}
			if (Input.GetAxis("Vertical") > 0f) {
				adjustedVertical = Input.GetAxis("Vertical") * runSpeed;
				if (foleyAudio != null && Input.GetAxis("Vertical") > minRunThreshold) {
					footFall = true;
					footFallCounter -= Time.deltaTime;
					if (footFallCounter < 0) {
						footFallCounter = footstepInterval;
						foleyAudio.pitch = foleyStartingPitch + Random.Range(-pitchVariance, pitchVariance);
						if (controller.isGrounded)
							foleyAudio.PlayOneShot(runSound);
					}
				}
				else {
					footFall = false;
				}
			}
			else
				adjustedVertical = Input.GetAxis("Vertical") * backpedalSpeed;

			if (rotateToPointer)
				xSpd = Input.GetAxis("Horizontal") * strafeSpeed;
			else
				xSpd = 0;
			zSpd = adjustedVertical;
			if (rotateToPointer) {
				if (Vector3.Distance(transform.position, mouseHitPos) > deadzone)
					motionVector.z = controller.isGrounded ? zSpd : (trueVelocity.z < maxZVelocity ? lastGroundVel.z + zSpd * airControlMultiplier : maxZVelocity);
				else
					motionVector.z = 0;
			}
			else
				motionVector.z = controller.isGrounded ? zSpd : (trueVelocity.z < maxZVelocity ? lastGroundVel.z + zSpd * airControlMultiplier : maxZVelocity);

			motionVector.x = controller.isGrounded ? xSpd : (trueVelocity.x < maxXVelocity ? lastGroundVel.x + xSpd * airControlMultiplier : maxXVelocity);
			motionVector.y = ySpd;
			controller.Move(transform.TransformVector(motionVector * Time.fixedDeltaTime));//this is the only call to Move or SimpleMove
		}

		void UpdateRotation() {
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
				if (controller.isGrounded)
					controller.transform.RotateAround(transform.position, Vector3.up, (autoTurnSpeed * Input.GetAxis("Horizontal")));
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
					foleyAudio.pitch = foleyStartingPitch + (Random.Range(-pitchVariance, pitchVariance));
					foleyAudio.PlayOneShot(landingSound);
				}
			}

			//jump activation
			if (controller.isGrounded && Input.GetButton("Jump")) {
				if (!jumping) {
					jumping = true;
					inAir = true;
					if (jumpSound != null) {
						foleyAudio.pitch = foleyStartingPitch + (Random.Range(-pitchVariance, pitchVariance));
						foleyAudio.PlayOneShot(jumpSound);
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
					possibleTarget.SendMessage("ModifyHealth", totalDamage - bonusDamage);
					bonusDamage = 0;
					if (stunTime > 0)
						possibleTarget.SendMessage("Stun", stunTime);
				}
			}
		}

		void UpdateCustomActions () {
			if (anim != null) {
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
			if (Vector3.Dot(Vector3.down, (_hit.point - transform.position)) >.9f) {
				if (_hit.moveDirection.y < 0.9f && _hit.normal.y > 0.5f) {
					platform = _hit.gameObject;
					if (platformParent)
						transform.SetParent(platform.transform.root, true);
				}
			}
		}

		public MessageHelp setBonusDamageHelp = new MessageHelp("SetBonusDamage", "Adds additional damage to the next attack. Resets to 0 after attack", 3, "How much additional damage should we add?");
		public void SetBonusDamage(float dmg) {
			if (debug)
				Debug.Log("Setting bonus damage to " + dmg);
			bonusDamage = dmg;
		}
	}
}