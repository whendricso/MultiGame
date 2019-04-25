using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Combat/Damage Box")]
	[RequireComponent(typeof(Rigidbody))]
	public class DamageBox : MultiModule {

		[Header("Damage Settings")]
		[RequiredField("How much damage does this thing do?")]
		public float damage = 10.0f;
		[RequiredField("How often?")]
		public float damageInterval = 2.0f;
		[System.NonSerialized]
		private List<GameObject> targets = new List<GameObject>();

		public HelpInfo help = new HelpInfo("This represents a 'zone' where anything with a health component can receive damage. Examples are a room filled with poison gas, or " +
			"a floor area covered in acid. Must have a trigger in it's heirarchy. Recommend setting Rigidbody to 'Is Kinematic' = true and 'Use Gravity' = false");

		void Start () {
			targets.Clear();
			StartCoroutine (SendDamage ());
		}

		void OnEnable () {
			StopAllCoroutines ();
			StartCoroutine (SendDamage ());
		}

		void OnDisable () {
			StopAllCoroutines ();
		}

		void OnTriggerEnter (Collider other) {
			targets.Add(other.gameObject);
		}

		void OnTriggerExit (Collider other) {
			if(targets.Contains(other.gameObject))
				targets.Remove(other.gameObject);
		}

		IEnumerator SendDamage() {
			yield return new WaitForSeconds(damageInterval);
			if (targets.Count > 0 && enabled) {
				foreach (GameObject tgt in targets) {
					if (tgt != null)
						tgt.BroadcastMessage ("ModifyHealth", damage, SendMessageOptions.DontRequireReceiver);
					else
						targets.Remove (tgt);
				}
			}
			StartCoroutine(SendDamage());
		}

		[Header("Available Messages")]
		public MessageHelp enableDamageHelp = new MessageHelp ("EnableDamage","Start dealing damage if we aren't already doing so");
		public void EnableDamage () {
			if (!gameObject.activeInHierarchy)
				return;
			enabled = true;
		}

		public MessageHelp disableDamageHelp = new MessageHelp ("DisableDamage","Stop dealing damage until re-enabled");
		public void DisableDamage () {
			enabled = false;
		}
	}
}