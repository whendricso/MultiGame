using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleeModule : MonoBehaviour {

	public float attackTime = 1.0f;
	public float attackDamage = 10.0f;
	private float damageCounter;
	public GameObject damageRayOrigin;
	public float meleeRange = 0.8f;
	public LayerMask damageRayMask;
	private float lastTriggerTime;
	public List<MessageManager.ManagedMessage> attackMessages = new List<MessageManager.ManagedMessage>();
	public List<MessageManager.ManagedMessage> messagesToVictim = new List<MessageManager.ManagedMessage>();

	public bool debug = false;

	void Start () {
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
		bool _didHit = Physics.Raycast(damageRayOrigin.transform.position, damageRayOrigin.transform.TransformDirection(Vector3.forward), out _hinfo, meleeRange, damageRayMask);
		if(_didHit) {
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
