using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Input/StartMessage")]
public class StartMessage : MultiModule {

		[Tooltip("Is this object being added to a pool when it dies?")]
		public bool pool = false;

		[Tooltip("When this object is created, what message should we send?")]
		[Reorderable]
		public MessageManager.ManagedMessage[] messages;

		public bool debug = false;

		public HelpInfo help = new HelpInfo("This component sends messages as soon as the object is created.");

		private void Start() {
			if (!pool)
				StartCoroutine(SendMessages());
		}

		void OnEnable () {
			if (pool)
				StartCoroutine(SendMessages());
		}

		IEnumerator SendMessages() {
			yield return new WaitForEndOfFrame();
			foreach (MessageManager.ManagedMessage _message in messages) {
				if (debug)
					Debug.Log("Start Message " + gameObject.name + " sent " + _message.message);
				if (_message.target == null)
					_message.target = gameObject;
				MessageManager.Send(_message);
			}
		}

		void OnValidate () {
			if (messages == null)
				return;
			for (int i = 0; i < this.messages.Length; i++) {
				MessageManager.ManagedMessage _msg = messages[i];
				MessageManager.UpdateMessageGUI(ref _msg, gameObject);
			}
		}
	}
}