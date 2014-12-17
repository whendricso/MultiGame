using UnityEngine;
using System.Collections;

public class MessageAnimator : MonoBehaviour {

	public string trigger = "";
	public string returnTrigger = "";
	Animator animator;
	private bool triggerSet = true;

	void Start () {
		animator = GetComponentInChildren<Animator>();
		if (animator == null) {
			Debug.LogError("MessageAnimator " + gameObject.name + " must have an Animator on itself or one of it's children.");
			enabled = false;
			return;
		}
		if (trigger == "") {
			Debug.LogError("MessageAnimator " + gameObject.name + " must have a 'trigger' assigned in the inspector, to call on the associated Animator.");
			enabled = false;
			return;
		}
	}

	void Activate () {
		TriggerAnimation();
	}

	void TriggerAnimation () {
		triggerSet = !triggerSet;

		if (!triggerSet)
			animator.SetTrigger(trigger);
		else
			animator.SetTrigger(returnTrigger);
	}
}
