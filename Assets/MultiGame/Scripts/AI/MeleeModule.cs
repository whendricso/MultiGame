using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleeModule : MultiModule {

	[System.NonSerialized]
	public Animator anim;
	public string attackAnimationTrigger;

	[Tooltip("Cooldown duration")]
	public float attackTime = 1.0f;
	public float attackDamage = 10.0f;
	private float damageCounter;
	[Tooltip("An object representing a raycast where the damage starts. Should be an empty transform slightly in front of the character. Raycasts from this point to prevent damage through walls etc.")]
	public GameObject damageRayOrigin;
	[Tooltip("Range of the attack")]
	public float meleeRange = 0.8f;
	[Tooltip("What collision layers can we hit?")]
	public LayerMask damageRayMask;
	private float lastTriggerTime;
	[Tooltip("Messages to send when damage is dealt")]
	public List<MessageManager.ManagedMessage> attackMessages = new List<MessageManager.ManagedMessage>();
	[Tooltip("Messages sent to the victim when damage is dealt")]
	public List<MessageManager.ManagedMessage> messagesToVictim = new List<MessageManager.ManagedMessage>();

	private List<GameObject> touchingObjects = new List<GameObject>();

	public HelpInfo help =  new HelpInfo("This component should be placed on an empty object representing an AI. The object should have a 3D model of a melee unit parented to it." +
		"\nWe also recommend adding a NavModule or similar, so it can get around, and some sort of AI 'brain' such as a Guard or Minion Module. Make sure to set up all settings" +
		" such as ray mask (cannot damage things not part of this mask)");

	[Tooltip("Send messages to the console?")]
	public bool debug = false;

	void Start () {
		if (anim == null)
			anim = GetComponent<Animator>();
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
	}

	void OnCollisionEnter (Collision collision) {
		touchingObjects.Add(collision.gameObject);
	}

	void OnCollisionExit (Collision collision) {
		touchingObjects.Remove(collision.gameObject);
	}

	void Update () {
		if (debug)
			Debug.DrawRay(damageRayOrigin.transform.position, damageRayOrigin.transform.TransformDirection(Vector3.forward));
		damageCounter -= Time.deltaTime;
		if (damageCounter < 0) {
			ApplyDamage();
		}
	}

	void ApplyDamage () {
		damageCounter = attackTime;

		RaycastHit _hinfo;
		bool _didHit = Physics.Raycast(damageRayOrigin.transform.position, transform.TransformDirection(transform.forward), out _hinfo, meleeRange, damageRayMask);

		if (!_didHit) {
			GameObject _closest = null;
			float _dist = Mathf.Infinity;
			float _bestDist = Mathf.Infinity;
			GameObject _obj;
			for (int i = 0; i < touchingObjects.Count; i++) {
				_obj = touchingObjects[i];
				if (_obj != null) {
					_didHit = Physics.Linecast(damageRayOrigin.transform.position, _obj.transform.position, out _hinfo, damageRayMask);
					_dist = Vector3.Distance(damageRayOrigin.transform.position, _hinfo.point);
					if (_dist < _bestDist) {
						_closest = _obj;
						_bestDist = _dist;
					}
				}
				else {
					touchingObjects.RemoveAt(i);
				}
			}
			if (_closest != null) {
				if (anim != null && !string.IsNullOrEmpty(attackAnimationTrigger))
					anim.SetTrigger(attackAnimationTrigger);
				_closest.SendMessage("ModifyHealth", -attackDamage, SendMessageOptions.DontRequireReceiver);
				foreach (MessageManager.ManagedMessage _msg in attackMessages) {
					MessageManager.Send(_msg);
				}
				foreach (MessageManager.ManagedMessage _msg in messagesToVictim) {
					MessageManager.SendTo(_msg, _hinfo.collider.gameObject);
				}
			}

		}
		else {
			if (anim != null && !string.IsNullOrEmpty(attackAnimationTrigger))
				anim.SetTrigger(attackAnimationTrigger);
			_hinfo.collider.gameObject.SendMessage("ModifyHealth", -attackDamage, SendMessageOptions.DontRequireReceiver);
			foreach (MessageManager.ManagedMessage _msg in attackMessages) {
				MessageManager.Send(_msg);
			}
			foreach (MessageManager.ManagedMessage _msg in messagesToVictim) {
				MessageManager.SendTo(_msg, _hinfo.collider.gameObject);
			}
		}
		if (debug) {
			if (_didHit)
				Debug.Log("Melee Module " + gameObject.name + " is applying damage to " + _hinfo.collider.gameObject.name);
			else
				Debug.Log("Melee Module " + gameObject.name + " did not find a damage target!");
		}
	}
}
