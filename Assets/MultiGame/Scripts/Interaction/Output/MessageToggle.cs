using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	[AddComponentMenu("MultiGame/Interaction/Output/MessageToggle")]
	public class MessageToggle : MultiModule {

		[Header("Important - Must be Populated")]
		[Tooltip("Game Objects we will toggle")]
		public GameObject[] gameObjectTargets;
		[Tooltip("Scripts we will toggle")]
		public MonoBehaviour[] scriptTargets;
		[Header("Other Settings")]
		[Tooltip("Light we wish to toggle")]
		public Light lightTarget;
//		[Tooltip("Should we toggle the opposite way?")]
		[HideInInspector]
		public bool invert = false;

		private bool previousVal;

		public HelpInfo help = new HelpInfo("This component toggles scripts and game objects based on messages. The 'Toggle' message takes a boolean value, 'SwapToggle' will alternate the " +
			"current setting. Can be used with MultiMenu, Unity 5 UI, or other message senders. Just make sure that the thing you want to toggle appears in one of the lists above. To add scripts, drag and drop the " +
			"header of the component onto the list itself, and MultiGame will add the reference. To do this on an object not currently being inspected, create a second Inspector by clicking the tiny menu icon next to the lock " +
			"at the top of the Inspector -> Add Tab -> Inspector. Then dock the new inspector somewhere else, so that both are visible. Select the object that has the component you wish to target, then lock that inspector by clicking " +
			"the tiny lock at the top. Finally, select this object (the one with the Message Toggle) and drag and drop the component itself (from it's title text) onto the 'Script Targets' list. This will tell MultiGame to add it to " +
			"the list.");

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

		[Header("Available Messages")]
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
