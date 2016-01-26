using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[RequireComponent (typeof(Rigidbody))]
	public class ImpactDamage : MultiModule {

		[Tooltip("How much hurt?")]
		public float damage = 10.0f;
		public enum TargetingMode {Self, Other, Both};
		[Tooltip("In a collision, which object should receive damage?")]
		public TargetingMode targetingMode = TargetingMode.Both;
		[Tooltip("How fast is the minimum speed we need to do damage?")]
		public float speedThreshold = 20.0f;
		[Tooltip("The health component receiving the damage, if none then we will use the component on this object")]
		public Health health;
		public HelpInfo help = new HelpInfo("This component sends 'ModifyHealth' to objects involved in collisions. Crash your car at high speed? This component decides how much " +
			"damage is dealt.");

		public bool debug = false;

		void Start () {
			if (GetComponent<Collider>() == null || GetComponent<Rigidbody>() == null) {
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
				if (debug)
					Debug.Log ("Apply impact damage");
				if (targetingMode == TargetingMode.Self || targetingMode == TargetingMode.Both)
					gameObject.BroadcastMessage("ModifyHealth", damage * collision.relativeVelocity.magnitude, SendMessageOptions.DontRequireReceiver);
				else
				if (targetingMode == TargetingMode.Other || targetingMode == TargetingMode.Both)
					collision.gameObject.BroadcastMessage("ModifyHealth", damage * collision.relativeVelocity.magnitude, SendMessageOptions.DontRequireReceiver);
				
			}
		}
	}
}