using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Input/Timed Message")]
	public class TimedMessage : MultiModule {


		[RequiredFieldAttribute("How long does the timer last?",RequiredFieldAttribute.RequirementLevels.Required)]
		public float timeDelay = 0.0f;
		[Tooltip("How much should that amount vary?")]
		public float variance = 0.0f;
		[HideInInspector]
		public string message = "";
		[Tooltip("What message should we send?")]
		public MessageManager.ManagedMessage managedMessage;
//		[Tooltip("Message target override")]
//		public GameObject target;
		[Tooltip("Should the timer start automatically as soon as it's created?")]
		public bool autoStart = true;
		[Tooltip("Does the timer repeat automatically>")]
		public bool looping = false;
		[Tooltip("Is this timer restricted to starting just one task at a time? (if true, will abort other tasks that would otherwise be stacked on top)")]
		public bool oneAtATime = true;
		[Tooltip("Enable debugging to tell MultiGame to give you useful messages in the console when something happens.")]
		public bool debug = false;

		public HelpInfo help = new HelpInfo("This component sends messages based on a timer. Accepts the 'StartTimer' and 'Abort' messages.");

		// Use this for initialization
		void Start () {
			if (message != "" && string.IsNullOrEmpty(managedMessage.message))
				managedMessage.message = message;
//			if (target == null)
//				target = gameObject;
			if (managedMessage.target == null)
				managedMessage.target = gameObject;
			if (autoStart)
				StartCoroutine(DelayedMessage(timeDelay));
			if (looping && timeDelay < 0.001) {
				Debug.LogWarning("Warning! Possible unstable loop detected in TimedMessage " + gameObject.name + ". Increase the Time Delay or disable looping if the game becomes unstable.");
			}
		}

		void OnValidate () {
			MessageManager.UpdateMessageGUI(ref managedMessage, gameObject);
		}

		public MessageHelp startTimerHelp = new MessageHelp("StartTimer", "Starts this timer, useful if it doesn't start automatically.");

		public void StartTimer () {
			if (oneAtATime)
				Abort();
			StartCoroutine(DelayedMessage(timeDelay + Random.Range(-variance, variance)));
		}

		public MessageHelp abortHelp = new MessageHelp("Abort", "Stops execution of the timer immediately.");
		public void Abort () {
			StopAllCoroutines();
		}

		IEnumerator DelayedMessage (float delay) {
			yield return new WaitForSeconds(delay);
			if (this.enabled) {
				if (debug)
					Debug.Log("Timed Message " + gameObject.name + " sent " + managedMessage.message);
				MessageManager.Send(managedMessage);//target.SendMessage(message, SendMessageOptions.DontRequireReceiver);
			}
			if (looping)
				StartCoroutine(DelayedMessage(timeDelay + Random.Range(-variance, variance)));
		}
	}
}
//Copyright 2014 William Hendrickson
