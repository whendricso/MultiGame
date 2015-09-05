using UnityEngine;
using System.Collections;

//used to synchronize Mecanim state machines over Photon
[RequireComponent (typeof(PhotonView))]
[RequireComponent (typeof(Animator))]
public class PhotonMecanimRelay : Photon.MonoBehaviour {

	public AnimatedState[] animatedStates;

	public MultiModule.HelpInfo help = new MultiModule.HelpInfo("Allows for a Mecanim Trigger to be activated across the Photon network. You can set a list of triggers with " +
		"'Animated States' and use the 'AnimateState' message, passing in an integer representing the index of the AnimatedState you want to use.\n" +
		"'TriggerAnimation' takes a string, and invokes the trigger with that name. See the Mecanim documentation for more information about Triggers.");

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

	public void AnimateState (int animatedState) {
		TriggerAnimation(animatedStates[animatedState].trigger);
	}
	
	public void TriggerAnimation (string anim) {
		if (photonView.isMine) {
			photonView.RPC("AnimateByMecanim", PhotonTargets.All, anim);
		}
	}

	[PunRPC]
	void AnimateByMecanim (string triggerName) {
		animator.SetTrigger(triggerName);
	}

}
//copyright 2014 William Hendrickson