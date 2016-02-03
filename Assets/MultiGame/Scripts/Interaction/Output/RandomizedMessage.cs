using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Output/Randomized Message")]
	public class RandomizedMessage : MultiModule {

		[Tooltip("Chance in percentage of any given message being sent")]
		[Range(0f,1f)]
		public float chance = 0.5f;
		[Tooltip("Messages to send randomly")]
		public List<MessageManager.ManagedMessage> messages = new List<MessageManager.ManagedMessage>();

		public HelpInfo help = new HelpInfo("This component randomly sends each message based on the 'chance'. So if you provide a list of 4 messages, with a chance of .5 (50%)" +
			" then most likely 2 of the 4 messages will be sent.");

		public bool debug = false;

		void Start () {
			foreach (MessageManager.ManagedMessage msg in messages) {
				if (msg.target == null)
					msg.target = gameObject;
			}
			chance = Mathf.Clamp(chance, 0f, 1f);
		}

		void OnValidate () {
			for (int i = 0; i < this.messages.Count; i++) {
				MessageManager.ManagedMessage _msg = messages[i];
				MessageManager.UpdateMessageGUI(ref _msg, gameObject);
			}
		}
		
		public void RollRandom () {
			RollProbability(chance);
		}

		public void RollProbability (float _chance) {
			float _result;
			foreach (MessageManager.ManagedMessage msg in messages) {
				_result = Random.Range(0f, 1f);
				if(_result <= _chance) {
					MessageManager.Send(msg);
					if (debug)
						Debug.Log("Randomized Message " + gameObject.name + " sent a message because it rolled a " + _result);
				}
			}
		}
	}
}