using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class CallTrigger : StateMachineBehaviour {

		public string enterTrigger;

		public MultiModule.HelpInfo help = new MultiModule.HelpInfo("Call Trigger will invoke 'Enter Trigger' in this state machine when this state is activated");

		public override void OnStateEnter (Animator _anim, AnimatorStateInfo _animInfo, int _layerIndex) {
			_anim.SetTrigger(enterTrigger);
		}
	}
}