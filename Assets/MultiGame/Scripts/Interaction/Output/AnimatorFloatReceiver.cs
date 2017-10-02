using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Output/Animator Float Receiver")]
	public class AnimatorFloatReceiver : MultiModule {

		[RequiredFieldAttribute("The float value of the Animator attached to this object which we're going to set a float for. Use parameter mode = float and input value in managed message sender")]
		public string animatorFloat = "";

		[HideInInspector]
		public Animator animator;

		public HelpInfo help = new HelpInfo("This component sets the value of a float in the attached Animator component by receiving the 'Animate' message with a floating-point argument. To use, " +
			"add this component to an object with an Animator on itself or one of it's children. Then, create a 'Float' variable in the Animator and type the name of that Float into the 'Animator Float' field above. " +
			"Finally, send the 'Animate' message using any message sender, with a floating point parameter that indicates the new value desired for the Float in the Animator.");
		public bool debug = false;

		void Awake () {
			animator = GetComponentInChildren<Animator>();
			if (animator == null) {
				Debug.LogError("Animator Float Receiver must be attached to a Mecanim character!");
				enabled = false;
				return;
			}
		}

		public MessageHelp animateHelp = new MessageHelp("Animate","Set a new value for a float in the attached Animator",3,"The new value desired for the Animator Float");
		public void Animate (float _val) {
			if (enabled == false)
				return;
			if (debug)
				Debug.Log ("Animator Float Receiver " + gameObject.name + " is setting the float to" + _val);
			animator.SetFloat(animatorFloat, _val);
		}
	}
}