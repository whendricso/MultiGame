using UnityEngine;
using System.Collections;

public class AnimationMirror : MonoBehaviour {
	
	public GameObject target;
	public string[] targetAnims;
	public string[] mirrorAnims;//plays mirror anim with corresponding target
	
	void Start () {
		if (target == null) {
			Debug.LogError("Animation Mirror requires a target!");
			gameObject.SetActive(false);
			return;
		}
		if (target.animation == null) {
			Debug.LogError("Animation Mirror requires a target with an Animation component, to copy.");
		}
		if (animation == null) {
			Debug.LogError("Animation Mirror requires an Animation component assigned in the Inspector.");
			gameObject.SetActive(false);
			return;
		}
		if (targetAnims.Length != mirrorAnims.Length) {
			Debug.LogError("Animation Mirror Target Anims and Mirror Anims must have the same number of elements.");
			gameObject.SetActive(false);
			return;
		}
	}
	
	void Update () {
		for ( int i = 0; i < targetAnims.Length; i += 1) {
			if (target.animation.IsPlaying(targetAnims[i]))
				animation.Play(mirrorAnims[i],PlayMode.StopAll);
		}
	}
}
