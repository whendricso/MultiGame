using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class CallTrigger : StateMachineBehaviour {

		public string enterTrigger;

		public override void OnStateEnter (Animator _anim, AnimatorStateInfo _animInfo, int _layerIndex) {
			_anim.SetTrigger(enterTrigger);
		}
	}
}