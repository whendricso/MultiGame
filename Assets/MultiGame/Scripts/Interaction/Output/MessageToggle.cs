using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Output/MessageToggle")]
	public class MessageToggle : MultiModule {

		[Tooltip("Game Objects we will toggle")]
		public GameObject[] gameObjectTargets;
		[Tooltip("Scripts we will toggle")]
		public MonoBehaviour[] scriptTargets;
		[Tooltip("Light we wish to toggle")]
		public Light lightTarget;
//		[Tooltip("Should we toggle the opposite way?")]
		[HideInInspector]
		public bool invert = false;

		private bool previousVal;

		public HelpInfo help = new HelpInfo("This component toggles scripts and game objects based on messages. The 'Toggle' message takes a boolean value, 'SwapToggle' will alternate the " +
			"current setting. Can be used with MultiMenu, Unity 5 UI, or other message senders.");

		public bool debug = false;

		// Use this for initialization
		void Start () {
			if (invert)
				ToggleOff();
			if (gameObjectTargets.Length > 0)
				previousVal = !gameObjectTargets[0].activeSelf;
			else {
				previousVal = false;
	//			gameObjectTargets[0] = gameObject;
			}
			if (invert)
				previousVal = !previousVal;
		}

		public MessageHelp toggleOnHelp = new MessageHelp("ToggleOn","Enables all 'Game Object Targets' and 'Script Targets'");
		public void ToggleOn () {
			Toggle(true);
		}

		public MessageHelp toggleOffHelp = new MessageHelp("ToggleOff","Disables all 'Game Object Targets' and 'Script Targets'");
		public void ToggleOff () {
			Toggle(false);
		}

		public MessageHelp swapToggleHelp = new MessageHelp("SwapToggle","Reverses the state of all targets");
		public void SwapToggle () {
			Toggle(previousVal);
		}

		public MessageHelp toggleHelp = new MessageHelp("Toggle","Allows you to set the state of all 'Game Object Targets' and 'Script Targets' explicitly",1,"The new state for the targets");
		public void Toggle(bool val) {
			if (debug)
				Debug.Log("Toggle " + val);
			previousVal = !val;
			if (lightTarget != null)
				lightTarget.enabled = val;
			if (gameObjectTargets.Length > 0) {
				foreach (GameObject target in gameObjectTargets)
					if (target != null)
						target.SetActive(val);
			}
			if (scriptTargets.Length > 0) {
				foreach (MonoBehaviour target in scriptTargets) {
					if (target != null)
						target.enabled = val;
				}
				
			}

		}
	}
}
//MultiGame Copyright 2014 William Hendrickson
