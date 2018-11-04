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
		[Tooltip("How wide should the target cone be? We must be facing withing this number of degrees of the target position + Tgt Y Offset to apply damage")]
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
		[RequiredFieldAttribute("An object representing a raycast where the damage starts. Should be an empty transform slightly in front of the character. Raycasts from this point to prevent damage through walls etc.", RequiredFieldAttribute.RequirementLevels.Required)]
		public GameObject damageRayOrigin;
		[Tooltip("Range of the attack")]
		public float hitscanRange = 2.3f;
		[Tooltip("What collision layers can block our attacks?")]
		public LayerMask damageObstructionMask;
		private float lastTriggerTime;
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

		private List<GameObject> touchingObjects = new List<GameObject>();
		private GameObject target;
		[System.NonSerialized]
		public Animator anim;
		private float attackCounter;
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
			if (target != null) {
				Vector3 tgtPos = new Vector3(target.transform.position.x, target.transform.position.y + tgtYOffset, target.transform.position.z);
				Vector3 toOther = tgtPos - transform.position;

				float heading = Vector3.Dot(transform.TransformDirection(Vector3.forward), toOther);

				if (debug) {
					if (heading < headingConeRadius || Physics.Linecast(damageRayOrigin.transform.position, tgtPos, damageObstructionMask, QueryTriggerInteraction.Ignore))
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
			}
		}

		void ActivateAttack (float headingToTarget) {
			attackCounter = cooldownDuration;
			Vector3 targetPosition = target.transform.position;

			if (target!= null) {//target found!
				if (debug)
					Debug.Log("Melee Module " + gameObject.name + " has a target.");
				if (headingToTarget > headingConeRadius && Vector3.Distance(damageRayOrigin.transform.position, targetPosition) <= hitscanRange) {//target is within damage cone and in-range!
					if (anim != null && !string.IsNullOrEmpty(attackAnimationTrigger)) if (debug)
							Debug.Log("Melee Module " + gameObject.name + " is in range and looking towards the target " + target.name);
					anim.SetTrigger(attackAnimationTrigger);
					if (source != null && attackSounds.Count > 0)
						source.PlayOneShot(attackSounds[Mathf.FloorToInt(Random.Range(0, attackSounds.Count))]);
					StartCoroutine(DamageOther());
				}
			}
		}

		IEnumerator DamageOther() {
			yield return new WaitForSeconds(damageDelay);
			Vector3 targetPosition = target.transform.position;
			RaycastHit _hinfo;
			if (target != null) {
				
				if (!Physics.Linecast(damageRayOrigin.transform.position, targetPosition, out _hinfo, damageObstructionMask, QueryTriggerInteraction.Ignore) && Vector3.Distance(damageRayOrigin.transform.position, targetPosition) <= hitscanRange && Vector3.Dot(transform.TransformDirection(Vector3.forward), targetPosition - transform.position) > headingConeRadius) {
					if (debug)
						Debug.Log("Melee Module " + gameObject.name + " is applying damage to " + target.name + " at a range of " + Vector3.Distance(damageRayOrigin.transform.position, targetPosition));

					foreach (MessageManager.ManagedMessage otherMsg in messagesToVictim) {
						MessageManager.SendTo(otherMsg, target);
					}
					foreach (MessageManager.ManagedMessage myMsg in attackMessages) {
						MessageManager.Send(myMsg);
					}
					target.gameObject.SendMessage("ModifyHealth", -attackDamage, SendMessageOptions.DontRequireReceiver);
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

		void SetTarget(GameObject newTarget) {
			target = newTarget;
		}

		void ClearTarget() {
			target = null;
		}
	}
}