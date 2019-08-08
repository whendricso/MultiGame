#pragma warning disable 0414
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiGame;

namespace MultiGame {

	public class BehaviorSequencer : MultiModule {

		[RequiredField("An identifier that differentiates this Behavior Sequencer from others on the same object. If used to satisfy a directive in an attached Utility Module, this should match the name of " +
			"the directive it satisfies.")]
		public string designator = "";
		[Tooltip("Should this sequencer stop all other sequencers on the same object when activated?")]
		public bool exclusive = true;
		public float completionDelay = 0;
		public bool loop = false;

		

		[Reorderable]
		public List<Action> sequence = new List<Action>();
		
		public HelpInfo help = new HelpInfo("Behavior Sequencer allows you to define a sequence of actions that can be started or interrupted by sending messages. To use, " +
			"first assign the Actions you wish this sequencer to take. Then, at the appropriate time, send the 'StartSequence' message to this object with a string argument containing the " +
			"'Designator' assigned above. This will initiate this sequencer and optionally stop all other sequencers on this object if this sequencer is set to 'Exclusive'. A Utility Module can " +
			"be used to select from a list of sequences based on current in-game conditions of a given agent.");

		public bool debug = false;

		[System.Serializable]
		public class Action {
			string name;
			public float delay;
			public MessageManager.ManagedMessage message;

			public Action() {
				name = "";
				delay = 0;
			}
		}

		void Start() {
			foreach (Action _action in sequence) {
				if (_action.message.target == null)
					_action.message.target = gameObject;
			}
		}

		private void OnValidate() {
			MessageManager.ManagedMessage _msg;
			foreach (Action _action in sequence) {
				_msg = _action.message;
				MessageManager.UpdateMessageGUI(ref _msg, gameObject);
			}

			
		}

		

		public MessageHelp startSequenceHelp = new MessageHelp("StartSequence","Initializes sequences on this object with a matching 'Designator', defined above.",4,"If the 'Designator' matches the string argument supplied, the BehaviorSequencer will activate.");
		public void StartSequence(string _designator) {
			if (!enabled)
				return;
			if (!gameObject.activeInHierarchy)
				return;
			if (_designator != designator)
				return;
			if (debug)
				Debug.Log("Behavior Sequencer " + gameObject.name + " is starting sequence " + _designator);
			if (exclusive) {
				gameObject.SendMessage("InterruptAll",SendMessageOptions.DontRequireReceiver);
			}
			float cumulativeTime = 0;
			foreach (Action _act in sequence) {
				cumulativeTime += _act.delay;
				StartCoroutine(TakeAction(_act, cumulativeTime));
			}
			StartCoroutine(CompleteSequence(cumulativeTime + completionDelay));
		}

		private IEnumerator CompleteSequence(float _delay) {
			yield return new WaitForSeconds(_delay);
			if (loop)
				StartSequence(designator);
		}

		public MessageHelp takeSelectedActionHelp = new MessageHelp("TakeSelectedAction","Allows you to schedule a specific action right now.",2,"The index of the Action in the Sequence. These are zero-indexed, so the first one is zero, second one is one etc.");
		private void TakeSelectedAction(int _selector) {
			if (sequence.Count < _selector) {
				TakeAction(sequence[_selector]);
			}
		}

		private IEnumerator TakeAction(Action _action) {
			yield return new WaitForSeconds(_action.delay);

			MessageManager.Send(_action.message);
		}

		private IEnumerator TakeAction(Action _action, float delayOverride) {
			yield return new WaitForSeconds(delayOverride);

			MessageManager.Send(_action.message);
		}

		public MessageHelp interruptHelp = new MessageHelp("Interrupt","Stops all scheduled actions on this BehaviorSequencer only if the designator matches the supplied string.",4,"The designator associated with the BehaviorSequencers you wish to Interrupt");
		/// <summary>
		/// Stops all scheduled actions on this BehaviorSequencer only if the designator matches the supplied string.
		/// </summary>
		/// <param name="_designator">The designator associated with the BehaviorSequencers you wish to Interrupt</param>
		public void Interrupt(string _designator) {
			if (_designator != designator)
				return;
			StopAllCoroutines();
		}

		public MessageHelp interruptAllHelp = new MessageHelp("InterruptAll","Immediately stops all actions scheduled on this BehaveiorSequencer");
		public void InterruptAll() {
			StopAllCoroutines();
		}
	}
}
