using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	//used to synchronize Mecanim state machines over Photon
	[AddComponentMenu("MultiGame/Network/Photon Mecanim Relay")]
	[RequireComponent (typeof(Animator))]
	public class PhotonMecanimRelay : PhotonModule {

		public AnimatedState[] animatedStates;

		public MultiModule.HelpInfo help = new MultiModule.HelpInfo("Allows for a Mecanim Trigger to be activated across the Photon network. You can set a list of triggers with " +
			"'Animated States' and use the 'AnimateState' message, passing in an integer representing the index of the AnimatedState you want to use.\n" +
			"'TriggerAnimation' takes a string, and invokes the trigger with that name. See the Mecanim documentation for more information about Triggers.");

		[System.NonSerialized]
		public Animator animator;

		[System.NonSerialized]
		public PhotonView view;

		[System.Serializable]
		public class AnimatedState {
			public string trigger = "";
		}

		void Awake () {
			animator = GetComponent<Animator>();
		}

		void Start () {
			view = GetView();
		}

		public void AnimateState (int animatedState) {
			TriggerAnimation(animatedStates[animatedState].trigger);
		}

		MultiModule.MessageHelp triggerAnimationHelp = new MultiModule.MessageHelp("TriggerAnimation","Causes the supplied Mecanim trigger to fire across all clients connected to this server",4,"The exact name of the trigger we wish to fire");
		public void TriggerAnimation (string anim) {
			if (view.isMine) {
				view.RPC("AnimateByMecanim", PhotonTargets.All, anim);
			}
		}

		[PunRPC]
		void AnimateByMecanim (string triggerName) {
			animator.SetTrigger(triggerName);
		}

	}
}
//copyright 2014 William Hendrickson