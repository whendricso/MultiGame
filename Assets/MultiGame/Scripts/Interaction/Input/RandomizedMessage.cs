using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Input/Randomized Message")]
	public class RandomizedMessage : MultiModule {


		[ReorderableAttribute]
		public List<RandomMessage> randomizedMessages = new List<RandomMessage> ();
		[Tooltip("Should we select only one item, or all items that pass the random roll? If true, we will select only one item.")]
		public bool stopAtFirst = true;
		public bool guaranteeSend = false;
		//		[Tooltip("Messages to send randomly")]
		//		public List<MessageManager.ManagedMessage> messages = new List<MessageManager.ManagedMessage>();

		private bool sent = false;

		public HelpInfo help = new HelpInfo("This component randomly selects a message to be sent from the list, each with a different probability. The probability of any one message being sent is reduced by adding more messages, the " +
			"'Chance' values do not need to add up to 1.\n" +
			"When 'RollRandom' is called, we will go through each item in the list starting with the first, and compare it with the roll of an imaginary 100-sided die (represented by values between 0 and 1). We select the first item we find with a greater value and then optionally stop looking.\n" +
			"To use, first add some entries to the 'Randomized Messages' list. For example, you could add a Sounder component, and a Destructible component, and add both the " +
			"'PlaySound' and 'Destruct' messages to the list, and adjust their chance. When you want a message to be randomly selected, send the 'RollRandom' message to this object.");

		public bool debug = false;

		[System.Serializable]
		public class RandomMessage {
			[Tooltip("Chance in percentage of any given message being sent")]
			[Range(0f,1f)]
			public float chance = 0.5f;
			public MessageManager.ManagedMessage message;
		}

		void OnEnable () {
			sent = false;
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
			if (!gameObject.activeInHierarchy)
				return;
			sent = false;
			float total = 0;
			for (int i = 0; i < randomizedMessages.Count; i++) {
				total += randomizedMessages [i].chance;
			}

			float selector = Random.value * total;
			if (debug)
				Debug.Log("Randomized Message " + gameObject.name + " rolled " + selector);

			for (int j = 0; j < randomizedMessages.Count; j++) {
				if (selector <= randomizedMessages [j].chance) {
					MessageManager.Send (randomizedMessages [j].message);
					sent = true;
					if (debug)
						Debug.Log("Randomized Message " + gameObject.name + " sent " + randomizedMessages[j].message);
					if (stopAtFirst)
						return;
				}
			}

			if (guaranteeSend && !sent) {
				sent = true;
				MessageManager.Send(randomizedMessages[Random.Range(0,randomizedMessages.Count-1)].message);
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