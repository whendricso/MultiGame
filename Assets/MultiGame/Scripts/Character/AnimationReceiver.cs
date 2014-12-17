using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Animation))]
public class AnimationReceiver : MonoBehaviour {

	public float fadeTime = 0.2f;

	public void FadeToAnim (string anim) {
		animation.CrossFade(anim, fadeTime);
	}

}
