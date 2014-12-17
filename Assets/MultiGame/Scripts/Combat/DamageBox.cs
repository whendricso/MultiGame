using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageBox : MonoBehaviour {

	public float damage = 10.0f;
	public float damageInterval = 2.0f;
	[System.NonSerialized]
	private List<GameObject> targets = new List<GameObject>();

	void Start () {
		targets.Clear();
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
		foreach (GameObject tgt in targets) {
			if (tgt != null)
				tgt.BroadcastMessage("ModifyHealth", damage, SendMessageOptions.DontRequireReceiver);
			else
				targets.Remove(tgt);
		}
		StartCoroutine(SendDamage());
	}
	
}
