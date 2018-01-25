using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Input/Randomized Message")]
	public class RandomizedMessage : MultiModule {


		[ReorderableAttribute]
		public List<RandomMessage> randomizedMessages = new List<RandomMessage> ();
//		[Tooltip("Messages to send randomly")]
//		public List<MessageManager.ManagedMessage> messages = new List<MessageManager.ManagedMessage>();

		public HelpInfo help = new HelpInfo("This component randomly selects a message to be sent from the list, each with a different probability. The probability of any one message being sent is reduced by adding more messages, the " +
			"'Chance' values do not need to add up to 1");

		public bool debug = false;

		[System.Serializable]
		public class RandomMessage {
			[Tooltip("Chance in percentage of any given message being sent")]
			[Range(0f,1f)]
			public float chance = 0.5f;
			public MessageManager.ManagedMessage message;
		}

		void Start () {
			foreach (RandomMessage msg in randomizedMessages) {
				if (msg.message.target == null)
					msg.message.target = gameObject;
			}
//			chance = Mathf.Clamp(chance, 0f, 1f);
		}

		void OnValidate () {
			for (int i = 0; i < this.randomizedMessages.Count; i++) {
				MessageManager.ManagedMessage _msg = randomizedMessages[i].message;
				MessageManager.UpdateMessageGUI(ref _msg, gameObject);
			}
		}

		public MessageHelp rollRandomHelp = new MessageHelp("RollRandom","Selects a single message from those supplied above. There is a tiny chance that one may be selected even if it has a 0 probability. NOTHING IS IMPOSSIBLE!!!!");
		public void RollRandom () {
			float total = 0;
			for (int i = 0; i < randomizedMessages.Count; i++) {
				total += randomizedMessages [i].chance;
			}

			float selector = Random.value * total;

			for (int j = 0; j < randomizedMessages.Count; j++) {
				if (selector <= randomizedMessages [j].chance) {
					MessageManager.Send (randomizedMessages [j].message);
					return;
				}
			}
		}

//		public void RollProbability (float _chance) {
//			float _result;
//			foreach (MessageManager.ManagedMessage msg in messages) {
//				_result = Random.Range(0f, 1f);
//				if(_result <= _chance) {
//					MessageManager.Send(msg);
//					if (debug)
//						Debug.Log("Randomized Message " + gameObject.name + " sent a message because it rolled a " + _result);
//				}
//			}
//		}


	}
}