using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class StateMessage : StateMachineBehaviour {

		[Tooltip("Message to send when we enter this Mecanim state")]
		public MessageManager.ManagedMessage enterMessage;

		public override void OnStateEnter (Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex) {
			if (enterMessage.target == null)
				enterMessage.target = _animator.gameObject;
			if (!string.IsNullOrEmpty( enterMessage.message))
				MessageManager.Send(enterMessage);
		}

	}
}