using UnityEngine;
using System.Collections;

public class MessageAnimator : MultiModule {

	[Tooltip("The Mecanim trigger to activate when TriggerAnimation is received, occurs the first and every alternating time after that")]
	public string trigger = "";
	[Tooltip("Optional trigger to send on the second and every alternating time after that")]
	public string returnTrigger = "";
	[Tooltip("Reference to the Animator component we are using")]
	Animator animator;
	private bool triggerSet = true;

	public HelpInfo help = new HelpInfo("This component sends Animator triggers when it receives the 'TriggerAnimation' message");

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

//	void Activate () {
//		TriggerAnimation();
//	}

	public void TriggerAnimation () {
		triggerSet = !triggerSet;

		if (!triggerSet)
			animator.SetTrigger(trigger);
		else
			animator.SetTrigger(returnTrigger);
	}
}
