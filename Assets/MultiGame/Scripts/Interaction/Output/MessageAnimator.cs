using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Output/Message Animator")]
	public class MessageAnimator : MultiModule {

		[RequiredFieldAttribute("The Mecanim trigger to activate when TriggerAnimation is received, occurs the first and every alternating time after that", RequiredFieldAttribute.RequirementLevels.Optional)]
		public string trigger = "";
		[RequiredFieldAttribute("Optional trigger to send on the second and every alternating time after that", RequiredFieldAttribute.RequirementLevels.Optional)]
		public string returnTrigger = "";
		[RequiredFieldAttribute("Reference to the Animator component we are using, if none specified Message Animator will try to find one on this object",RequiredFieldAttribute.RequirementLevels.Optional)]
		Animator animator;
		private bool triggerSet = true;

		public bool debug = false;

		public HelpInfo help = new HelpInfo("This component sends Animator triggers when it receives the 'TriggerAnimation' message. This allows any message sender to cause Animator state transitions for " +
			"controlling logic or animation. It can also send a trigger that's not included here by receiving the 'TriggerSpecificAnimation' message which takes the name of the trigger as an argument.");

		void OnEnable () {
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

		public MessageHelp triggerAnimationHelp = new MessageHelp("TriggerAnimation","Sends the 'Trigger' defined above to the Animator. Sends the 'Return Trigger' the second time it's called.");
		public void TriggerAnimation () {
			if (!enabled)
				return;
			if (!gameObject.activeInHierarchy)
				return;
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

		public MessageHelp triggerSpecificAnimationHelp = new MessageHelp("TriggerSpecificAnimation","Send any trigger you like to the Animator even if it's not defined above",
			4,"The Mecanim trigger you would like to invoke");
		public void TriggerSpecificAnimation (string _trigger) {
			if (!enabled)
				return;
			if (!gameObject.activeInHierarchy)
				return;
			animator.SetTrigger(_trigger);
		}
	}
}