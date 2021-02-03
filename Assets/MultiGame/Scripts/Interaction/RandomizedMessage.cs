using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Input/Randomized Message")]
	public class RandomizedMessage : MultiModule {


		[ReorderableAttribute]
		public List<RandomMessage> randomizedMessages = new List<RandomMessage> ();
		public enum StoppingModes {allPassed, singleRandom, stopAtFirst };
		[Tooltip("Should we stop at a single message? If All Passed, then every message which passes the random check will be sent. If Single Random, we will only select one from the list. If Stop At First, we will treat the list like a priority queue and select the first from the list that passed the check.")]
		public StoppingModes stoppingMode = StoppingModes.singleRandom;
		public bool guaranteeSend = false;
		//		[Tooltip("Messages to send randomly")]
		//		public List<MessageManager.ManagedMessage> messages = new List<MessageManager.ManagedMessage>();

		private bool sent = false;
		private List<int> options;

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
			if (options == null)
				options = new List<int>();
			options.Clear();
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

			options.Clear();

			for (int j = 0; j < randomizedMessages.Count; j++) {
				if (selector <= randomizedMessages [j].chance) {
					if (stoppingMode == StoppingModes.allPassed) {
						MessageManager.Send(randomizedMessages[j].message);
						if (debug)
							Debug.Log("Randomized Message " + gameObject.name + " sent " + randomizedMessages[j].message + " which has a chance of " + randomizedMessages[j].chance);
					} else {
						if (stoppingMode == StoppingModes.singleRandom)
							options.Add(j);
					}
					sent = true;

					if (stoppingMode == StoppingModes.stopAtFirst) {
						MessageManager.Send(randomizedMessages[j].message);
						if (debug)
							Debug.Log("Randomized Message " + gameObject.name + " sent " + randomizedMessages[j].message + " which has a chance of " + randomizedMessages[j].chance);
						sent = true;
						return;
					}
				}
			}

			if (debug)
				Debug.Log("Randomized Message " + gameObject.name + " is set to " + stoppingMode + "");

			if (stoppingMode == StoppingModes.singleRandom) {
				int _rnd = -1;
				_rnd = SelectSingleRandom();
				if (_rnd != -1) {
					MessageManager.Send(randomizedMessages[_rnd].message);
					sent = true;
				}
			}

			if (guaranteeSend && !sent) {
				sent = true;
				MessageManager.Send(randomizedMessages[Random.Range(0,randomizedMessages.Count-1)].message);
			}
		}

		private int SelectSingleRandom() {
			if (debug)
				Debug.Log("Selection 1 of " + options.Count + " options passed");
			if (options.Count > 0) {
				if (options.Count == 1) {
					return 0;
				} else {
					return (Random.Range(0, options.Count));
				}
			} else
				return -1;
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