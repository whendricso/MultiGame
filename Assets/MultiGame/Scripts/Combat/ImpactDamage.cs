﻿using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Rigidbody))]
public class ImpactDamage : MonoBehaviour {

	public float damage = 10.0f;
	public enum TargetingMode {Self, Other, Both};
	public TargetingMode targetingMode = TargetingMode.Both;
	public float speedThreshold = 20.0f;
	public Health health;

	void Start () {
		if (collider == null || rigidbody == null) {
			Debug.Log ("Impact Damage " + gameObject.name + " needs a rigidbody and collider!");
			enabled = false;
			return;
		}
		if (health == null)
			health = GetComponent<Health>();
		if (health == null) {
			Debug.Log ("Impact Damage " + gameObject.name + " needs a health component!");
			enabled = false;
			return;
		}
	}
	
	void OnCollisionEnter (Collision collision) {
		if (collision.relativeVelocity.magnitude >= speedThreshold) {
			Debug.Log ("Apply impact damage");
			if (targetingMode == TargetingMode.Self || targetingMode == TargetingMode.Both)
				gameObject.BroadcastMessage("ModifyHealth", damage * collision.relativeVelocity.magnitude, SendMessageOptions.DontRequireReceiver);
			if (targetingMode == TargetingMode.Other || targetingMode == TargetingMode.Both)
				collision.gameObject.BroadcastMessage("ModifyHealth", damage * collision.relativeVelocity.magnitude, SendMessageOptions.DontRequireReceiver);
			
		}
	}
}
