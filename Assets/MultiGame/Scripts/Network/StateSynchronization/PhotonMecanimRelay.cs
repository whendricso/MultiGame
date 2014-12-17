using UnityEngine;
using System.Collections;

//used to synchronize Mecanim state machines over Photon
[RequireComponent (typeof(PhotonView))]
[RequireComponent (typeof(Animator))]
public class PhotonMecanimRelay : Photon.MonoBehaviour {

	public AnimatedState[] animatedStates;

	[HideInInspector]
	public Animator animator;
	[HideInInspector]
	public PhotonView photonView;

	[System.Serializable]
	public class AnimatedState {
		public string trigger = "";
	}

	void Awake () {
		animator = GetComponent<Animator>();
	}

	void Start () {
		photonView = GetComponent<PhotonView>();
	}

	void AnimateState (int animatedState) {
		TriggerAnimation(animatedStates[animatedState].trigger);
	}
	
	void TriggerAnimation (string anim) {
		if (photonView.isMine) {
			photonView.RPC("AnimateByMecanim", PhotonTargets.All, anim);
		}
	}

	[RPC]
	void AnimateByMecanim (string triggerName) {
		animator.SetTrigger(triggerName);
	}

}
//copyright 2014 William Hendrickson