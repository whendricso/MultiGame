using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	[RequireComponent(typeof (CharacterController))]
	[RequireComponent(typeof (AudioSource))]
	public class CharacterOmnicontroller : MultiModule {

		public enum RelativityModes {Character, Camera };
//		public RelativityModes relativityMode = RelativityModes.Camera;

		[Header("Important - Must be populated")]
		[Tooltip("What physics layers can the player walk on?")]
		public LayerMask walkRayMask;

		[Header("Input")]
		[RequiredFieldAttribute("How close do we stop when chasing the pointer?")]
		public float deadzone = .24f;
		[Tooltip("Should this component rotate the character to face the mouse pointer using a raycast into the scene?")]
		public bool rotateToPointer = false;
		[RequiredFieldAttribute("How fast is our jump?")]
		public float jumpPower = 6f;
		[RequiredFieldAttribute("How long can the player continue holding jump to increase the height?")]
		public float extraJumpTime = .8f;
		[Tooltip("How much does the jump slow over time? This defines the arc of the jump while moving up. Gravity defines the fall.")]
		public AnimationCurve jumpRolloff;

		[Header("Motion")]
		[Tooltip("If true, will auto-parent to the object it stands on, for moving platform support.")]
		public bool platformParent = true;
		[RequiredFieldAttribute("Moving forward, how fast can the character go at full sprint?")]
		public float runSpeed = 7f;
		[RequiredFieldAttribute("How fast are we able to move sideways?")]
		public float strafeSpeed = 4.5f;
		[Tooltip("Should this component enforce a rotation on the character?")]
		public bool autoTurn = false;
		[RequiredFieldAttribute("When rotating based on direction, how fast can we spin?")]
		public float autoTurnSpeed = 8f;
		[RequiredFieldAttribute("How fast can we move backwards?")]
		float backpedalSpeed = 3f;
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
		[Tooltip("Looping footstep sound")]
		public AudioClip runSound;
		[Tooltip("Minimum vertical input (between 0 and 1) needed to hear the footfall sound.")]
		public float minRunThreshold = .3f;
		[Tooltip("Time, in seconds, between footfalls")]
		public float footstepInterval = .35f;
		[Tooltip("Looping footstep sound")]
		public AudioClip strafeSound;
		[Tooltip("Played when we begin jumping")]
		public AudioClip jumpSound;
		[Tooltip("Played when we land")]
		public AudioClip landingSound;

		[Header("Combat")]
		[Tooltip("Should we use a default attack for this character? You can disable this if you are using another system for melee attacks.")]
		public bool useDefaultAttack = true;
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
		//		[Tooltip("How much do we vary the pitch of the sound? Small values add realism.")]
		//		[Range(0f,.99f)]
		//		public float attackSoundVariance = 0f;
		[RequiredFieldAttribute("Name of a trigger in the attached Animator, if any.", RequiredFieldAttribute.RequirementLevels.Recommended)]
		public string animatorFire = "Fire";
		[Tooltip("Played during the character's built-in ranged attack")]
		public AudioClip fireSound;
		[Tooltip("How much do we vary the pitch of the sound? Small values add realism.")]
		public float fireSoundVariance = 0f;

		[ReorderableAttribute]
		[Header("Custom")]
		[Tooltip("Custom input actions which respond to a key and/or button press. When activated, any supplied parameters for that CustomAction will be used and the rest ignored.")]
		public List<CustomAction> customActions = new List<CustomAction>();

		private bool jumping = false;
		private float jumpTimer;
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

		private bool inAir = false;
		private float adjustedVertical = 0f;
//		private float originalPitch;
		private float attackTimer = 0f;
		private Vector3 lastGroundVel;
//		private Vector3 previousPlatformPosition = Vector3.zero;
		private Rigidbody rigid;
		private RaycastHit hinfo;
//		private bool didHit = false;
		private CharacterController controller;
		private Animator anim;
		private AudioSource source;
		private bool footFall = false;
		private GameObject platform;//if non-null, we're standing on something, and should move if it moves

		public HelpInfo help = new HelpInfo("Character Omnicontroller is an all-purpose player input controller which allows for various third person perspectives as well as first person " +
			"perspective controllers. Try using it with 'Auto Rotate' disabled and add a 'MouseAim' component for FPS style. If the character has an Animator component anywhere in it's heirarchy, this controller" +
			" will send animation commands to it, trigger names are set above. To use for third person, make sure you have a camera in your scene that is either positioned carefully or follows the player. " +
			"For top-down games we recommend using 'Rotate To Pointer' to give an intuitive feel.");

		void OnValidate () {
			foreach (CustomAction action in customActions) {
				MessageManager.UpdateMessageGUI( ref action.message, gameObject);
			}
		}

		void Start () {
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
			lastGroundVel = controller.velocity;
//			originalPitch = source.pitch;

			foreach (CustomAction action in customActions) {
				action.timeStamp = Time.time;
				if (action.message.target == null)
					action.message.target = gameObject;
			}
		}

		void FixedUpdate () {
			attackTimer -= Time.deltaTime;

			if (!controller.isGrounded) {
				platform = null;
				if (platformParent)
					transform.SetParent(null);
			}
			


			if (useDefaultAttack && attackTimer < 0f) {
				if (Input.GetButtonDown(attackButton) || Input.GetKeyDown(attackKey)) {
					if (anim != null)
					if (!string.IsNullOrEmpty(animatorAttack))
							anim.SetTrigger(animatorAttack);
					if (attackSound != null) {
//						source.pitch = Random.Range (originalPitch - attackSoundVariance, originalPitch + attackSoundVariance);
						source.PlayOneShot(attackSound);
					}
					attackTimer = attackCooldown;
				}
			}

			if (anim != null) {
				if (!string.IsNullOrEmpty(animatorRun))
					anim.SetFloat(animatorRun, Input.GetAxis("Vertical"));
				if (!string.IsNullOrEmpty(animatorStrafe))
					anim.SetFloat(animatorStrafe, Input.GetAxis("Horizontal"));

				foreach (CustomAction action in customActions) {
					if ((!string.IsNullOrEmpty( action.button) &&  Input.GetButtonDown(action.button) )|| Input.GetKeyDown(action.key)) {
						if (Time.time - action.timeStamp > action.cooldown) {
							action.timeStamp = Time.time;
							if (!string.IsNullOrEmpty( action.mecanimTrigger ))
								anim.SetTrigger(action.mecanimTrigger);

							source.PlayOneShot(action.sound);

							if (!string.IsNullOrEmpty( action.message.message))
								MessageManager.Send(action.message);
						}
					}
				}


			}

			if (Input.GetButtonUp("Jump"))
				jumping = false;
			
			jumpTimer -= Time.deltaTime;

			if (jumpTimer <= 0)
				jumping = false;

			if (controller.isGrounded) {
				jumpTimer = extraJumpTime;
				jumping = false;

			} else {
				controller.Move(Time.deltaTime * transform.TransformVector(Input.GetAxis("Horizontal") * (airControlMultiplier * strafeSpeed), 0f, Input.GetAxis("Vertical") * (runSpeed * airControlMultiplier)));

			}

			if (jumping) {
				if (jumpTimer > 0)
					controller.Move(Time.deltaTime * transform.TransformVector(0f, jumpPower * jumpRolloff.Evaluate(jumpTimer/extraJumpTime),0f));
			}

			if ( controller.isGrounded && Input.GetButton("Jump")) {
				if (!jumping) {
					inAir = true;
					jumping = true;
					if (jumpSound != null)
						source.PlayOneShot(jumpSound);
					if (anim != null)
					if (!string.IsNullOrEmpty(animatorJump))
							anim.SetTrigger(animatorJump);
				}
				controller.Move(Time.deltaTime * new Vector3(0f, jumpPower, 0f));
			}

			if (autoTurn && rotateToPointer) {
				/*didHit = */Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hinfo, 1000f, walkRayMask);
			}
				
			if (Input.GetAxis("Vertical") > 0f) {
				adjustedVertical = Input.GetAxis("Vertical") * runSpeed;

				if (Input.GetAxis("Vertical") > minRunThreshold) {
					if (!footFall) {
						StartCoroutine(FootFall());
						footFall = true;
					} else {
						footFall = false;
					}
				}
					
			}
			else
				adjustedVertical = Input.GetAxis("Vertical") * backpedalSpeed;
			
			if (!autoTurn) {
				if (!jumping)
					controller.SimpleMove(/*Time.deltaTime */  transform.TransformVector( new Vector3( Input.GetAxis("Horizontal") * strafeSpeed, 0f, adjustedVertical)));
				else
					controller.Move(Time.deltaTime *  transform.TransformVector(new Vector3( Input.GetAxis("Horizontal") * strafeSpeed, 0f, adjustedVertical)));
			}
			else {
				if (!rotateToPointer) {
					if (!jumping) {
						controller.transform.RotateAround(transform.position, Vector3.up, /*Time.deltaTime */ (autoTurnSpeed * Input.GetAxis("Horizontal")));
						controller.SimpleMove(/*Time.deltaTime */ transform.TransformVector( new Vector3( 0f, 0f, adjustedVertical)));
					} else {
						controller.Move(Time.deltaTime *  transform.TransformVector( lastGroundVel.x,0f, lastGroundVel.z));
					}
				}
				else {
					if (Vector3.Distance(transform.position, hinfo.point) > deadzone) {
						if (controller.isGrounded)
							controller.transform.LookAt(new Vector3( hinfo.point.x, transform.position.y, hinfo.point.z));
						if (!jumping)
							controller.SimpleMove(/*Time.deltaTime */ transform.TransformVector( new Vector3( Input.GetAxis("Horizontal") * strafeSpeed, 0f, adjustedVertical)));
						else {
							controller.Move(Time.deltaTime * transform.TransformVector( lastGroundVel.x, 0f,lastGroundVel.z));
						}
					}
				}
			}

			if (controller.isGrounded && inAir) {
				inAir = false;
				if (landingSound != null)
					source.PlayOneShot(landingSound);
			}
			if (controller.isGrounded)
				lastGroundVel = transform.InverseTransformVector( controller.velocity);
		}

		void LateUpdate () {
			//constrain the character to always stand up, in case our platform rotates in a weird way
			transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
		}

		void OnControllerColliderHit ( ControllerColliderHit _hit) {
			if (controller.isGrounded == false)
				return;
			if (_hit.moveDirection.y < 0.9f && _hit.normal.y > 0.5f) {
				platform = _hit.gameObject;
//				previousPlatformPosition = platform.transform.position;
				if (platformParent)
					transform.SetParent(platform.transform.root, true);
			}
		}
			
		public IEnumerator FootFall () {

			source.PlayOneShot(runSound);

			if (footFall) {
				yield return new WaitForSeconds(footstepInterval);
				StartCoroutine(FootFall());
			}
		}


	}
}