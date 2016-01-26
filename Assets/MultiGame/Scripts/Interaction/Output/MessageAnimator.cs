using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class MessageAnimator : MultiModule {

		[Tooltip("The Mecanim trigger to activate when TriggerAnimation is received, occurs the first and every alternating time after that")]
		public string trigger = "";
		[Tooltip("Optional trigger to send on the second and every alternating time after that")]
		public string returnTrigger = "";
		[Tooltip("Reference to the Animator component we are using")]
		Animator animator;
		private bool triggerSet = true;

		public bool debug = false;

		public HelpInfo help = new HelpInfo("This component sends Animator triggers when it receives the 'TriggerAnimation' message. Alternatively, you can send 'TriggerSpecificAnimation' with" +
			" a string parameter representing the Mecanim trigger you want to activate.");

		void Start () {
			animator = GetComponentInChildren<Animator>();
			if (animator == null) {
				Debug.LogError("MessageAnimator " + gameObject.name + " must have an Animator on itself or one of it's children.");
				enabled = false;
				return;
			}
		}

	//	void Activate () {
	//		TriggerAnimation();
	//	}

		public void TriggerAnimation () {
			if (string.IsNullOrEmpty(trigger))
				Debug.LogError("Message Animator " + gameObject.name + " needs a trigger assigned in the inspector");

			triggerSet = !triggerSet;

			if (debug) {
				if (!triggerSet)
					Debug.Log("Ttriggering animation trigger " + trigger);
				else
					Debug.Log("Triggering return animation " + returnTrigger);
			}

			if (!triggerSet)
				animator.SetTrigger(trigger);
			else {
				if (!string.IsNullOrEmpty(returnTrigger))
					animator.SetTrigger(returnTrigger);
				else
					animator.SetTrigger(trigger);
			}
		}

		public void TriggerSpecificAnimation (string _trigger) {
				animator.SetTrigger(_trigger);
		}
	}
}