using UnityEngine;
using System.Collections;

public class AnimatorFloatReceiver : MultiModule {

	[Tooltip("The float value of the Animator attached to this object which we're going to set a float for. Use parameter mode = float and input value in managed message sender")]
	public string animatorFloat = "";

	[HideInInspector]
	public Animator animator;

	public HelpInfo help = new HelpInfo("This component sets the value of a float in the attached Animator component");

	void Awake () {
		animator = GetComponentInChildren<Animator>();
		if (animator == null) {
			Debug.LogError("Animator Float Receiver must be attached to a Mecanim character!");
			enabled = false;
			return;
		}
	}

	public void Animate (float _val) {
		if (enabled == false)
			return;

		animator.SetFloat(animatorFloat, _val);
	}
}
