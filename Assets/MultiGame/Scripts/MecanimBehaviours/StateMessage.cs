using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class StateMessage : StateMachineBehaviour {

		public bool targetRoot = false;

		[Tooltip("Message to send when we enter this Mecanim state")]
		public MessageManager.ManagedMessage enterMessage;

		public MultiModule.HelpInfo help = new MultiModule.HelpInfo("State Message sends a message when this state is activated. Combined with Message Animator, this can allow " +
			"state machines to function as branching behavior state machines");

		public override void OnStateEnter (Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex) {
			if (enterMessage.target == null) {
				if (targetRoot)
					enterMessage.target = _animator.gameObject.transform.root.gameObject;
				else
					enterMessage.target = _animator.gameObject;
				enterMessage.target = _animator.gameObject;
			}
			if (!string.IsNullOrEmpty( enterMessage.message))
				MessageManager.Send(enterMessage);
		}

	}
}