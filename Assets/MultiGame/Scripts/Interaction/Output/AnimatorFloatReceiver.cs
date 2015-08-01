using UnityEngine;
using System.Collections;

public class AnimatorFloatReceiver : MonoBehaviour {

	public string animatorFloat = "";

	[HideInInspector]
	public Animator animator;

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
