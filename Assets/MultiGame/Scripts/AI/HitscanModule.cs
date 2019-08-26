using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/AI/Hitscan Module")]
	public class HitscanModule : MultiModule {

		[Header("Targeting Parameters")]
		[Tooltip("How far above or below the target object's origin should we look for the 'center' of the object?")]
		public float tgtYOffset = .5f;
		[Tooltip("How wide should the target cone be? We must be facing within this percentage of a 180 degree FOV of the target position + Tgt Y Offset to apply damage")]
		[Range(0,1)]
		public float headingConeRadius = .35f;

		[Header("Combat Settings")]
		[RequiredFieldAttribute("Trigger in the Animator Controller on this object to be sent when the AI attacks", RequiredFieldAttribute.RequirementLevels.Recommended)]
		public string attackAnimationTrigger;
		[Tooltip("Cooldown duration")]
		public float cooldownDuration = 1.0f;
		[Tooltip("How long do we wait after starting the attack animation to actually apply the damage?")]
		public float damageDelay = .5f;
		[Tooltip("Damage per hit")]
		public float attackDamage = 10.0f;
		[Tooltip("If greater than 0, will stun it's target for this duration in seconds")]
		public float stunTime = 0;
		[RequiredFieldAttribute("An object representing a raycast where the damage starts. Should be an empty transform slightly in front of the character. Raycasts from this point to prevent damage through walls etc.", RequiredFieldAttribute.RequirementLevels.Required)]
		public GameObject damageRayOrigin;
		[Tooltip("Range of the attack")]
		public float hitscanRange = 2.3f;
		[Tooltip("What collision layers can block our attacks & vision?")]
		public LayerMask damageObstructionMask;
		[Tooltip("How long do we keep hunting our target after we can't see them anymore?")]
		public float huntingTimeOut = 10;
		//private float lastTriggerTime;
		[Header("Message Senders")]
		[ReorderableAttribute]
		[Tooltip("Messages to send when damage is dealt")]
		public List<MessageManager.ManagedMessage> attackMessages = new List<MessageManager.ManagedMessage>();
		[ReorderableAttribute]
		[Tooltip("Messages sent to the victim when damage is dealt")]
		public List<MessageManager.ManagedMessage> messagesToVictim = new List<MessageManager.ManagedMessage>();
		[ReorderableAttribute]
		[Tooltip("Messages sent if we miss the target")]
		public List<MessageManager.ManagedMessage> missMessages = new List<MessageManager.ManagedMessage>();

		[Header("Audio Settings")]
		[RequiredField("A separate audio source for combat can be specified, so that movement sound is not interrupted",RequiredFieldAttribute.RequirementLevels.Recommended)]
		public AudioSource source;
		[Reorderable]
		public List<AudioClip> attackSounds = new List<AudioClip>();
		[Reorderable]
		public List<AudioClip> hitSounds = new List<AudioClip>();
		[Reorderable]
		public List<AudioClip> missSounds = new List<AudioClip>();

		private bool targetVisible = false;
		private float lastSeenTime = 0;
		private bool hunting = false;
		private List<GameObject> touchingObjects = new List<GameObject>();
		private GameObject target;
		[System.NonSerialized]
		public Animator anim;
		private float attackCounter;
		private float stunDuration = 0;
#if UNITY_EDITOR

		public HelpInfo help =  new HelpInfo("This component should be placed on an empty object representing an AI. The object should have a 3D model of a unit parented to it." +
			"\nWe also recommend adding a NavModule or similar, so it can get around, and some sort of AI 'brain' such as a Guard or Minion Module. Make sure to set up all settings" +
			" such as ray mask (so that it can't shoot or hit through walls)");
#endif
		[Tooltip("Send messages to the console?")]
		public bool debug = false;

		void Start () {
			if (source == null)
				source = GetComponent<AudioSource>();
			if (anim == null)
				anim = GetComponentInChildren<Animator>();
			if (damageRayOrigin == null) {
				Debug.LogError("Melee Module " + gameObject.name + " requires a Damage Ray Origin to cast an attack ray into the scene");
				enabled = false;
				return;
			}
		}

		private void OnDisable() {
			attackCounter = 0;
			stunDuration = 0;
			target = null;
			touchingObjects.Clear();
		}

		void Stun(float duration) {
			stunDuration = duration;
		}

		void OnValidate () {
			for (int i = 0; i < attackMessages.Count; i++) {
				MessageManager.ManagedMessage _msg = attackMessages[i];
				MessageManager.UpdateMessageGUI(ref _msg, gameObject);
			}
			for (int i = 0; i < messagesToVictim.Count; i++) {
				MessageManager.ManagedMessage _msg = messagesToVictim[i];
				MessageManager.UpdateMessageGUI(ref _msg, gameObject);
			}
			for (int i = 0; i < missMessages.Count; i++) {
				MessageManager.ManagedMessage _msg = missMessages[i];
				MessageManager.UpdateMessageGUI(ref _msg, gameObject);
			}
		}

		void OnCollisionEnter (Collision collision) {
			touchingObjects.Add(collision.gameObject);
		}

		void OnCollisionExit (Collision collision) {
			touchingObjects.Remove(collision.gameObject);
		}

		void FixedUpdate () {
			stunDuration -= Time.deltaTime;
			if (stunDuration > 0) {
				if (anim != null)
					anim.speed = 0;
				return;
			}
			else {
				if (anim != null)
					anim.speed = 1;
			}
			if (target != null) {

				Vector3 tgtPos = new Vector3(target.transform.position.x, target.transform.position.y + tgtYOffset, target.transform.position.z);
				Vector3 toOther = tgtPos - transform.position;

				float heading = Vector3.Dot(transform.TransformDirection(Vector3.forward), toOther);
				targetVisible = !Physics.Linecast(damageRayOrigin.transform.position, tgtPos, damageObstructionMask, QueryTriggerInteraction.Ignore);
				if (targetVisible)
					lastSeenTime = Time.time;

				if (hunting) {
					if (Time.time - lastSeenTime > huntingTimeOut) {
						target = null;
						hunting = false;
						return;
					}
				}

				if (debug) {
					if (heading < headingConeRadius || !targetVisible)
						Debug.DrawLine(damageRayOrigin.transform.position, tgtPos, XKCDColors.RedOrange);//Debug.DrawRay(damageRayOrigin.transform.position, damageRayOrigin.transform.TransformDirection(Vector3.forward));
					else {
						if (Vector3.Distance(damageRayOrigin.transform.position, tgtPos) <= hitscanRange)
							Debug.DrawLine(damageRayOrigin.transform.position, tgtPos, XKCDColors.YellowGreen);
						else
							Debug.DrawLine(damageRayOrigin.transform.position, tgtPos, Color.white);
					}
				}
				attackCounter -= Time.deltaTime;
				if (attackCounter < 0) {
					ActivateAttack(heading);
				}

			} else {//target is null
				targetVisible = false;
			}
		}

		void ActivateAttack (float headingToTarget) {
			if (!gameObject.activeInHierarchy)
				return;

			if (target!= null) {//target found!
				if (debug)
					Debug.Log("Melee Module " + gameObject.name + " has a target.");

				attackCounter = cooldownDuration;
				Vector3 targetPosition = target.transform.position;

				if (targetVisible) {
					if (headingToTarget > headingConeRadius && Vector3.Distance(damageRayOrigin.transform.position, targetPosition) <= hitscanRange) {//target is within damage cone and in-range!
						hunting = false;
						foreach (MessageManager.ManagedMessage myMsg in attackMessages) {
							MessageManager.Send(myMsg);//nullref error??
						}
						if (anim != null) {
							if (debug)
								Debug.Log("Melee Module " + gameObject.name + " is in range and looking towards the target " + target.name);
							if (!string.IsNullOrEmpty(attackAnimationTrigger))
								anim.SetTrigger(attackAnimationTrigger);
						}
						if (source != null && attackSounds.Count > 0)
							source.PlayOneShot(attackSounds[Mathf.FloorToInt(Random.Range(0, attackSounds.Count))]);
						StartCoroutine(DamageOther());
					}
				}
			}
		}

		IEnumerator DamageOther() {
			yield return new WaitForSeconds(damageDelay);
			if (stunDuration < 0) {
				Vector3 targetPosition = target.transform.position;
				RaycastHit _hinfo;
				if (target != null) {

					if (!Physics.Linecast(damageRayOrigin.transform.position, targetPosition, out _hinfo, damageObstructionMask, QueryTriggerInteraction.Ignore) && Vector3.Distance(damageRayOrigin.transform.position, targetPosition) <= hitscanRange && Vector3.Dot(transform.TransformDirection(Vector3.forward), targetPosition - transform.position) > headingConeRadius) {
						if (debug)
							Debug.Log("Melee Module " + gameObject.name + " is applying damage to " + target.name + " at a range of " + Vector3.Distance(damageRayOrigin.transform.position, targetPosition));

						foreach (MessageManager.ManagedMessage otherMsg in messagesToVictim) {
							MessageManager.SendTo(otherMsg, target);
						}
						
						target.gameObject.SendMessage("ModifyHealth", -attackDamage, SendMessageOptions.DontRequireReceiver);
						if (stunTime > 0)
							target.gameObject.SendMessage("HitStun", stunTime, SendMessageOptions.DontRequireReceiver);
						if (source != null && hitSounds.Count > 0)
							source.PlayOneShot(hitSounds[Mathf.FloorToInt(Random.Range(0, hitSounds.Count))]);
					}
					else {
						if (debug)
							Debug.Log("Melee Module " + gameObject.name + " missed target " + target.name);
						if (source != null && missSounds.Count > 0)
							source.PlayOneShot(missSounds[Mathf.FloorToInt(Random.Range(0, missSounds.Count))]);
						foreach (MessageManager.ManagedMessage missMsg in missMessages) {
							MessageManager.Send(missMsg);
						}
					}
				}
			}
		}

		public MessageHelp targetNearestHelp = new MessageHelp("TargetNearest","Targets the nearest object of a given tag",4,"The tag of the object we wish to target");
		public void TargetNearest(string _targetTag) {
			GameObject _nearest = FindClosestByTag(_targetTag);
			if (_nearest != null)
				SetTarget(_nearest);
		}

		void SetTarget(GameObject newTarget) {
			if (debug)
				Debug.Log("HitscanModule " + gameObject.name + " received the target " + newTarget.name);
			if (!gameObject.activeInHierarchy)
				return;
			target = newTarget;
		}

		void Hunt() {
			hunting = true;
		}

		public void StopHunting() {
			target = null;
			hunting = false;
		}

		public MessageHelp clearTargetHelp = new MessageHelp("ClearTarget","Causes the Hitscan Module to stop targeting anything.");
		public void ClearTarget() {
			target = null;
		}

		void ReturnFromPool() {
			ClearTarget();
			gameObject.SendMessage("FaceTarget");
		}
	}
}